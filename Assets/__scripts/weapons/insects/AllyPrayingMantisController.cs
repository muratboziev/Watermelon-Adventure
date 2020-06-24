using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllyPrayingMantisController : AllyInsectController
{
    public float walk_time;
    public float moveForce;
    public Rigidbody2D rb2d;
    public GameObject groundCheck_1, groundCheck_2, side_check;
    public GameObject p_start, p_end, p_evac, 
                      p_land_11, p_land_12, p_land_21, p_land_22;

    private int ground_layer;    
    private RaycastHit2D hit_info, hit_info2;
    private float time_before_grounded_check = 0.1f;
    private WaitForSeconds wait_grounded_check;
    private bool facing_right = false;
    private int facingRightSign = 1;
    private Vector2 move_dir;
    private int ground_layer_mask;
    private RaycastHit2D rch2d_hor1, rch2d_hor2, rch2d_ver1, rch2d_ver2;

    private int walk_fly_anim_param = Animator.StringToHash("fly_walk"),
                grab_anim_param = Animator.StringToHash("grab");    

    new void Start()
    {
        max_killed_enemies = 3;
        ground_layer = LayerMask.NameToLayer("Ground");
        ground_layer_mask = 1 << ground_layer;
        wait_grounded_check = new WaitForSeconds(time_before_grounded_check);
        rb2d = GetComponent<Rigidbody2D>();        
        base.Start();
    }

    //------------------------------------------------------------------------

    public bool is_grounded()
    {
        hit_info = Physics2D.Linecast(transform.position, groundCheck_1.transform.position, ground_layer_mask);
        hit_info2 = Physics2D.Linecast(transform.position, groundCheck_2.transform.position, ground_layer_mask);        

        if (hit_info.collider != null && hit_info2.collider != null)
        {
            return true;
        }

        return false;
    }

    public bool is_sided()
    {
        hit_info = Physics2D.Linecast(transform.position, side_check.transform.position, ground_layer_mask);

        if (hit_info.collider != null)
        {
            return true;
        }

        return false;
    }

    public void walk()
    {
        move_dir.x = moveForce * facingRightSign;
        move_dir.y = 0;
        rb2d.velocity = move_dir;
    }

    public override bool start_attack()
    {
        bool cur_direction = p_start.transform.position.x < p_end.transform.position.x;   //true если герой смотрит вправо

        if (facing_right != cur_direction)
        {
            facing_right = cur_direction;
            Flip();
        }

        facingRightSign = facing_right ? 1 : -1;

        return base.attack(attack_coroutine_praying_mantis);
    }

    bool check_landing_area(Vector3 p11, Vector3 p12, Vector3 p21, Vector3 p22)
    {
        rch2d_hor1 = Physics2D.Linecast(p11, p12, ground_layer_mask);
        rch2d_hor2 = Physics2D.Linecast(p21, p22, ground_layer_mask);
        rch2d_ver1 = Physics2D.Linecast(p11, p21, ground_layer_mask);
        rch2d_ver2 = Physics2D.Linecast(p12, p22, ground_layer_mask);

        //Debug.DrawLine(p11, p12, Color.red, 100f);

        return rch2d_hor1.collider == null && rch2d_hor2.collider == null && rch2d_ver1.collider != null && rch2d_ver2.collider != null;
    }

    bool find_landing_area(GameObject p_11, GameObject p_12, GameObject p_21, GameObject p_22, out Vector3 land_zone_top_point)
    {
        Vector3 p11 = p_11.transform.position, p12 = p_12.transform.position,
                p21 = p_21.transform.position, p22 = p_22.transform.position;

        float check_step = 10f;
        float search_zone_length = 60f;
        float landing_area_height = 20f;

        //go top 

        land_zone_top_point = p11;


        Vector3 search_top_point = p11, search_bottom_point = p11;
        search_top_point.y += search_zone_length;                       //верхняя точка зоны поиска зоны приземления
        search_bottom_point.y -= search_zone_length;                    //нижняя точка зоны поиска зоны приземления

        for (float j = search_top_point.y; j >= search_bottom_point.y; j -= check_step)         //ищем зону приземления с верхней точки до нижней
        {
            p11.y = j;
            p12.y = j;

            p21.y = p11.y - landing_area_height;
            p22.y = p12.y - landing_area_height;            

            if (check_landing_area(p11, p12, p21, p22))
            {
                land_zone_top_point = p11;
                return true;
            }
        }

        return false;        
    }

    IEnumerator attack_coroutine_praying_mantis()
    {
        float cur_walked_distance = 0;                                                                  //расстояние пройденное на текущем шаге                        
        float cur_way_length;                                                        //длина текущего отрезка атаки, расстоние пройденное на текущем отрезке атаки
        bool can_land = false;

        bool move_to_last_point = false;                

        Vector3 fly_start_point = p_start.transform.position, 
                fly_end_point = p_end.transform.position, 
                fly_evac_point = p_evac.transform.position,
                p_landing_zone_top;

        can_land = find_landing_area(p_land_11, p_land_12, p_land_21, p_land_22, out p_landing_zone_top);


        //----------fly-----------------------------------------

        rb2d.bodyType = RigidbodyType2D.Kinematic;

        anim.SetInteger(walk_fly_anim_param, 0);                

        attack_direction_ray.origin = fly_start_point;
        attack_direction_ray.direction = get_direction(fly_start_point, fly_end_point);        

        //Debug.DrawLine(p_landing_zone_top, new Vector3(p_landing_zone_top.x + 10, p_landing_zone_top.y), Color.black, 500f);

        cur_walked_distance = 0;
        transform.position = attack_way[0].transform.position;
        cur_way_length = (fly_start_point - fly_end_point).magnitude;

        while (cur_walked_distance < cur_way_length)
        {
            cur_walked_distance += Time.deltaTime * fly_speed;
            transform.position = attack_direction_ray.GetPoint(cur_walked_distance);

            if (num_of_grabbed_enemies >= max_killed_enemies && !move_to_last_point)
            {
                move_to_last_point = true;
                break;
            }

            if (can_land && is_grounded() && groundCheck_1.transform.position.y < p_landing_zone_top.y)            
            {
                break;
            }

            yield return null;
        }

        //----------walk-----------------------------------------

        if (can_land && !move_to_last_point)
        {            
            rb2d.bodyType = RigidbodyType2D.Dynamic;

            anim.SetInteger(walk_fly_anim_param, 1);

            float walk_start_time = Time.time;
            while (Time.time < walk_start_time + walk_time && is_grounded() && !is_sided())
            {
                walk_start_time += Time.deltaTime;
                walk();

                if (num_of_grabbed_enemies >= max_killed_enemies && !move_to_last_point)
                {
                    move_to_last_point = true;
                    break;
                }

                yield return wait_grounded_check;
            }
        }

        //----------fly-----------------------------------------

        rb2d.bodyType = RigidbodyType2D.Kinematic;

        anim.SetInteger(walk_fly_anim_param, 0);

        fly_end_point = transform.position;
        attack_direction_ray.origin = fly_end_point;
        attack_direction_ray.direction = get_direction(fly_end_point, fly_evac_point);

        cur_walked_distance = 0;
        cur_way_length = (fly_end_point - fly_evac_point).magnitude;

        while (cur_walked_distance < cur_way_length)
        {
            cur_walked_distance += Time.deltaTime * fly_speed;
            transform.position = attack_direction_ray.GetPoint(cur_walked_distance);

            yield return null;

        }

        //-----------------------------------------------------




        StartCoroutine(end_attack());
                
    }


}
