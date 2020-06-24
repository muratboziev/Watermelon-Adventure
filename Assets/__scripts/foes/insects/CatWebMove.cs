using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatWebMove : FoeBase
{
    [Header("Web")]

    public GameObject web_start_point;
    public LineRenderer line_renderer;
    public float move_speed = 150f;
    public float max_web_length = 200f;

    private Coroutine attack_cor;                

    Ray2D attack_direction_ray = new Ray2D();

    bool cs_entry = true, cs_attack = false, cs_attack_complete = false, 
         cs_idle = false, cs_crawl_out = false, cs_crawl_in = false, cs_freez = false;

    bool ts_crawl_out = false, ts_crawl_in = false, 
         ts_freeze = false, ts_idle = false, ts_attack = false;

    public override void start_attack()
    {
        attack_cor = StartCoroutine(attack());
    }

    public override void stop_attack()
    {
        StopCoroutine(attack_cor);
    }

    IEnumerator attack()
    {        

        while (true)
        {
            ts_freeze = cs_entry || (cs_idle && !is_player_in_active_distance() && !cs_freez);
            ts_idle = (cs_crawl_in || cs_freez) && is_player_in_active_distance() && !cs_idle;
            ts_crawl_out = cs_idle && is_player_in_walk_distance() && !cs_crawl_out;
            ts_crawl_in = cs_crawl_out && cs_attack_complete && !cs_crawl_in;            
            ts_attack = cs_crawl_out && !cs_attack;

            if (ts_freeze)
            {
                cs_freez = true;
                cs_idle = false;
                cs_entry = false;

                freeze();
            }

            if (ts_idle)
            {                
                cs_idle = true;
                cs_freez = false;                

                hiden_idle();

                if (cs_crawl_in)
                    yield return wait_walk;
                cs_crawl_in = false;
            }

            if (ts_crawl_out)
            {                
                cs_crawl_out = true;
                cs_idle = false;
                crawl_out();                
            }

            if (ts_attack)
            {
                cs_attack = true;
                crawl_attack();
                StartCoroutine(attack_coroutine_cat_web());
            }

            if (ts_crawl_in)
            {
                cs_crawl_in = true;
                cs_attack = false;                
                cs_crawl_out = false;
                cs_attack_complete = false;
                crawl_in();                
            }

            yield return null;
        }    
    }

    IEnumerator attack_coroutine_cat_web()
    {
        float init_pos_x = gameObject.transform.position.x;
        float init_pos_y = gameObject.transform.position.y;        

        move_speed = Mathf.Abs(move_speed);
        float cur_web_length = 0f;        
        
        attack_direction_ray.origin = web_start_point.transform.position;
        attack_direction_ray.direction = -transform.up;
        line_renderer.positionCount = 2;
        line_renderer.SetPosition(0, attack_direction_ray.origin);        

        while (true)
        {
            cur_web_length += Time.deltaTime * move_speed;

            transform.position = attack_direction_ray.GetPoint(cur_web_length);
            line_renderer.SetPosition(1, attack_direction_ray.GetPoint(cur_web_length));            

            if (cur_web_length > max_web_length)
            {
                move_speed = -move_speed;                
            }

            if (cur_web_length < 0)
            {
                transform.position = new Vector3(init_pos_x, init_pos_y, 0);                
                cs_attack_complete = true;                
                yield break;
            }

            yield return null;
        }
    }



}



