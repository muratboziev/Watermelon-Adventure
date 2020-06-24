using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllySpiderController : AllyInsectController
{
    public float max_web_length = 200f;
    public LineRenderer line_renderer;    
    

    //----------------------------------------------------------------------------
    new void Start()
    {
        base.Start();
        line_renderer.positionCount = 0;
    }


    //----------------------------------------------------------------------------

    public override bool start_attack()
    {
        return base.attack(attack_coroutine_spider);
    }
    

    IEnumerator attack_coroutine_spider()
    {
        float cur_web_length = 0f;
        bool changed_direction = false;     //менялось ли направление

        cur_web_length = 0f;
        attack_direction_ray.origin = attack_way[0].transform.position;
        attack_direction_ray.direction = -transform.up;
        line_renderer.positionCount = 2;
        line_renderer.SetPosition(0, attack_direction_ray.origin);

        fly_speed = Mathf.Abs(fly_speed);

        while (true)
        {
            cur_web_length += Time.deltaTime * fly_speed;

            transform.position = attack_direction_ray.GetPoint(cur_web_length);
            line_renderer.SetPosition(1, attack_direction_ray.GetPoint(cur_web_length));
            yield return null;

            if ((cur_web_length > max_web_length || num_of_grabbed_enemies >= max_killed_enemies) && !changed_direction)
            {
                fly_speed = -fly_speed;
                changed_direction = true;
            }

            if (cur_web_length < 0)
            {
                StartCoroutine(base.end_attack());
            }

        }
    }
}
