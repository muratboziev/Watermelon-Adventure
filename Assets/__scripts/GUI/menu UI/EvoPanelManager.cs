using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EvoPanelManager : MonoBehaviour
{
    [Header("Links")]
    public GameManager game_man;
    public MenuManager menu_man;
    public Animator evo_pan_anim;

    [Header("Evo panel pars")]    
    public Text evo_points_earned;
    public string text_no_money;
    public string text_not_unlocked;
    public string text_bought;

    [Header("Evo item info")]
    public Image evo_item_image;
    public Text evo_item_description;    
    public Text evo_points_required;    
    public Image buy_button;
    public Text warning_text;    
    
    public EvoLineSprites evo_lines;

    [Header("Evo item pars")]
    public EvoItem [] evo_items;
    public Dictionary<int, EvoItem> evo_tree = new Dictionary<int, EvoItem>();

    private int cur_selected_item;
    private int anim_to_play = 1;

    Color buy_button_enabled_color = Color.green;
    Color buy_button_disabled_color = Color.gray;

    void Start()
    {
        foreach (EvoItem ei in evo_items)
        {
            evo_tree[ei.index] = ei;
        }
    }

    public void evo_item_press(int par)
    {
        EvoItem selected_item = evo_tree[par];  

        if (!selected_item.is_bought)
        {
            if (!selected_item.open_for_bought)
            {
                warning_text.color = Color.red;
                warning_text.text = text_not_unlocked;
                buy_button.color = buy_button_disabled_color;                    
            }
            else if (selected_item.points_to_buy > game_man.player_points.evo_points_gained_total)
            {
                warning_text.color = Color.red;
                warning_text.text = text_no_money;
                buy_button.color = buy_button_disabled_color;                    
            }
            else
            {
                warning_text.text = string.Empty;
                buy_button.color = buy_button_enabled_color;
            }                
        }
        else
        {
            buy_button.color = buy_button_disabled_color;
            warning_text.text = text_bought;
            warning_text.color = Color.green;
        }

        cur_selected_item = par;

        evo_item_image.sprite = selected_item.sprite_bought;
        evo_item_description.text = selected_item.description;
        evo_points_required.text = selected_item.points_to_buy.ToString();

        menu_man.cur_menu_state = menu_man.make_transition(menu_man.cur_menu_state.transition[1]);

        game_man.audio_man.play_menu_transition_enabled();
    }

    public void buy(int par)
    {
        EvoItem selected_item = evo_tree[cur_selected_item];        

        if(!selected_item.is_bought && selected_item.open_for_bought && selected_item.points_to_buy <= game_man.player_points.evo_points_gained_total)
        {
            selected_item.buy();

            game_man.player_points.evo_points_gained_total -= selected_item.points_to_buy;
            evo_points_earned.text = game_man.player_points.evo_points_gained_total.ToString();

            menu_man.cur_menu_state = menu_man.make_transition(menu_man.cur_menu_state.transition[1]);            
            StartCoroutine(perk_was_bought());

            game_man.audio_man.play_evo_bought();
        }
        else
        {
            game_man.audio_man.play_menu_transition_disabled();
        }
    }

    public void cancel_buy(int par)
    {
        menu_man.cur_menu_state = menu_man.make_transition(menu_man.cur_menu_state.transition[0]);

        game_man.audio_man.play_menu_transition_enabled();
    }

    public void update_evolution_panel()
    {
        evo_points_earned.text = game_man.player_points.evo_points_gained_total.ToString();

        foreach (EvoItem ei in evo_items)
        {            
            ei.update_self();
        }
    }

    public void animate_hero(int par)
    {
        anim_to_play = anim_to_play == 1 ? ++anim_to_play : 1;

        if (anim_to_play == 1)
        {            
            evo_pan_anim.Play("breath");            
        }
        else
        {            
            evo_pan_anim.Play("turn");
        }
    }

    IEnumerator perk_was_bought()
    {   
        while (menu_man.uiPanel.evolutionPanel.canvas.alpha < 0.75f)
            yield return new WaitForSeconds(0.05f);
        
        evo_pan_anim.SetInteger("perk_bought", cur_selected_item);

        yield return new WaitForSeconds(0.5f);
        evo_pan_anim.SetInteger("perk_bought", 0);
    }



}
