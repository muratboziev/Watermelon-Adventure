using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[System.Serializable]
public class BgScrollObjectPair
{
    public string name;                         //to be shown in inspector for usability purposes

    public GameObject object1;
    public GameObject object2;
    
    public float hor_speed_koef;                    //speed koeficient of objects to scroll        
    public bool hor_is_const_scroll = false;        //is object requires to be scrolled constantly (clouds)  

    public bool vert_has_paralax = true;            //does bg has vertical paralax and follows hero with delay (hills, mountains)
    public bool vert_const_offset = false;            //does bg is always on same position relative to hero (sky)  (if none is true no change of position when player moves vertically will happen (clouds))
    public float vert_relative_offset_from_player;    
    public float vert_paralax;
    
    private float go_width;

    private float cur_delta_x, cur_delta_y;        
    private float abs_offset_ver;              //абсолютное значение расстояние на которое центр фона должен отстоять от ГГ
    private Vector2 new_pos = new Vector2(), new_pos2 = new Vector2();
    private float vert_total_delta = 0;
    private float exceed_delta = 0;
    private bool vert_offset_exceeded = false;
    

    public void move_bg_object_hor(Rigidbody2D hero_rb2d, float hor_replace_dist)
    {
        new_pos = object1.transform.position;
        new_pos2 = object2.transform.position;

        new_pos.x += pos_X_delta(hero_rb2d.velocity.x);
        new_pos2.x += pos_X_delta(hero_rb2d.velocity.x);

        object1.transform.position = new_pos;
        object2.transform.position = new_pos2;

        make_pair_bg_offset(object1.transform, object2.transform, hero_rb2d.velocity.x, hero_rb2d.gameObject.transform.position.x, hor_replace_dist);        
    }

    public void move_bg_object_ver(Rigidbody2D hero_rb2d)
    {
        new_pos = object1.transform.position;       //clouds
        new_pos2 = object2.transform.position;

        if (vert_has_paralax)                       //hills moutains
        {
            new_pos.y = pos_Y_delta(hero_rb2d.velocity.y, hero_rb2d.gameObject.transform.position.y, new_pos.y);
            new_pos2.y = pos_Y_delta(hero_rb2d.velocity.y, hero_rb2d.gameObject.transform.position.y, new_pos2.y);
            
        }
        else if (vert_const_offset)                 //sky
        {
            new_pos.y = hero_rb2d.transform.position.y + abs_offset_ver;
            new_pos2.y = hero_rb2d.transform.position.y + abs_offset_ver;
        }

        object1.transform.position = new_pos;
        object2.transform.position = new_pos2;
    }

    public float pos_X_delta(float hero_X_speed)
    {
        cur_delta_x = 0;

        if (hor_is_const_scroll)
        {
            cur_delta_x = hor_speed_koef;
        }
        else if (hero_X_speed != 0f)
        {
            cur_delta_x = hor_speed_koef * Mathf.Sign(hero_X_speed) * -1;
        }

        return cur_delta_x;
    }

    public float pos_Y_delta(float hero_Y_speed, float hero_y_pos, float bg_y_pos)
    {

        cur_delta_y = hero_Y_speed * vert_paralax / 100;        

        vert_total_delta += cur_delta_y;
                

        if (Mathf.Abs(vert_total_delta) > Mathf.Abs(abs_offset_ver) + 30)
        {
            if (!vert_offset_exceeded)
            {
                vert_offset_exceeded = true;
                exceed_delta = hero_y_pos - bg_y_pos;
                return bg_y_pos;
            }

            return bg_y_pos;// hero_y_pos - exceed_delta;
        }

        vert_offset_exceeded = false;
        return bg_y_pos + cur_delta_y;
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
                    //if (name == "clouds near")
                    //    Debug.Log("right " + new_pos.x + " " + (new_pos.x + go_width) + " vel= " + hero_velocity);
                    new_pos.x += go_width;
                    far_bg.position = new_pos;
                }
                else
                if ((hero_velocity < 0 && hero_pos_x < far_bg.position.x) ||                                     //hero goes left and far background is on right
                    (hor_is_const_scroll && hero_velocity < 0 && hero_pos_x < far_bg.position.x))                    //constant scrolling (always from right to left) and hero goes left (or stood) and far bg is on right                       

                {
                    new_pos = near_bg.position;                    
                    new_pos.x -= go_width;
                    far_bg.position = new_pos;
                }          

            }
            
        }
    }

    public void initial_go_placing(Vector2 hero_pos, float global_hor_scroll_speed)
    {
        float width, height;                //width of the GO's to scroll from its collide 

        width = object1.GetComponent<BoxCollider2D>().size.x;
        height = object1.GetComponent<BoxCollider2D>().size.y;
        go_width = width;

        hor_speed_koef *= global_hor_scroll_speed;
        
        abs_offset_ver = vert_relative_offset_from_player * height;

        new_pos.x = hero_pos.x;
        new_pos.y = hero_pos.y + abs_offset_ver;
        object1.transform.position = new_pos;

        new_pos.x = hero_pos.x + width;
        new_pos.y = hero_pos.y + abs_offset_ver;
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

    List<BgScrollObjectPair> hor_bg_const_scroll = new List<BgScrollObjectPair>();
    List<BgScrollObjectPair> hor_bg = new List<BgScrollObjectPair>();

    void Update()
    {
        if (bg_scroll_is_active)
        {
            foreach (BgScrollObjectPair bg in hor_bg_const_scroll)
            {
                bg.move_bg_object_hor(hero_rb2d, global_hor_replace_dist);
            }

            if (hero_rb2d.velocity.x != 0)                                      //если герой движется, передвигаем сразу все фоны 
            {
                foreach (BgScrollObjectPair bg in hor_bg)
                {
                    bg.move_bg_object_hor(hero_rb2d, global_hor_replace_dist);

                }
            }

            if (hero_rb2d.velocity.y != 0)
            {
                foreach (BgScrollObjectPair bg in hor_bg)
                {
                    bg.move_bg_object_ver(hero_rb2d);
                }

                foreach (BgScrollObjectPair bg in hor_bg_const_scroll)
                {
                    bg.move_bg_object_ver(hero_rb2d);
                }
            }
        }
    }

    public void initialize_bg_scroll()
    {   
        int cur_act = menu_man.cur_act - 1;

        backgrounds[cur_act].go.SetActive(true);

        foreach (BgScrollObjectPair bg in backgrounds[cur_act].obj_to_scroll)                          //initializing and placing
        {            
            bg.initial_go_placing(hero_rb2d.gameObject.transform.position, global_hor_scroll_speed);

            if (bg.hor_is_const_scroll)
                hor_bg_const_scroll.Add(bg);
            else
                hor_bg.Add(bg);
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