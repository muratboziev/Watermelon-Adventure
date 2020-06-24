using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllyDragonflyController : AllyInsectController
{
    bool facing_right = true;
    WaitForSeconds dragonfly_pause = new WaitForSeconds(0.1f);

    //----------------------------------------------------------------------------
    new void Start()
    {
        base.Start();
    }

    //----------------------------------------------------------------------------

    public override bool start_attack()
    {
        return base.attack(attack_coroutine_dragonfly);
    }

    IEnumerator attack_coroutine_dragonfly()
    {
        float cur_walked_distance = 0;                                                                  //расстояние пройденное на текущем шаге                        
        float cur_way_length;                                                        //длина текущего отрезка атаки, расстоние пройденное на текущем отрезке атаки
        bool cur_direction = attack_way[0].transform.position.x < attack_way[1].transform.position.x;   //true если герой смотрит вправо
        bool move_to_last_point = false, attack_finished = false;

        int num_of_attack_points = attack_way.Length;                                                   //кол-во точек атаки

        Vector3 cur_aim = new Vector3(),                                                        //теукщая точка атаки к которой стремится стрекоза
                cur_pos = new Vector3();                                                       //предыдущая точка атаки

        Vector3[] way_point = new Vector3[num_of_attack_points];                                //координаты точек атаки        

        for (int i = 0; i < num_of_attack_points; i++)
            way_point[i] = attack_way[i].transform.position;            //копируем координаты точек атаки чтобы по мере движения героя стрекоза летела по начальным точкам        

        if (facing_right != cur_direction)
        {
            facing_right = cur_direction;
            Flip();
        }

        int cur_point_num = 0;
        transform.position = attack_way[0].transform.position;
        while (cur_point_num < num_of_attack_points - 1)    
        {   

            if (!move_to_last_point)
            {
                cur_pos = way_point[cur_point_num];
                cur_aim = way_point[cur_point_num + 1];                
            }
            else
            {
                cur_pos = transform.position;
                cur_aim = way_point[way_point.Length - 1];
                attack_finished = true;                                                     //дошли до последней точки
            }

            attack_direction_ray.origin = cur_pos;
            attack_direction_ray.direction = get_direction(cur_pos, cur_aim);

            cur_way_length = (cur_pos - cur_aim).magnitude;
            cur_walked_distance = 0;

            while (cur_walked_distance < cur_way_length)
            {
                cur_walked_distance += Time.deltaTime * fly_speed;
                transform.position = attack_direction_ray.GetPoint(cur_walked_distance);

                if (num_of_grabbed_enemies >= max_killed_enemies && !move_to_last_point)
                {
                    move_to_last_point = true;                                                  //индикатор того что надо прерывать движение по оставшимся точкам и лететь к последней точке                   
                    break;
                }

                yield return null;
            }

            
            if (!move_to_last_point)            
            {
                cur_point_num++;
                yield return dragonfly_pause;
            }
            
            if (attack_finished)
            {
                break;
            }
        }

        StartCoroutine(end_attack());
    }
}
