using System.Collections;
using UnityEngine;

public class NPCController : MonoBehaviour
{
    [Header("Links")]
    public GameManager game_man;
    public DialogManager dialog_man;        

    [Header("Parametres")]
    public float move_force;
    public float max_speed;            
    public float deactivate_time;    

    Rigidbody2D rb2d;
    Animator anim;
    Vector2 velocity_limit = new Vector2();
    string npc_evac_point_str = "npc_evacuate_point";
    bool need_evacuate;
    float dir;    
    GameObject right_arm;

    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        need_evacuate = false;
        right_arm = transform.Find("body/arms/right_arm_root").gameObject;
    }
    
    void Update()
    {
        if (need_evacuate)
        {
            if (rb2d.velocity.x < max_speed)        
            {
                rb2d.AddForce(Vector2.right * move_force * dir);
            }
            else
            {
                velocity_limit.x = dir * max_speed;
                velocity_limit.y = rb2d.velocity.y;
                rb2d.velocity = velocity_limit;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(npc_evac_point_str))
        {
            deactivate_npc();
        }
    }

    //--------------------------------------------------------------------------------------------

    bool is_facing_right()
    {
        return transform.position.x < right_arm.transform.position.x;
    }

    public void Flip()
    {
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    void check_direction(GameObject compare_with)
    {
        if ((gameObject.transform.position.x > compare_with.transform.position.x && is_facing_right()) || 
            (gameObject.transform.position.x < compare_with.transform.position.x && !is_facing_right())
            )
        {
                Flip();
        }
        
    }

    public void have_dialog_and_evac()
    {        
        StartCoroutine(have_dialog_and_evacuate());
    }

    IEnumerator have_dialog_and_evacuate()
    {
        //wait for dialog to complete
        check_direction(game_man.hero_contr.gameObject);
        dialog_man.have_dialog();
        while (dialog_man.having_dialog)
            yield return null;

        //evacuate
        check_direction(game_man.box_cont.evac_point.gameObject);
        game_man.player_points.is_friend_saved_level = true;
        anim.Play("run");
        dir = is_facing_right() ? 1f : -1f;
        need_evacuate = true;

        float end_time = Time.time + deactivate_time;
        while (Time.time < end_time)
            yield return null;

        deactivate_npc();        
    }

    void deactivate_npc()
    {
        game_man.audio_man.play_friend_saved();
        gameObject.SetActive(false);
        game_man.gui_manager_hp.update_player_score(friend_saved: true);
    }


}
