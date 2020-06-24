using UnityEngine;
using UnityEngine.UI;
using System;


[Serializable]
public class CharacterData
{
    public string name;
    public string scene_name;
    public string full_name;

    public Sprite saved_npc_sprite;
    public Sprite not_saved_npc_sprite;
    public String name_text_color;

    public HeroHealthSprite health_sprites;

    public Dialog dialog;

    public GameObject prefab;
}

[Serializable]
public class SceneInfo
{
    public string name;
    public string next_scene_name;
    public bool opened;
    public bool completed;
    public bool is_friend_saved;
    public bool is_dna_found;    
    public bool is_boss_level;    
}

//-----------------hero data------------------------
[Serializable]
public class PlayerPoints
{
    public int simple_points_gained_total;
    public int evo_points_gained_total;

    public int simple_points_gained_level;
    public int evo_points_gained_level;

    public bool is_friend_saved_level;
    public bool is_dna_found_level;

    public bool is_friend_saved_total;
    public bool is_dna_found_total;


    public void save_cur_points(SceneInfo scene_inf)
    {
        simple_points_gained_total += simple_points_gained_level;
        evo_points_gained_total += evo_points_gained_level;

        if (!scene_inf.is_friend_saved)
            scene_inf.is_friend_saved = is_friend_saved_level;

        if (!scene_inf.is_dna_found)
            scene_inf.is_dna_found = is_dna_found_level;

        drop_cur_points();
    }

    public void drop_cur_points()
    {
        simple_points_gained_level = 0;
        evo_points_gained_level = 0;

        is_friend_saved_level = false;
        is_dna_found_level = false;        
    }
}

//-------------evo------------------------------
[Serializable]
public class EvoLineSprites
{
    public Sprite line_hor_active;
    public Sprite line_hor_inactive;
    public Sprite line_angle_active;
    public Sprite line_angle_inactive;
}

[Serializable]
public class EvoLine
{
    [HideInInspector]
    public EvoPanelManager evo_pan_man;

    public bool enabled;
    public bool horizontal;
    
    public int item_to;

    public Image line_image;

    public void enable_line()
    {
        enabled = true;
        if (horizontal)
            line_image.sprite = evo_pan_man.evo_lines.line_hor_active;
        else
            line_image.sprite = evo_pan_man.evo_lines.line_angle_active;
    }
}

[Serializable]
public class EvoItem
{
    public string description;
    public float value;
    public int index;
    public int points_to_buy;
    public bool is_bought;
    public bool open_for_bought;    

    public Image item_image;
    public Sprite sprite_bought;
    public Sprite sprite_disabled;
    public Sprite sprite_can_be_bought;

    public EvoLine line_out_1, line_out_2;

    public EvoPanelManager evo_pan_man;

    public void buy()
    {
        is_bought = true;
        update_self();        
    }

    public void update_self()
    {
        line_out_1.evo_pan_man = evo_pan_man;
        line_out_2.evo_pan_man = evo_pan_man;

        if (is_bought)
        {
            item_image.sprite = sprite_bought;
            open_lines();
            open_neighbours_for_buying();
        }
        else if (open_for_bought)
            item_image.sprite = sprite_can_be_bought;
        else
            item_image.sprite = sprite_disabled;
    }

    public void open_lines()
    {
        if (line_out_1.line_image != null)
        {
            line_out_1.enable_line();            
        }
        if (line_out_2.line_image != null)
        {
            line_out_2.enable_line();
        }
    }

    public void open_neighbours_for_buying()
    {
        EvoItem neighbour;
        if (line_out_1.line_image != null)
        {
            neighbour = evo_pan_man.evo_items[line_out_1.item_to - 1];
            if (!neighbour.is_bought)
            {
                neighbour.open_for_bought = true;
                neighbour.item_image.sprite = neighbour.sprite_can_be_bought;
            }
        }
        if (line_out_2.line_image != null)
        {
            neighbour = evo_pan_man.evo_items[line_out_2.item_to - 1];
            if (!neighbour.is_bought)
            {
                neighbour.open_for_bought = true;
                neighbour.item_image.sprite = neighbour.sprite_can_be_bought;
            }
        }        
    }

}


