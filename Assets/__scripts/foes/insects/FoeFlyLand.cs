using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoeFlyLand : FoeBase
{
    private Coroutine attack_cor;

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

                if (!is_player_in_fly_distance())
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
                else                                                    //проверка fly_distance сделана выше
                {                    
                    fly();
                    yield return wait_grounded_check;                  //чтобы противник успел оторваться от земли до следующей проверки is_grounded
                    continue;                    
                }

            }

            grounded_prev = is_grounded();
            looked_at_player_prev = is_looking_at_player();
            yield return null;
            
        }


    }


}














