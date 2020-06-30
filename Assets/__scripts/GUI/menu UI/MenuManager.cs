using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class MenuManager : MonoBehaviour
{
    [Header("Links")]    
    public GameManager game_man;
    public Animator level_complete_anim;
    public Animator cloud_anim;    
    public Panels uiPanel;
    public EvoPanelManager evo_pan_man;

    public Dictionary<string, Menu_State> menu_state = new Dictionary<string, Menu_State>();
    [HideInInspector]
    public Menu_State cur_menu_state;

    [Header("Options")]
    public OptionsGUI optionsGUI;

    [Header("Images")]
    public GUIImages gui_images;

    [Header("Sprites")]
    public GUISprites gui_sprites;
    Color saved_color = new Color(255,255,0);
    Color not_saved_color = new Color(255, 255, 255);

    [HideInInspector]
    public int cur_act = 0, cur_level = 0;
    string cancel_str = "Cancel";          

    string friend_saved_str = "friend saved: ", friend_not_saved_str = "friend not saved";
    string dna_found_str = "dna point found!", dna_not_found_str = "dna point not found";    

    //---------------------------------------------------------------------------------------------------

    void Awake()
    {
        menu_state["main_menu"] = new Menu_State("main_menu");
        menu_state["game_mode_select"] = new Menu_State("game_mode_select");
        menu_state["evolution"] = new Menu_State("evolution");
        menu_state["buy_evo"] = new Menu_State("buy_evo");
        menu_state["act_select"] = new Menu_State("act_select");
        menu_state["level_select"] = new Menu_State("level_select");
        menu_state["options_mainmenu"] = new Menu_State("options_mainmenu");
        menu_state["options_pause"] = new Menu_State("options_pause");
        menu_state["infinite_run"] = new Menu_State("infinite_run");
        menu_state["pause"] = new Menu_State("pause");
        menu_state["level_completed"] = new Menu_State("level_completed");
        menu_state["gameplay"] = new Menu_State("gameplay");

        Menu_State cur;
        cur = menu_state["main_menu"];
        cur.transition = new MenuTransition[3];
        cur.panel = uiPanel.mainmenuPanel;
        cur.transition[0] = new MenuTransition(cur, menu_state["game_mode_select"], fade:true, load:false);         //gamemodeselect
        cur.transition[1] = new MenuTransition(cur, menu_state["evolution"], fade:true, load:false);                //evolution
        cur.transition[2] = new MenuTransition(cur, menu_state["options_mainmenu"], fade:true, load:false);         //options
        cur.back_transition = null;

        cur = menu_state["game_mode_select"];
        cur.transition = new MenuTransition[3];
        cur.panel = uiPanel.gameModeSelectPanel;
        cur.transition[0] = new MenuTransition(cur, menu_state["main_menu"], fade: true, load: false);              //back
        cur.transition[1] = new MenuTransition(cur, menu_state["act_select"], fade: true, load: false);             //story
        cur.transition[2] = new MenuTransition(cur, menu_state["infinite_run"], fade: true, load: false);           //inf run
        cur.back_transition = cur.transition[0];

        cur = menu_state["evolution"];
        cur.transition = new MenuTransition[2];
        cur.panel = uiPanel.evolutionPanel;
        cur.transition[0] = new MenuTransition(cur, menu_state["main_menu"], fade: true, load: false);               //back
        cur.transition[1] = new MenuTransition(cur, menu_state["buy_evo"], fade: true, load: false);                 //select_evo_item
        cur.back_transition = cur.transition[0];

        cur = menu_state["buy_evo"];
        cur.transition = new MenuTransition[2];
        cur.panel = uiPanel.buyevoPanel;
        cur.transition[0] = new MenuTransition(cur, menu_state["evolution"], fade: true, load: false);               //back
        cur.transition[1] = new MenuTransition(cur, menu_state["evolution"], fade: true, load: false);               //buy
        cur.back_transition = cur.transition[0];

        cur = menu_state["act_select"];
        cur.transition = new MenuTransition[4];
        cur.panel = uiPanel.actSelection;
        cur.transition[0] = new MenuTransition(cur, menu_state["game_mode_select"], fade: true, load: false);          //back
        cur.transition[1] = new MenuTransition(cur, menu_state["level_select"], fade: true, load: false);               //act
        cur.transition[2] = new MenuTransition(cur, menu_state["level_select"], fade: true, load: false);               //act
        cur.transition[3] = new MenuTransition(cur, menu_state["level_select"], fade: true, load: false);               //act
        cur.back_transition = cur.transition[0];

        cur = menu_state["level_select"];
        cur.transition = new MenuTransition[2];
        cur.panel = uiPanel.levelSelection;
        cur.transition[0] = new MenuTransition(cur, menu_state["act_select"], fade: true, load: false);                 //back
        cur.transition[1] = new MenuTransition(cur, menu_state["gameplay"], fade: true, load: true);                    //level        
        cur.back_transition = cur.transition[0];

        cur = menu_state["gameplay"];
        cur.transition = new MenuTransition[3];
        cur.panel = uiPanel.gameplayContoller;
        cur.transition[0] = new MenuTransition(cur, menu_state["pause"], fade: false, load: false);                     //back pressed
        cur.transition[1] = new MenuTransition(cur, menu_state["level_completed"], fade: true, load: false);            //level completed
        cur.transition[2] = new MenuTransition(cur, menu_state["gameplay"], fade: true, load: true);                    //hero died
        cur.back_transition = cur.transition[0];

        cur = menu_state["pause"];
        cur.transition = new MenuTransition[4];
        cur.panel = uiPanel.pausePanel;
        cur.transition[0] = new MenuTransition(cur, menu_state["gameplay"], fade: false, load: false);                  //resume
        cur.transition[1] = new MenuTransition(cur, menu_state["gameplay"], fade: true, load: true);                    //restart
        cur.transition[2] = new MenuTransition(cur, menu_state["options_pause"], fade: false, load: false);             //options
        cur.transition[3] = new MenuTransition(cur, menu_state["main_menu"], fade: true, load: true);                   //quit
        cur.back_transition = cur.transition[0];

        cur = menu_state["options_mainmenu"];
        cur.transition = new MenuTransition[2];
        cur.panel = uiPanel.optionsPanel_mainmenu;
        cur.transition[0] = new MenuTransition(cur, menu_state["main_menu"], fade: true, load: false);
        cur.transition[1] = new MenuTransition(cur, menu_state["main_menu"], fade: true, load: false);
        cur.back_transition = cur.transition[1];

        cur = menu_state["options_pause"];
        cur.transition = new MenuTransition[2];
        cur.panel = uiPanel.optionsPanel_pause;
        cur.transition[0] = new MenuTransition(cur, menu_state["pause"], fade: false, load: false);
        cur.transition[1] = new MenuTransition(cur, menu_state["pause"], fade: false, load: false);
        cur.back_transition = cur.transition[1];

        cur = menu_state["infinite_run"];
        cur.transition = new MenuTransition[2];
        cur.panel = uiPanel.infiniterun;
        cur.transition[0] = new MenuTransition(cur, menu_state["game_mode_select"], fade: true, load: false);      //back                
        cur.transition[1] = new MenuTransition(cur, menu_state["gameplay"], fade: true, load: true);       //infiniterun        
        cur.back_transition = cur.transition[0];

        cur = menu_state["level_completed"];
        cur.transition = new MenuTransition[3];
        cur.panel = uiPanel.levelComplete;
        cur.transition[0] = new MenuTransition(cur, menu_state["main_menu"], fade: true, load: true);       //main menu
        cur.transition[1] = new MenuTransition(cur, menu_state["gameplay"], fade: true, load: true);      //replay              
        cur.transition[2] = new MenuTransition(cur, menu_state["gameplay"], fade: true, load: true);      //next level                
        cur.back_transition = cur.transition[0];

        cur_menu_state = menu_state["main_menu"];

        optionsGUI.slider_sound_mainmenu.value = game_man.sound_level;
        optionsGUI.slider_music_mainmenu.value = game_man.music_level;

        optionsGUI.slider_sound_pause.value = game_man.sound_level;
        optionsGUI.slider_music_pause.value = game_man.music_level;
    }
    
    void Update()
    {
        if (Input.GetButtonDown(cancel_str) && cur_menu_state.back_transition != null)
        {
            string scene_name = "start_menu";                                       //for level complete to main menu only

            if (cur_menu_state.name.Equals("gameplay"))
            {
                Time.timeScale = 0f;
                game_man.bg_scroll_cont.deactivate_bg_scroll(deact_go:false);
                game_man.pause_audiosources();
            }
            else if (cur_menu_state.name.Equals("pause"))
            {                
                Time.timeScale = 1f;
                game_man.bg_scroll_cont.activate_bg_scroll();
                game_man.unpause_audiosources();
            }

            cur_menu_state = make_transition(cur_menu_state.back_transition, scene_name);            
        }
    }

    public void button_handler(int par)
    {        
        bool transition_enabled = true;
        string scene_name = string.Empty;

        if (cur_menu_state.name.Equals("main_menu") && par == 0)     //0 = story
            update_act_selection_panel();
        else if (cur_menu_state.name.Equals("main_menu") && par == 1)     //1 = evolution
            evo_pan_man.update_evolution_panel();
        else if (cur_menu_state.name.Equals("act_select") && par != 0)     //0 = back
        {
            scene_name = game_man.make_level_name(par, 1);
            if (game_man.scenes_dict[scene_name].opened)
            {
                cur_act = par;
                update_level_selection_panel(cur_act);
            }
            else
                transition_enabled = false;
        }
        else if (cur_menu_state.name.Equals("level_select") && par != 0)     //0 = back
        {
            scene_name = game_man.make_level_name(cur_act, par);
            if (game_man.scenes_dict[scene_name].opened)
            {
                cur_level = par;
                par = 1;
                game_man.playable_character = "watermelon";
            }
            else
                transition_enabled = false;
        }
        else if (cur_menu_state.name.Equals("infinite_run"))
        {
            //game_man.selected_character = playable_characters[par - 1];
        }
        else if (cur_menu_state.name.Equals("pause"))
        {
            if (par != 2)                                      //if not to options panel or main menu
            {
                Time.timeScale = 1;
                game_man.bg_scroll_cont.bg_scroll_is_active = true;
            }

            if (par == 1)                                                   //1 = restart
            { 
                scene_name = game_man.make_level_name(cur_act, cur_level);

                game_man.stop_enemies();

                if (game_man.dialog_man.having_dialog)
                    game_man.dialog_man.end_dialog();
            }
            if (par == 3)                                                   //3 = to main menu
            {
                scene_name = "start_menu";

                game_man.stop_enemies();

                if (game_man.dialog_man.having_dialog)
                    game_man.dialog_man.end_dialog();
            }
        }
        else if (cur_menu_state.name.Equals("level_completed"))
        {            
            if (par == 0)                                                        //to main menu
            {
                scene_name = "start_menu";                              
                game_man.player_points.save_cur_points(game_man.scenes_dict[game_man.cur_scene_name]);
            }
            else if (par == 1)                                                   //replay                
                scene_name = game_man.make_level_name(cur_act, cur_level);
            else if (par == 2)                                              //next_level
            {                
                game_man.player_points.save_cur_points(game_man.scenes_dict[game_man.cur_scene_name]);
                if (cur_level + 1 > 11)
                {
                    if (cur_act <= 3)
                    {
                        cur_level = 1;
                        cur_act += 1;
                    }
                    //else;
                    //endgame
                }
                else
                {
                    cur_level++;
                }
                scene_name = game_man.make_level_name(cur_act, cur_level);
            }
        }

        if (transition_enabled)
        {
            cur_menu_state = make_transition(cur_menu_state.transition[par], scene_name);
            game_man.audio_man.play_menu_transition_enabled();
        }
        else
        {
            game_man.audio_man.play_menu_transition_disabled();
        }
    }

    public void options_panel_button_handler(int par)
    {
        int sound, music;
        if (par == 0)                           //ok
        {
            if (cur_menu_state.name.Equals("options_mainmenu"))
            {
                music = (int)optionsGUI.slider_music_mainmenu.value;
                sound = (int)optionsGUI.slider_sound_mainmenu.value;
                optionsGUI.slider_music_pause.value = music;
                optionsGUI.slider_sound_pause.value = sound;
            }
            else
            {
                music = (int)optionsGUI.slider_music_pause.value;
                sound = (int)optionsGUI.slider_sound_pause.value;
                optionsGUI.slider_music_mainmenu.value = music;
                optionsGUI.slider_sound_mainmenu.value = sound;
            }
            game_man.music_level = music;
            game_man.sound_level = sound;
        }
        else                                    //cancel
        {
            music = game_man.music_level;
            sound = game_man.sound_level;

            optionsGUI.slider_music_pause.value = music;
            optionsGUI.slider_sound_pause.value = sound;

            optionsGUI.slider_music_mainmenu.value = music;
            optionsGUI.slider_sound_mainmenu.value = sound;
         }
    }

    public void mainmenu_quit()
    {
        #if UNITY_STANDALONE
		    Application.Quit();
        #endif

        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

    //level complete ----------------------------------------------------------------------------------------------------------

    public void level_complete(bool friend_saved, bool dna_found, CharacterData friend_data)
    {
        gui_images.friend_image.sprite = friend_data.not_saved_npc_sprite;
        gui_images.dna_image.sprite = gui_sprites.dna_not_found_sprite;
        gui_images.star_image.sprite = gui_sprites.star_not_gained;

        gui_images.level_complete_text.gameObject.GetComponent<CanvasGroup>().alpha = 0f;
        gui_images.friend_saved_text.gameObject.GetComponent<CanvasGroup>().alpha = 0f;
        gui_images.friend_name_text.gameObject.GetComponent<CanvasGroup>().alpha = 0f;
        gui_images.dna_text.gameObject.GetComponent<CanvasGroup>().alpha = 0f;

        if (friend_saved)
        {
            gui_images.friend_name_text.color = game_man.preset_colors[friend_data.name_text_color];

            gui_images.friend_saved_text.text = friend_saved_str;
            gui_images.friend_saved_text.color = saved_color;            
        }
        else
        {
            gui_images.friend_saved_text.text = friend_not_saved_str;
            gui_images.friend_saved_text.color = not_saved_color;            
        }

        if (dna_found)
        {
            gui_images.dna_text.text = dna_found_str;
            gui_images.dna_text.color = saved_color;            
        }
        else
        {
            gui_images.dna_text.text = dna_not_found_str;
            gui_images.dna_text.color = not_saved_color;
        }

        StartCoroutine(level_complete_animation(friend_saved, dna_found, friend_data));
    }

    public IEnumerator level_complete_animation(bool friend_saved, bool dna_found, CharacterData friend_data)
    {
        cur_menu_state = make_transition(cur_menu_state.transition[1]);                  //show level completion panel                

        level_complete_anim.SetBool("level_complete", true);
        level_complete_anim.SetBool("friend_saved", friend_saved);
        level_complete_anim.SetBool("dna_found", dna_found);

        while (level_complete_anim.GetCurrentAnimatorStateInfo(0).IsName("start"))              //ждем окончания анимации start
            yield return true;

        game_man.audio_man.play_level_complete();

        while (level_complete_anim.GetCurrentAnimatorStateInfo(0).IsName("level_complete"))     //ждем окончания анимации level_compete
            yield return true;        

        if (friend_saved)
        {            
            game_man.audio_man.play_achievement_friend();

            yield return new WaitForSeconds(0.2f);                                              //ждем примерно середины анимации friend_saved
            gui_images.friend_image.sprite = friend_data.saved_npc_sprite;
            gui_images.friend_name_text.text = friend_data.full_name;

            while (level_complete_anim.GetCurrentAnimatorStateInfo(0).IsName("friend_saved"))   //ждем окончания анимации friend_saved
                yield return true;
        }

        if (dna_found)
        {
            while (!level_complete_anim.GetCurrentAnimatorStateInfo(0).IsName("dna_found"))     //ждем начала анимации dna_found (если не был спасен друг)
                yield return true;

            game_man.audio_man.play_achievement_dna();
        }
        
        level_complete_anim.SetBool("level_complete", false);
    }

    //-----------------------------------------------------------------------------------------------------------------------

    void update_act_selection_panel()
    {
        string scene_name;
        for (int i = 0; i < gui_images.act_image.Length; i++)
        {
            scene_name = game_man.make_level_name(i+1, 1);
            if (game_man.scenes_dict[scene_name].opened)
            {
                gui_images.act_image[i].sprite = gui_sprites.act_opened_sprite[i];
            }
            else
            {
                gui_images.act_image[i].sprite = gui_sprites.act_closed_sprite;
            }
        }
    }

    void update_level_selection_panel(int act)
    {
        SceneInfo cur_scene;        
        string scene_name;

        gui_images.act_text.sprite = gui_sprites.actText[act-1];
        for (int i = 0; i < gui_images.level_selection_image.Length; i++)
        {
            scene_name = game_man.make_level_name(act, i+1);
            
            cur_scene = game_man.scenes_dict[scene_name];
            if (cur_scene.opened)
            {
                gui_images.level_selection_image[i].level_image.sprite = gui_sprites.level_opened_sprite;
                if (gui_images.level_selection_image[i].achieve_panel != null)       //if not boss level
                {
                    gui_images.level_selection_image[i].achieve_panel.SetActive(true);

                    if (cur_scene.completed)
                        gui_images.level_selection_image[i].achievments[0].sprite = gui_sprites.star_gained;
                    else
                        gui_images.level_selection_image[i].achievments[0].sprite = gui_sprites.star_not_gained;

                    if (cur_scene.is_friend_saved)
                        gui_images.level_selection_image[i].achievments[1].sprite = gui_sprites.ally_saved_sprite;
                    else
                        gui_images.level_selection_image[i].achievments[1].sprite = gui_sprites.ally_not_saved_sprite;

                    if (cur_scene.is_dna_found)
                        gui_images.level_selection_image[i].achievments[2].sprite = gui_sprites.dna_found_sprite;
                    else
                        gui_images.level_selection_image[i].achievments[2].sprite = gui_sprites.dna_not_found_sprite;

                    //scene_par = cur_scene.is_dna_found;
                    //gui_images.level_selection_image[i].stars[2].sprite = scene_par ? gui_sprites.star_opened_sprite : gui_sprites.star_closed_sprite;
                }                
            }
            else
            {
                gui_images.level_selection_image[i].level_image.sprite = gui_sprites.level_closed_sprite;
                if (gui_images.level_selection_image[i].achieve_panel != null)
                    gui_images.level_selection_image[i].achieve_panel.SetActive(false);
            }
        }
    }

    //-----------------------------------------------------------------------------------------------------------------------
    public Menu_State make_transition(MenuTransition mt, string scene_name="")
    {
        StartCoroutine(menu_transition(mt, scene_name));
        return mt.state_to;
    }


    public IEnumerator menu_transition(MenuTransition mt, string scene_name="")
    {
        Menu_State state_from = mt.state_from;
        Menu_State state_to = mt.state_to;
        bool with_fading = mt.with_fading;
        bool with_scene_loading = mt.with_scene_loading;

        WaitForSeconds loadTime = new WaitForSeconds(1.2f);   //1

        float elapsedTime = 0;
        float fade_duration = 0.35f;                        
        float fade_to = 0f, fade_from = 0f;

        uiPanel.clickBlocker.gameobj.SetActive(true);

        //-------------------------------------------------
        with_fading = false;

        if (state_to.name == "buy_evo" || state_from.name == "buy_evo")
        {
            fade_to = 0.65f;
            fade_from = 0.65f;
            fade_duration = 0.25f;
        }

        if (with_fading)
        {            
            while (elapsedTime < fade_duration)
            {
                elapsedTime += Time.deltaTime;
                state_from.panel.canvas.alpha = Mathf.Lerp(1, fade_to, elapsedTime / fade_duration);
                yield return null;
            }
        }
        else
        {
            state_from.panel.canvas.alpha = 1;
        }

        if (state_to.name != "buy_evo")
        {
            state_from.panel.gameobj.SetActive(false);
        }

        //-------------------------------------------------

        if (with_scene_loading)
        {
            elapsedTime = 0;
            uiPanel.loadingPanel.gameobj.SetActive(true);
            
            while (elapsedTime < fade_duration)
            {
                elapsedTime += Time.deltaTime;
                uiPanel.loadingPanel.canvas.alpha = Mathf.Lerp(0, 1, elapsedTime / fade_duration);
                yield return null;
            }
            
            uiPanel.backgroundPanel.gameobj.SetActive(scene_name == "start_menu");

            yield return loadTime;

            SceneManager.LoadScene(scene_name);

            elapsedTime = 0;

            while(!SceneLoadHandler.scene_load_complete)
            {
                yield return null;
            }

            while (elapsedTime < fade_duration)
            {
                elapsedTime += Time.deltaTime;
                uiPanel.loadingPanel.canvas.alpha = Mathf.Lerp(1, 0, elapsedTime / fade_duration);
                yield return null;
            }

            uiPanel.loadingPanel.gameobj.SetActive(false);

        }

        //-------------------------------------------------

        elapsedTime = 0;
        state_to.panel.gameobj.SetActive(true);        
        if (with_fading)
        {
            elapsedTime = 0;
            while (elapsedTime < fade_duration)
            {
                elapsedTime += Time.deltaTime;
                state_to.panel.canvas.alpha = Mathf.Lerp(fade_from, 1, elapsedTime / fade_duration);
                yield return null;
            }
        }
        else
        {
            state_to.panel.canvas.alpha = 1;
        }

        //-------------------------------------------------

        uiPanel.clickBlocker.gameobj.SetActive(false);
    }


}


