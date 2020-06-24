using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoeAnimState
{
    public static int idle = 0; 
    public static int walk = 1; 
    public static int fly = 2;
    public static int freeze = 3;    
    public static int crawl_out = 4;    
    public static int crawl_in = 5;        
    public static int attack = 6;
    public static int hide = 7;
    
}

[System.Serializable]
public class WaitInterval
{
    public float time_before_fly = 2,
                        time_before_walk = 1,
                        time_before_flip = 1,
                        time_before_stop = 0.5f,
                        time_before_grounded_check = 0.1f;
}

[System.Serializable]
public class MoveData
{
    public float moveForce,
                 flyForce,
                 fly_delta = 20,
                 walk_dist = 100,
                 fly_dist = 250,
                 active_dist = 500;
}

public class FoeBase : MonoBehaviour
{
    [Header("Links")]
    public GameObject player;
    public Transform groundCheck;

    [Header("Data")]
    public MoveData move_data;
    public WaitInterval wait_interval;

    [Header("Anim")]
    public Animator turn_die_anim;
    public Animator atack_anim;
    public float step_speed = 1.5f;    
    
    [Header("Sounds")]
    public AudioSource audioSource_fly;
    public AudioSource audioSource_idle;
    public AudioSource audioSource_walk;
    public AudioSource audioSource_hit;
    public AudioClip fly_sound;
    public AudioClip idle_sound;
    public AudioClip walk_sound;
    public AudioClip hit_sound;
    public AudioClip killed_sound;

    //-------------------------------------------------------------------

    private int ground_layer;
    private readonly int action_anim_param = Animator.StringToHash("action"),
                     facing_right_anim_param = Animator.StringToHash("facing_right");
    private Rigidbody2D rb2d;
    private RaycastHit2D hit_info;

    //-------------------------------------------------------------------

    protected Vector2 move_dir;
    protected bool facingRight = false;    
    protected bool grounded_prev, looked_at_player_prev;    
    protected WaitForSeconds wait_fly, wait_walk, wait_flip, wait_grounded_check, wait_stop;    
        
    Vector3 player_pos;
    float dist;

    WaitForSeconds wait_fly_sound = new WaitForSeconds(0.5f);    

    //------------------------------------------------------------------------

    void Awake()
    {
        //player = GameObject.FindGameObjectsWithTag("Player")[0];
        ground_layer = LayerMask.NameToLayer("Ground");        

        rb2d = GetComponent<Rigidbody2D>();
        atack_anim.speed = step_speed;
        move_dir = new Vector2();

        wait_fly = new WaitForSeconds(wait_interval.time_before_fly);
        wait_walk = new WaitForSeconds(wait_interval.time_before_walk);
        wait_flip = new WaitForSeconds(wait_interval.time_before_flip);
        wait_stop = new WaitForSeconds(wait_interval.time_before_stop);
        wait_grounded_check = new WaitForSeconds(wait_interval.time_before_grounded_check);

        if (audioSource_fly != null)
            audioSource_fly.clip = fly_sound;

        if (audioSource_idle != null)
            audioSource_idle.clip = idle_sound;

        if (audioSource_walk != null)
            audioSource_walk.clip = walk_sound;
    }

    //--------------------------------------------------------------------------------------------------

    public void walk()
    {

        atack_anim.SetInteger(action_anim_param, FoeAnimState.walk);

        dist = vector_to_player().x;

        move_dir.x = move_data.moveForce * Mathf.Sign(dist); // + Random.Range(-10, 10);
        move_dir.y = 0;
        rb2d.velocity = move_dir;

        play_walk_sound();
    }

    public void fly()
    {
        atack_anim.SetInteger(action_anim_param, FoeAnimState.fly);

        dist = vector_to_player().x;
        move_dir.x = dist / 2;// + Mathf.Sign(dist) * fly_delta;
        move_dir.y = move_data.flyForce;

        //rb2d.AddForce(move_dir);
        rb2d.velocity = move_dir;

        play_fly_sound();
    }

    public void stop()
    {
        rb2d.velocity = Vector2.zero;
    }

    public void stop_and_idle()
    {
        if (rb2d.bodyType != RigidbodyType2D.Static)
            rb2d.velocity = Vector2.zero;
        atack_anim.SetInteger(action_anim_param, FoeAnimState.idle);        
        play_idle_sound();
    }

    public void freeze()
    {
        if (rb2d.bodyType != RigidbodyType2D.Static)
            rb2d.velocity = Vector2.zero;
        atack_anim.SetInteger(action_anim_param, FoeAnimState.freeze);
        stop_audioSources();
    }

    public void crawl_in()
    {
        atack_anim.SetInteger(action_anim_param, FoeAnimState.crawl_in);
        stop_audioSources();
    }

    public void crawl_out()
    {        
        atack_anim.SetInteger(action_anim_param, FoeAnimState.crawl_out);
    }

    public void hiden_idle()
    {
        atack_anim.SetInteger(action_anim_param, FoeAnimState.hide);
    }

    public void crawl_attack()
    {
        atack_anim.SetInteger(action_anim_param, FoeAnimState.attack);
        play_walk_sound();
    }

    //--------------------------------------------------------------------------------------------------

    public void play_walk_sound()
    {        
        if (audioSource_walk != null && !audioSource_walk.isPlaying)
        {            
            stop_audioSources();
            audioSource_walk.Play();            
        }
    }

    void play_fly_sound()
    {
        if (audioSource_fly != null && !audioSource_fly.isPlaying)
        {
            stop_audioSources();
            audioSource_fly.Play();
            //StartCoroutine(play_fly_sound());
        }

        /*while (!is_grounded())
        {
            if (!audioSource_fly.isPlaying)
            {                
                audioSource_fly.Play();
                yield return null; //wait_fly_sound;
            }
        }

        audioSource_fly.Stop();
        yield return null;*/
    }

    public void play_hit_sound()
    {
        if (audioSource_hit != null && !audioSource_hit.isPlaying)
        {
            stop_audioSources();
            audioSource_hit.clip = hit_sound;
            audioSource_hit.Play();
        }
    }

    public void play_killed_sound()
    {
        if (audioSource_hit != null && !audioSource_hit.isPlaying)
        {
            stop_audioSources();
            audioSource_hit.clip = killed_sound;
            audioSource_hit.Play();
        }
    }

    public void play_idle_sound()
    {        
        if (audioSource_idle != null && !audioSource_idle.isPlaying)
        {            
            stop_audioSources();
            audioSource_idle.Play();
        }
    }

    public void stop_audioSources()
    {
        if (audioSource_fly != null)
            audioSource_fly.Stop();
        if (audioSource_idle != null)        
            audioSource_idle.Stop();         
        if (audioSource_walk != null)
            audioSource_walk.Stop();
        if (audioSource_hit != null)
            audioSource_hit.Stop();        
    }

    //--------------------------------------------------------------------------------------------------

    public bool is_grounded()
    {
        hit_info = Physics2D.Linecast(transform.position, groundCheck.position, 1 << ground_layer);

        if (hit_info.collider != null)
        {            
            return true;
        }
        
        return false;        
    }    

    public bool is_looking_at_player()
    {
        /*true if foe is looking at player*/
        player_pos = player.transform.position;        
        if (player_pos.x < transform.position.x && facingRight || player_pos.x > transform.position.x && !facingRight)
            return false;
        else
            return true;
    }

    public void flip()
    {
        facingRight = !facingRight;
        turn_die_anim.SetBool(facing_right_anim_param, facingRight);
    }

    //--------------------------------------------------------------------------------------------------

    public Vector2 vector_to_player()
    {        
        return player.transform.position - transform.position;        
    }

    public bool is_player_in_walk_distance()
    {
        return Mathf.Abs(vector_to_player().x) <= move_data.walk_dist;        
    }

    public bool is_player_in_fly_distance()
    {
        return Mathf.Abs(vector_to_player().x) <= move_data.fly_dist;
    }

    public bool is_player_in_active_distance()
    {
        return Mathf.Abs(vector_to_player().x) <= move_data.active_dist;
    }

    //--------------------------------------------------------------------------------------------------

    public virtual void stop_attack()
    {        
    }


    public virtual void start_attack()
    { }
}
