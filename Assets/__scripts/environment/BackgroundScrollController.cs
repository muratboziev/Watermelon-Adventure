using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ObjectPair
{
    public string name;                         //to be shown in inspector for usability purposes

    public GameObject object1;
    public GameObject object2;
    
    public float hor_speed_koef;                    //speed koeficient of objects to scroll        
    public bool hor_is_const_scroll = false;        //is object requires to be scrolled constantly (clouds)
    [HideInInspector]
    public static float hor_replace_dist;
    [HideInInspector]
    public float hor_speed;

    public float vert_initial_offset;
    public float vert_initial_offset_abs_value;
    public float vert_paralax_delta;    
    public bool vert_has_paralax = true;            //does object has vertical paralax (not used in script)            
    [HideInInspector]
    public float vert_speed;

    [HideInInspector]
    public float width, height;                //width of the GO's to scroll from its collide


    private float cur_delta_x, cur_delta_y;
    private Vector2 new_pos = new Vector2();
    private float dist_to_bg;    
    private float sign;
    bool goes_right, goes_left, bg_other_on_right, change_pos_condition;

    public void move_bg(Rigidbody2D hero_rb2d)
    {

        new_pos = object1.transform.position;
        new_pos.x += pos_X_delta(hero_rb2d.velocity.x);
        new_pos.y += pos_Y_delta(hero_rb2d.velocity.y, hero_rb2d.gameObject.transform.position.y);        
        object1.transform.position = new_pos;

        new_pos = object2.transform.position;
        new_pos.x += pos_X_delta(hero_rb2d.velocity.x);
        new_pos.y += pos_Y_delta(hero_rb2d.velocity.y, hero_rb2d.gameObject.transform.position.y);
        object2.transform.position = new_pos;

        make_pair_bg_offset(object1.transform, object2.transform, hero_rb2d.velocity.x, hero_rb2d.gameObject.transform.position.x);
        make_pair_bg_offset(object2.transform, object1.transform, hero_rb2d.velocity.x, hero_rb2d.gameObject.transform.position.x);
    }

    public float pos_X_delta(float hero_X_speed)
    {
        if (hor_is_const_scroll || hero_X_speed != 0f)
        {
            cur_delta_x = hor_speed;
            if (!hor_is_const_scroll)
            {
                cur_delta_x *= Mathf.Sign(hero_X_speed) * -1;
            }

            return cur_delta_x;
        }

        return 0;
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

    public void make_pair_bg_offset(Transform self_pos, Transform pair_obj_trans, float hero_velocity, float hero_pos)
    {
        dist_to_bg = hero_pos - self_pos.position.x ;

        goes_right = hero_velocity > 0;
        goes_left = hero_velocity < 0;
        bg_other_on_right = hero_pos < pair_obj_trans.position.x;

        //if (name.Contains("grass_near2") && Mathf.Abs(dist_to_bg) < 50)
        //    Debug.Log(name + " goes_right = " + goes_right + " dist_to_bg " + dist_to_bg + " hero_veloity = " + hero_velocity + " bg_other_on_right " + bg_other_on_right);

        change_pos_condition = (Mathf.Abs(hero_velocity) > 0 || hor_is_const_scroll) && 
                                (
                                    (goes_right && !bg_other_on_right) || 
                                    (goes_left && bg_other_on_right) || 
                                    (hor_is_const_scroll && !bg_other_on_right)
                                );       

         
        if (Mathf.Abs(dist_to_bg) <= hor_replace_dist && change_pos_condition )     //если герой пересек фон
        {
            if (hor_is_const_scroll)
                sign = 1;                
            else if (goes_right)
                sign = Mathf.Sign(dist_to_bg) * -1;
            else if (goes_left)
                sign = Mathf.Sign(dist_to_bg);

            new_pos = self_pos.position;
            new_pos.x += width * sign;
            pair_obj_trans.position = new_pos;
        }
    }

    public void initial_go_placing(Vector2 hero_pos)
    {
        new_pos.x = hero_pos.x;
        new_pos.y = hero_pos.y + vert_initial_offset_abs_value;
        object1.transform.position = new_pos;

        new_pos.x = hero_pos.x + width;
        new_pos.y = hero_pos.y + vert_initial_offset_abs_value;
        object2.transform.position = new_pos;
    }
}

[System.Serializable]
public class Background
{
    public string name;
    public GameObject go;
    public ObjectPair[] obj_to_scroll;    
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
    public bool active = false;

    ObjectPair[] obj_to_scroll;              
    
    int scroll_obj_length;                    
    

    public void activate_bg()
    {
        backgrounds[menu_man.cur_act - 1].go.SetActive(true);
        obj_to_scroll = backgrounds[menu_man.cur_act - 1].obj_to_scroll;

        ObjectPair.hor_replace_dist = global_hor_replace_dist;

        for (int i = 0; i < obj_to_scroll.Length; i++)                          //initializing and placing
        {            
            obj_to_scroll[i].width = obj_to_scroll[i].object1.GetComponent<BoxCollider2D>().size.x;
            obj_to_scroll[i].height = obj_to_scroll[i].object1.GetComponent<BoxCollider2D>().size.y;

            obj_to_scroll[i].hor_speed = global_hor_scroll_speed * obj_to_scroll[i].hor_speed_koef;

            obj_to_scroll[i].vert_speed = obj_to_scroll[i].vert_paralax_delta;// * global_hor_scroll_speed / obj_to_scroll[i].width;
            obj_to_scroll[i].vert_initial_offset_abs_value = obj_to_scroll[i].vert_initial_offset * obj_to_scroll[i].height;

            obj_to_scroll[i].initial_go_placing(hero_rb2d.gameObject.transform.position);
        }

        scroll_obj_length = obj_to_scroll.Length;
        active = true;
    }

    public void deactivate_bg(bool deact_go=true)
    {
        active = false;
        if (deact_go)
            foreach (Background bg in backgrounds)
                bg.go.SetActive(false);
    }

    void Update()
    {
        if (active)
        {
            for (int i = 0; i < scroll_obj_length; i++)
            {   
                obj_to_scroll[i].move_bg(hero_rb2d);
            }
        }
    }
    

}