using Cinemachine;
using System;
using System.Collections;
using UnityEngine;

public class HeroState
{
    public static int IDLE          = 0;
    public static int RUN           = 1;
    public static int JUMP          = 2;
    public static int DAMAGED       = 3;

    public static int ROLL          = 4;
    public static int KATANA        = 7;
    public static int SHOE          = 8;
    public static int INSECTICIDE   = 9;
    public static int FLAMETHROWER  = 10;    
    public static int SHOTGUN       = 11;    
    public static int GAUSS_GUN     = 12;
    public static int MINIGUN       = 12;
    public static int BOXING        = 12;
    public static int INSECT        = 12;
    public static int CROSSBOW        = 12;

    public static int UNEXISTING_STATE = 99;
}

[Serializable]
public class HeroParams
{
    public int max_hero_hp;
    public int cur_hero_hp;
    public float moveForce;
    public float jumpForce;
    public Impact roll_force = new Impact(0f, 0f, 0f);
    public float maxSpeed;
    public float roll_reload_time;
    public float blink_and_inv_time;
}

[Serializable]
public class WeaponLinks
{
    public string name;
    public GameObject weap_go;
    public ParticleSystem weap_ps;
    public GaussGunController gauss_gun_contr;
    public AllyInsectController ally_ins_contrt;
}

public class HeroController : MonoBehaviour
{    
    [Header("Hero params")]
    public HeroParams initial_hero_params;    
    public HeroParams recalc_hero_params;

    [Header("Checks")]
    public Transform groundCheckLeft;
    public Transform groundCheckRight;
    public Transform sideCheckBottom;
    public Transform sideCheckTop;

    [Header("Links")]
    public GameObject body;
    public GameObject body_rolls;    

    public Rigidbody2D rb2d;            //public for background scroll controller        
    public WeaponManager weap_man;    
    public GameManager game_man;
    //public CinemachineFramingTransposer cinemachineFramingTransposer;

    public WeaponLinks [] weapons_links;

    [Header("PS")]
    public ParticleSystem killEffectParticleSystem;
    public ParticleSystem moveParticleSystem;
    public ParticleSystem dnaPS;

    [Header("Audio")]
    public AudioSource audio_source_walk;
    public AudioSource audio_source_jump;
    public AudioSource audio_source_attack;
    public AudioSource audio_source_attack_insect;
    public AudioSource audio_source_weap_switch;
    public AudioSource audio_source_hit;
    public AudioSource audio_source_died;

    [HideInInspector]
    public bool facingRight = true;
    [HideInInspector]
    public bool block_user_control = false;
    private bool invincible = false;
    private float wait_for_roll_start_time = 0.1f;
    private float time_to_trigger_attack_anim;

    private bool mobile_input = false;

    //-------------------------------------------------------    

    Animator anim;    
    Vector2 velocity_limit = new Vector2();
    Impact impact_val = new Impact(0f, 0f, 0f);
    private WaitForSeconds blink_and_inv_wait_time, time_before_deactivating_hero, wait_for_roll_anim_start;

    //-------------------------------------------------------      

    float move_hor, prev_speed;
    bool grounded = true, grounded_prev = false, sided = false;
    bool jump_pressed = false, attack_pressed = false, attack_released = true;
    bool can_attack = false, attack_done = false, is_reloaded = false, roll_condition = false, weap_switched = true;

    int legs_anim_state, arms_anim_state, prev_legs_anim_state, prev_arms_anim_state;
    int jump_counter = 2;
    int ground_layer, enemy_layer, ally_layer, layer_mask;

    private readonly int legs_state_anim_param = Animator.StringToHash("state_legs"),
                         arms_state_anim_param = Animator.StringToHash("state_arms"),
                         hero_invincible_anim_param = Animator.StringToHash("invincible");

    string jump_string = "Jump", attack_string = "Fire1",
            hor_string = "Horizontal";    
    string idle_anim_state = "idle";

    float prev_walk_sound_play_time = 0f, next_attack_sound_play_time = 0f;

    //lifecycle-------------------------------------------------------------------------------

    void Awake()
    {
        anim = GetComponent<Animator>();
        rb2d = GetComponent<Rigidbody2D>();        

        ground_layer = LayerMask.NameToLayer("Ground");
        enemy_layer = LayerMask.NameToLayer("Enemy");
        ally_layer = LayerMask.NameToLayer("Ally");
        layer_mask = (1 << ground_layer) | (1 << enemy_layer) | (1 << ally_layer);

        blink_and_inv_wait_time = new WaitForSeconds(recalc_hero_params.blink_and_inv_time);
        time_before_deactivating_hero = new WaitForSeconds(2.5f); // killEffectParticleSystem.main.startLifetime.constant
        wait_for_roll_anim_start = new WaitForSeconds(wait_for_roll_start_time);

        block_user_control = false;

        mobile_input = (Application.platform == RuntimePlatform.Android);
    }

    void Update()
    {
        grounded = Physics2D.Linecast(transform.position, groundCheckLeft.position, layer_mask).collider != null
                        ||
                    Physics2D.Linecast(transform.position, groundCheckRight.position, layer_mask).collider != null;

        sided = Physics2D.Linecast(transform.position, sideCheckBottom.position, 1 << ground_layer) || 
                Physics2D.Linecast(transform.position, sideCheckTop.position, 1 << ground_layer);

        if (!mobile_input && !block_user_control)
            move_hor = Input.GetAxis(hor_string);

        if (Input.GetButtonDown(jump_string))
            jump_pressed = true;
        
        if (Input.GetButtonDown(attack_string))
        {
            attack_pressed = true;
            attack_released = false;
        }

        if (Input.GetButtonUp(attack_string))
        {
            attack_pressed = false;
            attack_released = true;
            weap_switched = true;
            weap_man.reload();

            if (weap_man.turn_sound_off_after_attack)
            {   
                audio_source_attack.Stop();
            }
        }

        if (move_hor == 0)                                                  //для устранения дрожания при застрявании у края 
        {
            velocity_limit.x = 0;
            velocity_limit.y = rb2d.velocity.y;
            rb2d.velocity = velocity_limit;
        }

        if (Mathf.Abs(rb2d.velocity.x) > recalc_hero_params.maxSpeed)                        //ограничение максимальной скорости
        {
            velocity_limit.x = Mathf.Sign(rb2d.velocity.x) * recalc_hero_params.maxSpeed;
            velocity_limit.y = rb2d.velocity.y;
            rb2d.velocity = velocity_limit;
        }

        if (grounded && !grounded_prev)                                   //приземлился
        {
            jump_counter = 2;
            audio_source_walk.Play();
        }

        check_and_emit_dirt(condition_air : true);
        grounded_prev = grounded;
    }

    void FixedUpdate()
    {        
        if (move_hor == 0)
        { 
            legs_anim_state = HeroState.IDLE;
            arms_anim_state = HeroState.IDLE;
        }

        if (move_hor != 0 && !sided)        //основное движение && rb2d.velocity.x <= recalc_hero_params.maxSpeed 
        {
            rb2d.AddForce(Vector2.right * move_hor * recalc_hero_params.moveForce * weap_man.weight);
            legs_anim_state = HeroState.RUN;
            arms_anim_state = HeroState.RUN;
            
            if (grounded && (Time.time > prev_walk_sound_play_time + 0.3f * (1 - weap_man.weight)))
            {                
                audio_source_walk.Play();
                prev_walk_sound_play_time = Time.time + 0.3f;
            }
        }

        if (!grounded)
        {
            legs_anim_state = HeroState.JUMP;
            arms_anim_state = HeroState.JUMP;
        }

        if (jump_pressed && jump_counter > 0)                               //jump button pressed
        {            
            legs_anim_state = HeroState.UNEXISTING_STATE;
            arms_anim_state = HeroState.UNEXISTING_STATE;

            rb2d.velocity = Vector2.zero;
            jump_counter--;
            rb2d.AddForce(Vector2.up * recalc_hero_params.jumpForce * weap_man.weight);
            
            audio_source_jump.Play();
        }

        if (attack_pressed)                                                   //attack button pressed
        {
            //weapon considered being reloaded only when both fire button is up and attack animation stopped playing
            is_reloaded = weap_man.reloaded && !is_animation_playing(weap_man.weapon_name);
            roll_condition = weap_man.animation_state != HeroState.ROLL || ((weap_man.animation_state == HeroState.ROLL) && grounded);
            can_attack = (is_reloaded || !weap_man.needs_reload) && roll_condition && weap_man.weapon_switch_completed && weap_switched;

            if (can_attack)
            {
                attack();
                arms_anim_state = weap_man.animation_state;
                time_to_trigger_attack_anim = Time.time;
            }
        }
        
        if (attack_pressed && time_to_trigger_attack_anim + 0.1f > Time.time)
        {
            arms_anim_state = weap_man.animation_state;
            Debug.Log(arms_anim_state);            
        }

        if (move_hor > 0 && !facingRight || move_hor < 0 && facingRight)     //faces wrong direction
        {
            Flip();
        }

        check_and_emit_dirt(condition_air : false);

        if (arms_anim_state != prev_arms_anim_state)
        {
            anim.SetInteger(arms_state_anim_param, arms_anim_state);
            if (arms_anim_state == HeroState.ROLL)
                StartCoroutine(invincible_while_roll());
        }

        if (legs_anim_state != prev_legs_anim_state)
        {
            anim.SetInteger(legs_state_anim_param, legs_anim_state);
            anim.SetFloat("leg_anim_speed", weap_man.weight);
        }

        prev_legs_anim_state = legs_anim_state;
        prev_arms_anim_state = arms_anim_state;

        jump_pressed = false;
        can_attack = false;
        prev_speed = rb2d.velocity.x;
    }

    //actions-------------------------------------------------------------------------------------------

    void attack()
    {   
        impact_val = ImpactType.recoil(weap_man.impact_on_hero, facingRight);

        if (weap_man.animation_state == HeroState.ROLL)
        {
            audio_source_walk.Stop();
            impact_val = ImpactType.recoil(recalc_hero_params.roll_force, facingRight);
            check_and_emit_dirt(unconditional: true);
        }

        StartCoroutine(ImpactType.push(rb2d, impact_val));
        attack_done = weap_man.attack();        

        if (attack_done && (Time.time > next_attack_sound_play_time))  // || (attack_released && weap_man.turn_sound_off_after_attack)))
        {   
            if (!weap_man.use_2nd_audiosource)
            {
                audio_source_attack.clip = weap_man.weapon_attack_sound;
                audio_source_attack.Play();
            }
            else
            {
                audio_source_attack_insect.clip = weap_man.weapon_attack_sound;
                audio_source_attack_insect.Play();
            }
            next_attack_sound_play_time = Time.time + weap_man.attack_sound_delay;
        }

    }

    public void Flip()
    {
        facingRight = !facingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
        //StartCoroutine(change_cinemachine_position());
    }

    void check_and_emit_dirt(bool condition_air = false, bool unconditional = false)        //2й параметр для roll т.к. при нем rb2d.velocity не успевает стать != 0 и ps не стартует
    {
        if (
            (condition_air && ((!grounded && grounded_prev) || (grounded && !grounded_prev))) ||                //проверка для прыжка и приземления
            (!condition_air && prev_speed == 0f && rb2d.velocity.x != 0f && grounded) ||                //проверка для бега
            unconditional
           )
        {
            if (!is_animation_playing(weap_man.weapon_name))
                moveParticleSystem.Emit(5);
        }
    }

    public void check_and_change_direction(GameObject compare_with)
    {
        if ((gameObject.transform.position.x > compare_with.transform.position.x && facingRight) ||
            (gameObject.transform.position.x < compare_with.transform.position.x && !facingRight)
            )
        {
            Flip();
        }

    }

    public void block_hero_control(bool block, bool stop_move)
    {
        if (stop_move)
            move_hor = 0;
        block_user_control = block;
    }

    public IEnumerator evacuate()
    {        
        move_hor = 0;
        rb2d.velocity = Vector2.zero;
        rb2d.angularVelocity = 0;
        yield return new WaitForSeconds(1f);
        move_hor = 1;        
    }

    //mobile input---------------------------------------------------------------------------------------------

    public void press_move_right()
    {
        if (mobile_input && !block_user_control)
           move_hor = 1;

        if (game_man.dialog_man.having_dialog)
            game_man.dialog_man.skip_phrase();
    }    

    public void press_move_left()
    {
        if (mobile_input && !block_user_control)
            move_hor = -1;

        if (game_man.dialog_man.having_dialog)
            game_man.dialog_man.skip_phrase();
    }

    public void release_move_button()
    {
        if (mobile_input && !block_user_control)
            move_hor = 0;
    }

    public void press_jump()
    {
        if (mobile_input && !block_user_control)
            jump_pressed = true;

        if (game_man.dialog_man.having_dialog)
            game_man.dialog_man.skip_phrase();
    }

    public void press_attack()
    {
        if (mobile_input && !block_user_control)
            attack_pressed = true;

        if (game_man.dialog_man.having_dialog)
            game_man.dialog_man.skip_phrase();
    }

    public void release_attack()
    {
        if (mobile_input && !block_user_control)
        {
            attack_pressed = false;
            weap_man.reload();
        }
    }   

    //colisions------------------------------------------------------------------------------------------------

    public void collision_with_foe(GameObject foe)      //collision with foe attack area
    {
        if (!invincible)                
        {
            recalc_hero_params.cur_hero_hp--;
            game_man.gui_manager_hp.update_player_hp_sprite(recalc_hero_params.cur_hero_hp);

            if (recalc_hero_params.cur_hero_hp > 0)
            {
                anim.SetInteger(legs_state_anim_param, HeroState.DAMAGED);
                anim.SetInteger(arms_state_anim_param, HeroState.DAMAGED);
                prev_legs_anim_state = HeroState.DAMAGED;
                prev_arms_anim_state = HeroState.DAMAGED;

                StartCoroutine(blink_and_invincible());
                impact_val = ImpactType.damage(WeaponManager.foe_impact_on_hero, foe.transform, transform);

                StartCoroutine(ImpactType.push(rb2d, impact_val));

                audio_source_hit.Play();
            }
            else if (rb2d.simulated)                //проверка чтобы не было больше 1 вызова от OnTriggerStay2D в hero_vuln_area
            {
                StartCoroutine(deactivate_hero());

                audio_source_died.Play();                
            }
        }
    }

    public void collision_with_weapon(GameObject weapon)        //collision with weapon
    {
        attack_pressed = false;
        weap_man.pick_weapon(weapon);
    }

    public void collision_with_pill(GameObject pill)        //collision with pill
    {
        bool is_pill = pill.name.Contains("pill");
        Animator take_pill_anim;

        if (recalc_hero_params.cur_hero_hp < recalc_hero_params.max_hero_hp || is_pill)
        {
            if (is_pill)                                                                         //pill
            {
                game_man.audio_man.play_pick_pill();
                recalc_hero_params.cur_hero_hp = recalc_hero_params.max_hero_hp + 1;
            }
            else                                                                                                    //fertilizer
            {
                game_man.audio_man.play_pick_fertilizer();
                recalc_hero_params.cur_hero_hp += 1;
            }

            game_man.gui_manager_hp.update_player_hp_sprite(recalc_hero_params.cur_hero_hp, took_pill: is_pill);

            pill.transform.parent = gameObject.transform.Find("colliders/pill_position");
            pill.transform.localPosition = Vector3.zero;
            take_pill_anim = pill.GetComponent<Animator>();
            StartCoroutine(take_pill(pill, take_pill_anim));
        }
    }

    public void collision_with_dna(GameObject dna)
    {
        game_man.audio_man.play_pick_dna();

        dna.SetActive(false);        
        game_man.player_points.evo_points_gained_level++;
        game_man.player_points.is_dna_found_level = true;

        game_man.gui_manager_hp.update_player_score(picked_dna:true);

        dnaPS.Emit(11);
    }

    public void collision_with_dna_part(GameObject dna_part)        //collision with dna part
    {
        game_man.audio_man.play_pick_dna_part();
        game_man.player_points.simple_points_gained_level++;        
        game_man.gui_manager_hp.update_player_score(picked_dna_part:true);
        game_man.pickable_object_spawn_manager.return_dna_part_to_stack(dna_part);
    }

    //coroutines--------------------------------------------------------------------------------------------------

    /*IEnumerator change_cinemachine_position()
    {
        float required_pos = 1 - cinemachineFramingTransposer.m_ScreenX;
        float pos_delta = 0.02f;

        if (required_pos < 0.5)
            pos_delta *= -1;

        while ((cinemachineFramingTransposer.m_ScreenX < required_pos && pos_delta > 0) || (cinemachineFramingTransposer.m_ScreenX > required_pos && pos_delta < 0))
        {
            cinemachineFramingTransposer.m_ScreenX += pos_delta;
            yield return null;
        }

        if (pos_delta > 0)
            cinemachineFramingTransposer.m_ScreenX = 0.75f;
        else
            cinemachineFramingTransposer.m_ScreenX = 0.25f;

    }*/

    IEnumerator take_pill(GameObject pill, Animator take_pill_anim)
    {
        take_pill_anim.Play("take_pill");
        yield return new WaitForSeconds(1.4f);
        pill.SetActive(false);
    }

    IEnumerator invincible_while_roll()
    {
        invincible = true;

        while (is_animation_playing(idle_anim_state))           //ждем пока не начнет проигрываться анимация roll
        {
            yield return wait_for_roll_anim_start;
        }

        while (is_animation_playing(weap_man.weapon_name))     //ждем когда закончится проигрываться анимация roll 
        {
            yield return wait_for_roll_anim_start;
        }

        yield return wait_for_roll_anim_start;                  //ждем дополнительное время
        yield return wait_for_roll_anim_start;
        yield return wait_for_roll_anim_start;
        yield return wait_for_roll_anim_start;

        invincible = false;
        yield return null;
    }

    IEnumerator blink_and_invincible()
    {
        invincible = true;

        anim.SetBool(hero_invincible_anim_param, true);

        yield return blink_and_inv_wait_time;
        anim.SetBool(hero_invincible_anim_param, false);

        invincible = false;
        yield return null;
    }

    public IEnumerator deactivate_hero()
    {
        body.SetActive(false);
        body_rolls.SetActive(false);        
        weap_man.dectivate_cur_weapon();
        rb2d.simulated = false;

        killEffectParticleSystem.Emit(7);        

        yield return time_before_deactivating_hero;

        game_man.stop_enemies();
        game_man.menu_man.make_transition(game_man.menu_man.cur_menu_state.transition[2], game_man.cur_scene_name);
    }

    //helpers--------------------------------------------------------------------------------------------------------------

    int get_direction(bool d)
    {
        if (d) return 1;
        return -1;
    }

    bool is_animation_playing(string anim_name)
    {
        return anim.GetCurrentAnimatorStateInfo(weap_man.anim_layer_index).IsName(anim_name);
    }

    //listeners------------------------------------------------------------------------------------------------------------

    public void weapon_switched()
    {
        play_weap_switched_sound();

        if (attack_pressed)
            weap_switched = false;        
    }    

    public void play_weap_switched_sound()
    {
        //if (weap_man.turn_sound_off_after_attack)
            audio_source_attack.Stop();

        audio_source_weap_switch.clip = weap_man.weapon_pick_sound;
        audio_source_weap_switch.Play();
    }

}

