using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class AllyInsectController : MonoBehaviour
{
    public WeaponManager weap_man;

    public float fly_speed = 150f;
    public float time_with_pray = 0.5f;
    public int max_killed_enemies = 1;
    public GameObject grab_point;
    public GameObject[] attack_way;
    //public bool can_grab_ally;    

    //------------------------------------------------
    protected GameObject preyGameObj;
    protected Ray2D attack_direction_ray = new Ray2D();
    protected Animator anim;
    protected int num_of_grabbed_enemies = 0;

    private Coroutine attack_cor;
    private bool attacks = false, prey_handled = false;
    private Rigidbody2D prey_rb2d;
    private AudioClip attack_sound_audio_clip_name;

    private string foe_vuln_tag = "foe_vuln_area_weapon", player_tag = "Player", box_tag = "box";

    private readonly int grab_anim_param = Animator.StringToHash("grab_foe");


    //---------------------------------------------------------------------------------------------------------

    protected void Start()
    {
        anim = GetComponent<Animator>();        
        gameObject.SetActive(false);        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.gameObject.transform.root.CompareTag(box_tag))
        {
            if (foe_vuln_tag.Contains(collision.gameObject.tag))
            {
                if (preyGameObj == null && num_of_grabbed_enemies < max_killed_enemies)
                {
                    grab_prey(collision);
                    StartCoroutine(kill_pray());
                }
            }

            if (collision.gameObject.CompareTag(player_tag))//|| (collision.gameObject.CompareTag(ally_tag) && can_grab_ally) )
            {
                if (preyGameObj == null && num_of_grabbed_enemies < max_killed_enemies)
                {
                    grab_prey(collision);
                    StartCoroutine(release_pray());                    
                }
            }
        }
    }

    //------------------------------------------------------------------------------------------------------------

    public virtual bool start_attack()
    {
        return false;
    }

    public bool attack(Func<IEnumerator> cur_attack_cor)
    {
        if (!attacks)
        {
            attack_sound_audio_clip_name = weap_man.cur_weapon_audio_clip;

            prey_handled = false;
            preyGameObj = null;
            anim.SetBool(grab_anim_param, false);

            attacks = true;
            gameObject.SetActive(true);

            num_of_grabbed_enemies = 0;
            attack_cor = StartCoroutine(cur_attack_cor());

            return true;
        }
        else
        {
            return false;
        }
    }

    public IEnumerator end_attack()
    {
        StopCoroutine(attack_cor);

        if (preyGameObj != null)
        {     
            if (foe_vuln_tag.Contains(preyGameObj.gameObject.tag))
            {
                StartCoroutine(kill_pray());
            }
            if (preyGameObj.CompareTag(player_tag))
            {
                StartCoroutine(release_pray());
            }
        }

        while (preyGameObj != null && !prey_handled)    //если есть добыча ждем пока с ней закончатся все действия чтобы не отключить ее преждевременно вместе насекомым-союзником
            yield return null;

        attacks = false;
        anim.SetBool(grab_anim_param, false);

        if (attack_sound_audio_clip_name.Equals(weap_man.hero_contr.audiosource_attack_insect.clip))       //не останавливаем звук атаки если проигрывается звук атаки другого насекомого
            weap_man.hero_contr.audiosource_attack_insect.Stop();

        gameObject.SetActive(false);
    }


    void grab_prey(Collider2D collision)
    {        
        prey_rb2d = collision.gameObject.GetComponentInParent<Rigidbody2D>();

        preyGameObj = prey_rb2d.gameObject;

        FoeBase fb = preyGameObj.GetComponentInParent<FoeBase>();
        if (fb)
        {
            fb.stop_attack();
            fb.stop_and_idle();
        }

        prey_rb2d.bodyType = RigidbodyType2D.Kinematic;
        prey_rb2d.velocity = Vector2.zero;

        preyGameObj.transform.parent = gameObject.transform;
        preyGameObj.transform.position = grab_point.transform.position;

        anim.SetBool(grab_anim_param, true);

        num_of_grabbed_enemies++;
    }

    IEnumerator kill_pray()
    {
        float start_time = Time.time;
        while (Time.time < start_time + time_with_pray)
        {
            yield return null;
        }

        FoeTakeDamage ftd = preyGameObj.GetComponent<FoeTakeDamage>();        
        anim.SetBool(grab_anim_param, false);
        ftd.get_damage(gameObject, distance: -2f);        

        post_process_pray();

        prey_handled = true;        
    }

    IEnumerator release_pray()
    {
        float start_time = Time.time;
        while (Time.time < start_time + time_with_pray)
        {
            yield return null;
        }

        anim.SetBool(grab_anim_param, false);
        post_process_pray();

        prey_handled = true;
    }

    void post_process_pray()                                    
    {
        //if (foe_tags.Contains(preyGameObj.gameObject.tag))
        prey_rb2d.bodyType = RigidbodyType2D.Dynamic;
        preyGameObj.transform.parent = null;
        prey_rb2d = null;
        preyGameObj = null;        
    }

    //--------------------------------------------------------------------------------------------------

    protected Vector3 get_direction(Vector3 init_pos, Vector3 target_pos)               //направление от 1-й точки ко второй https://docs.unity3d.com/Manual/DirectionDistanceFromOneObjectToAnother.html
    {
        var dist = target_pos - init_pos;
        return dist / dist.magnitude;
    }

    protected void Flip()
    {
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }


}
