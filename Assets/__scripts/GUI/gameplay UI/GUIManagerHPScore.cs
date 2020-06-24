using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class HeroHealthSprite
{    
    public Sprite protect;
    public Sprite full_hp;
    public Sprite[] health_sprites_3hp;
    public Sprite[] health_sprites_4hp;
    public Sprite dead;
}

[System.Serializable]
public class CurLevelAchievements
{
    public Sprite barel_found_sprite;
    public Sprite barel_not_found_sprite;
    public Sprite [] dna_progress_sprite;
    public Image barel_found_image;
    public Image friend_saved_image;
    public Image dna_progress_image;
    public Text dna_count_text;
    public Text points_gained;    
}

public class GUIManagerHPScore : MonoBehaviour
{
    [Header("Links")]
    public GameManager game_man;        

    [Header("Hero GUI elements")]    
    public Image hero_hp_image;
    public CurLevelAchievements level_achievements;

    int dna_image_len;
    int cur_dna_image_num;    
    int parts_in_dna;
    int dna_image_fill_step;

    //---------------------------------------------------------------------

    private void Start()
    {
        dna_image_len = level_achievements.dna_progress_sprite.Length;
        cur_dna_image_num = 0;
        parts_in_dna = 14;
        dna_image_fill_step = parts_in_dna / dna_image_len;
    }

    public void update_player_hp_sprite(int hero_hp_left, bool took_pill=false)
    {
        HeroHealthSprite cur_hero_hp_sprites = game_man.cur_hero_health_sprite();
        Sprite[] hp_sprites;

        if (game_man.evo_pan_man.evo_items[0].is_bought == true)
        {
            hp_sprites = cur_hero_hp_sprites.health_sprites_4hp;
        }
        else
        {
            hp_sprites = cur_hero_hp_sprites.health_sprites_3hp;
        }

        if (took_pill)
            hero_hp_image.sprite = cur_hero_hp_sprites.protect;        
        else if (hero_hp_left == game_man.hero_contr.recalc_hero_params.max_hero_hp)
        {
            hero_hp_image.sprite = cur_hero_hp_sprites.full_hp;
        }
        else if (hero_hp_left <= 0)
        {
            hero_hp_image.sprite = cur_hero_hp_sprites.dead;
        }
        else
        {
            hero_hp_image.sprite = hp_sprites[hero_hp_left - 1];
        }
    }

    public void update_player_score(bool level_start=false, bool picked_dna_part=false, bool friend_saved=false, bool box_unlocked=false, bool picked_dna=false)
    {
        if (level_start)
        {            
            game_man.player_points.simple_points_gained_level = game_man.player_points.simple_points_gained_total;
            game_man.player_points.evo_points_gained_level = game_man.player_points.evo_points_gained_total;
            cur_dna_image_num = game_man.player_points.simple_points_gained_level / dna_image_fill_step;

            if (game_man.scenes_dict[game_man.cur_scene_name].is_friend_saved)
            {
                level_achievements.friend_saved_image.sprite = game_man.npc_scene_dict[game_man.cur_scene_name].saved_npc_sprite;
            }
            else
            {
                level_achievements.friend_saved_image.sprite = game_man.npc_scene_dict[game_man.cur_scene_name].not_saved_npc_sprite;
            }
        }
        else if (picked_dna_part)
        {
            if (game_man.player_points.simple_points_gained_level > dna_image_fill_step * cur_dna_image_num && cur_dna_image_num + 1 < dna_image_len)
            {   
                cur_dna_image_num++;                
            }

            if (game_man.player_points.simple_points_gained_level > parts_in_dna)
            {
                game_man.player_points.evo_points_gained_level++;
                game_man.player_points.simple_points_gained_level = 0;                
                cur_dna_image_num = 0;                
            }
        }
        else if (friend_saved)
        {
            game_man.score_panel_animator.SetTrigger("friend_saved");
            level_achievements.friend_saved_image.sprite = game_man.npc_scene_dict[game_man.cur_scene_name].saved_npc_sprite;
        }
        else if(picked_dna)
        {
            game_man.score_panel_animator.SetTrigger("dna_gained");
        }

        level_achievements.dna_count_text.text = game_man.player_points.evo_points_gained_level.ToString();
        level_achievements.points_gained.text = game_man.player_points.simple_points_gained_level.ToString();
        level_achievements.dna_progress_image.sprite = level_achievements.dna_progress_sprite[cur_dna_image_num];
    }

}
