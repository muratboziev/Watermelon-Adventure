using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoeWalk : FoeBase
{
    public bool caterpillar = false;
    public Coroutine attack_cor;

    private void Start()
    {
        grounded_prev = is_grounded();
        looked_at_player_prev = is_looking_at_player();
    }

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
            if (is_grounded())
            {                
                if (!is_player_in_active_distance())
                {                    
                    freeze();
                    yield return null;
                    continue;
                }

                if (!grounded_prev)
                {
                    stop_and_idle();
                    yield return wait_flip;
                    grounded_prev = is_grounded();
                    continue;
                }

                if (!is_player_in_walk_distance())
                {
                    stop_and_idle();
                    yield return wait_flip;
                    continue;
                }

                if (!is_looking_at_player())
                {
                    stop_and_idle();
                    flip();
                    yield return wait_flip;
                    continue;
                }
                else
                {
                    walk();                    
                    if (caterpillar)
                    {
                        yield return wait_stop;
                        stop_and_idle();
                        yield return wait_stop;
                    }
                    yield return null;
                    continue;
                } 
            }

            grounded_prev = is_grounded();
            //looked_at_player_prev = is_looking_at_player();
            yield return null;
        }
    }
}