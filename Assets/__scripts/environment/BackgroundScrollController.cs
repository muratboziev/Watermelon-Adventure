using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BgScrollObjectPair
{
    public string name;                         //to be shown in inspector for usability purposes

    public GameObject object1;
    public GameObject object2;
    
    public float hor_speed_koef;                    //speed koeficient of objects to scroll        
    public bool hor_is_const_scroll = false;        //is object requires to be scrolled constantly (clouds)  

    public bool vert_immutable;
    public float vert_initial_offset;    
    public float vert_paralax_delta;    
    public bool vert_has_paralax = true;            //does object has vertical paralax (not used in script)  

    private float hor_speed, vert_speed;
    private float go_width;

    private float cur_delta_x, cur_delta_y;
    private Vector2 new_pos = new Vector2();


    public void move_bg_object(Rigidbody2D hero_rb2d, float hor_replace_dist)
    {
        new_pos = object1.transform.position;
        new_pos.x += pos_X_delta(hero_rb2d.velocity.x);
        object1.transform.position = new_pos;

        new_pos = object2.transform.position;
        new_pos.x += pos_X_delta(hero_rb2d.velocity.x);
        object2.transform.position = new_pos;

        make_pair_bg_offset(object1.transform, object2.transform, hero_rb2d.velocity.x, hero_rb2d.gameObject.transform.position.x, hor_replace_dist);        

        //new_pos.y += pos_Y_delta(hero_rb2d.velocity.y, hero_rb2d.gameObject.transform.position.y);
    }

    public float pos_X_delta(float hero_X_speed)
    {
        cur_delta_x = 0;

        if (hor_is_const_scroll)
        {
            cur_delta_x = hor_speed;
        }
        else if (hero_X_speed != 0f)
        {
            cur_delta_x = hor_speed * Mathf.Sign(hero_X_speed) * -1;
        }

        return cur_delta_x;
    }

    public float pos_Y_delta(float hero_Y_speed, float hero_Y_pos)
    {
        if (vert_has_paralax && hero_Y_speed != 0f)
        {
            cur_delta_y = hero_Y_speed * vert_speed / 100;

            //if (name.Contains("grass near"))
                //Debug.Log(name + " " + hero_Y_speed / 1000);

            return cur_delta_y;
        }

        return 0;        
    }

    public void make_pair_bg_offset(Transform bg_1, Transform bg_2, float hero_velocity, float hero_pos_x, float hor_replace_dist)
    {
        if (hero_velocity != 0 || hor_is_const_scroll)
        {            
            float dist_to_bg1, dist_to_bg2;

            Transform near_bg, far_bg;
            float dist_to_near_bg;

            dist_to_bg1 = Mathf.Abs(hero_pos_x - bg_1.position.x);
            dist_to_bg2 = Mathf.Abs(hero_pos_x - bg_2.position.x);

            if (dist_to_bg1 < dist_to_bg2)
            {
                near_bg = bg_1;
                far_bg = bg_2;
                dist_to_near_bg = dist_to_bg1;
            }
            else
            {
                near_bg = bg_2;
                far_bg = bg_1;
                dist_to_near_bg = dist_to_bg2;
            }

            if (dist_to_near_bg <= hor_replace_dist)
            {
                

                if ((hero_velocity > 0 && hero_pos_x > far_bg.position.x) ||                                     //hero goes right and far background is on left                    
                    (hor_is_const_scroll && hero_velocity >= 0 && hero_pos_x > far_bg.position.x))               //constant scrolling (always from right to left) and hero goes right (or stood) and far bg is on left
                {
                    new_pos = near_bg.position;
                    if (name == "clouds near")
                        Debug.Log("right " + new_pos.x + " " + (new_pos.x + go_width) + " vel= " + hero_velocity);
                    new_pos.x += go_width;
                    far_bg.position = new_pos;
                }
                else
                if ((hero_velocity < 0 && hero_pos_x < far_bg.position.x) ||                                     //hero goes left and far background is on right
                    (hor_is_const_scroll && hero_velocity < 0 && hero_pos_x < far_bg.position.x))                    //constant scrolling (always from right to left) and hero goes left (or stood) and far bg is on right                       

                {
                    new_pos = near_bg.position;
                    if (name == "clouds near")
                        Debug.Log("left " + new_pos.x + " " + (new_pos.x - go_width) + " vel= " + hero_velocity);
                    new_pos.x -= go_width;
                    far_bg.position = new_pos;
                }          

            }
            
        }
    }

    public void initial_go_placing(Vector2 hero_pos, float global_hor_scroll_speed)
    {
        float width, height;                //width of the GO's to scroll from its collide 

        if (name == "clouds near")
        {
            Debug.Log(1);
        }

        width = object1.GetComponent<BoxCollider2D>().size.x;
        height = object1.GetComponent<BoxCollider2D>().size.y;
        go_width = width;

        hor_speed = global_hor_scroll_speed * hor_speed_koef;
        vert_speed = vert_paralax_delta;// * global_hor_scroll_speed / obj_to_scroll[i].width;            

        new_pos.x = hero_pos.x;
        new_pos.y = hero_pos.y + vert_initial_offset * height;
        object1.transform.position = new_pos;

        new_pos.x = hero_pos.x + width;
        new_pos.y = hero_pos.y + vert_initial_offset * height;
        object2.transform.position = new_pos;
    }
}

[System.Serializable]
public class Background
{
    public string name;
    public GameObject go;
    public BgScrollObjectPair[] obj_to_scroll;    
}

public class BackgroundScrollController : MonoBehaviour
{
    [Header("Links")]
    public MenuManager menu_man;
    public Rigidbody2D hero_rb2d;

    [Header("Scroll params")]
    public float global_hor_scroll_speed;             
    public float global_hor_replace_dist = 20;            

    [Header("Scroll objects")]
    public Background [] backgrounds;

    [HideInInspector]
    public bool bg_scroll_is_active = false;

    private BgScrollObjectPair[] obj_to_scroll;    
    int scroll_obj_length;                    
    

    public void initialize_bg_scroll()
    {
        backgrounds[menu_man.cur_act - 1].go.SetActive(true);

        obj_to_scroll = backgrounds[menu_man.cur_act - 1].obj_to_scroll;
        scroll_obj_length = obj_to_scroll.Length;        

        for (int i = 0; i < scroll_obj_length; i++)                          //initializing and placing
        {
            obj_to_scroll[i].initial_go_placing(hero_rb2d.gameObject.transform.position, global_hor_scroll_speed);
        }        
    }

    void Update()
    {
        if (bg_scroll_is_active)
        {
            for (int i = 0; i < scroll_obj_length; i++)
            {
                obj_to_scroll[i].move_bg_object(hero_rb2d, global_hor_replace_dist);
            }
        }
    }

    public void activate_bg_scroll()
    {
        bg_scroll_is_active = true;
    }

    public void deactivate_bg_scroll(bool deact_go=true)
    {
        bg_scroll_is_active = false;
        if (deact_go)
            foreach (Background bg in backgrounds)
                bg.go.SetActive(false);
    }

  

}