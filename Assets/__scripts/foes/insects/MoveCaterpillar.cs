using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCaterpillar : FoeBase
{

    private Coroutine attack_cor;

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

                if (!is_looking_at_player())
                {
                    stop_and_idle();
                    yield return wait_flip;
                    flip();                    
                }

                stop_and_idle();
                yield return wait_walk;
                walk();
                
            }

            yield return null;
        }


    }
}


/*if (move_anim.GetCurrentAnimatorStateInfo(0).IsName(move_state))
{
    if (!moved)
    {
        rb2d.AddForce(Vector2.left * moveForce * moveForce);
        moved = true;
    }
}

if (move_anim.GetCurrentAnimatorStateInfo(0).IsName("idle"))
{
    moved = false;
    prev_time = Time.timeSinceLevelLoad;
}*/
