using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoeCrawlInOut : FoeBase
{
    private Coroutine attack_cor;            
    private string str_attack = "attack";

    bool cs_entry = true, cs_attack = false, cs_flip = false,
     cs_idle = false, cs_crawl_out = false, cs_crawl_in = false, cs_freez = false;

    bool ts_crawl_out = false, ts_crawl_in = false, ts_flip = false,
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
            ts_attack = cs_crawl_out && !cs_attack;
            ts_crawl_in = cs_attack && !is_player_in_walk_distance() && !cs_crawl_in && atack_anim.GetCurrentAnimatorStateInfo(0).IsName(str_attack);
            ts_flip = !is_looking_at_player() && cs_attack;

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
                cs_crawl_in = false;

                hiden_idle();                
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
            }

            if (ts_crawl_in)
            {
                cs_crawl_in = true;
                cs_attack = false;
                cs_crawl_out = false;                
                crawl_in();
            }

            if (ts_flip)
            {                
                flip();
            }

            yield return null;
        }

    }
}


/*
  if (is_player_in_active_distance())
            {
                if (!is_looking_at_player())
                {
                    flip();
                    yield return wait_flip;
                    continue;
                }

                if (is_player_in_walk_distance())
                {                    
                    if (atack_anim.GetCurrentAnimatorStateInfo(0).IsName(str_idle))
                    {                        
                        crawl_out();
                        yield return wait_grounded_check;
                    }
                    
                    crawl_attack();
                    yield return wait_walk;
                    continue;
                }
                else
                {                    
                    if (atack_anim.GetCurrentAnimatorStateInfo(0).IsName(str_attack))
                    {                        
                        crawl_in();
                        yield return wait_grounded_check;
                    }

                    hiden_idle();
                    yield return wait_walk;
                    continue;
                }
            }
            else
            {
                freeze();
                yield return null;
            }
 */
