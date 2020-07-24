using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using Cinemachine;


public class RequiredLinks
{
    public GameManager game_man;
    public GUIManagerGameplay gameplay_gui_man;
    public GUIManagerHPScore gui_man_hp_score;    
    public WeaponManager weap_man;
    public HeroController hero_cont;
    public GameProgressSaver game_progr;
    public EvoPanelManager evo_pan_man;
    public MenuManager menu_man;
    public EventSystem event_sys;
    public CinemachineVirtualCamera cinemachine_camera;
    public CinemachineConfiner cinemach_confiner;
    public AllySpiderController ally_spider;
    public AllyDragonflyController ally_dragonfly;
    public AllyPrayingMantisController ally_praying_mantis;
    public LevelCompleteDetection level_compl_detect;
    public GameObject UI_gameobject;
    public NPCController npc_contr;    
    public NpcBoxController box_contr;    
    public BackgroundScrollController bg_scroll_cont;
    public DialogManager dialog_man;
    public PickItem pick_item;
    public PickableItemSpawnManager pickable_manager;
    public AudioManager audio_man;    
    public EnterExitDungeon enter_exit_dungeon;
}

public class SceneLoadHandler : MonoBehaviour
{
    
    public static GameManager game_man;
    static RequiredLinks object_to_link = new RequiredLinks();
    public static bool scene_load_complete;

    // Any static function tagged with RuntimeInitializeOnLoadMethod will be called only a single time when the game load
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static public void InitSceneCallback()                                              //called after awake(enabled?)
    {
        SceneManager.sceneUnloaded += scene_unload_started;

        SceneManager.sceneLoaded += instantiate_hero_and_allies;
        SceneManager.sceneLoaded += establish_links;
        SceneManager.sceneLoaded += align_weapons_characteristics;        
        SceneManager.sceneLoaded += align_hero_characteristics;
        SceneManager.sceneLoaded += update_gameplay;
        SceneManager.sceneLoaded += play_music;

        SceneManager.sceneLoaded += start_stop_cloud_animation;
        SceneManager.sceneLoaded += drop_singlton_counter;
        SceneManager.sceneLoaded += check_event_system;

        SceneManager.sceneLoaded += run_enemies;

        SceneManager.sceneLoaded += scene_load_completed;
    }

    public static void scene_unload_started(Scene scene)
    {
        scene_load_complete = false;        
    }

    public static void scene_load_completed(Scene scene, LoadSceneMode mode)
    {        
        scene_load_complete = true;

        /*if (object_to_link.game_man.load_main != true && scene.name == "start_menu")
        {
            string sc_to_load = object_to_link.game_man.scene_to_load;
            int act = (int)char.GetNumericValue(sc_to_load[3]); 
            int level = (int)char.GetNumericValue(sc_to_load[10]);

            object_to_link.game_man.playable_character = "watermelon";

            object_to_link.menu_man.cur_act = act;
            object_to_link.menu_man.cur_level = level;
            object_to_link.menu_man.cur_menu_state = object_to_link.menu_man.make_transition(object_to_link.menu_man.cur_menu_state.transition[0]);
            object_to_link.menu_man.cur_menu_state = object_to_link.menu_man.make_transition(object_to_link.menu_man.cur_menu_state.transition[1]);
            object_to_link.menu_man.cur_menu_state = object_to_link.menu_man.make_transition(object_to_link.menu_man.cur_menu_state.transition[act]);
            object_to_link.menu_man.cur_menu_state = object_to_link.menu_man.make_transition(object_to_link.menu_man.cur_menu_state.transition[1], sc_to_load);
        }*/
    }

    public static void instantiate_hero_and_allies(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != "start_menu")
        {
            game_man = FindObjectOfType<GameManager>() as GameManager; 

            //inst hero
            GameObject hero_to_instantiate = game_man.npc_name_dict[game_man.playable_character].prefab;
            GameObject hero_start_position = GameObject.FindGameObjectsWithTag("level_start_zone")[0];
            GameObject instantiated_hero = Instantiate(hero_to_instantiate);

            HeroController hero_cont = instantiated_hero.GetComponent<HeroController>();

            instantiated_hero.transform.parent = null;
            instantiated_hero.transform.position = new Vector3(hero_start_position.transform.position.x, hero_start_position.transform.position.y, 0);

            if (!hero_cont.facingRight)
                hero_cont.Flip();
            
            object_to_link.menu_man.uiPanel.clickBlocker.gameobj.SetActive(false);

            //inst allies
            GameObject instantiated_ally = Instantiate(game_man.npc_name_dict["ally_spider"].prefab);
            instantiated_ally.transform.parent = null;

            instantiated_ally = Instantiate(game_man.npc_name_dict["ally_dragonfly"].prefab);
            instantiated_ally.transform.parent = null;

            instantiated_ally = Instantiate(game_man.npc_name_dict["ally_praying_mantis"].prefab);
            instantiated_ally.transform.parent = null;
        }
        else
        {
            
        }
    }

    public static void establish_links(Scene scene, LoadSceneMode mode)
    {        
        object_to_link.game_man = FindObjectOfType<GameManager>() as GameManager;
        object_to_link.menu_man = FindObjectOfType<MenuManager>() as MenuManager;
        object_to_link.event_sys = FindObjectOfType<EventSystem>() as EventSystem;
        object_to_link.cinemachine_camera = FindObjectOfType<CinemachineVirtualCamera>() as CinemachineVirtualCamera;
        object_to_link.cinemach_confiner = FindObjectOfType<CinemachineConfiner>() as CinemachineConfiner;
        object_to_link.evo_pan_man = FindObjectOfType<EvoPanelManager>() as EvoPanelManager;
        object_to_link.game_progr = FindObjectOfType<GameProgressSaver>() as GameProgressSaver;
        object_to_link.dialog_man = FindObjectOfType<DialogManager>() as DialogManager;
        object_to_link.audio_man = FindObjectOfType<AudioManager>() as AudioManager;        

        if (scene.name != "start_menu")
        {
            //находим объекты которые/которыми надо линковать
            object_to_link.hero_cont = FindObjectOfType<HeroController>() as HeroController;
            object_to_link.weap_man = FindObjectOfType<WeaponManager>() as WeaponManager;                        
            object_to_link.gameplay_gui_man = FindObjectOfType<GUIManagerGameplay>() as GUIManagerGameplay;
            object_to_link.gui_man_hp_score = FindObjectOfType<GUIManagerHPScore>() as GUIManagerHPScore;
            object_to_link.level_compl_detect = FindObjectOfType<LevelCompleteDetection>() as LevelCompleteDetection;
            object_to_link.npc_contr = FindObjectOfType<NPCController>() as NPCController;            
            object_to_link.box_contr = FindObjectOfType<NpcBoxController>() as NpcBoxController;
            object_to_link.bg_scroll_cont = FindObjectOfType<BackgroundScrollController>() as BackgroundScrollController;            
            object_to_link.UI_gameobject = GameObject.Find("UI");            
            object_to_link.pick_item = FindObjectOfType<PickItem>() as PickItem;
            object_to_link.pickable_manager = FindObjectOfType<PickableItemSpawnManager>() as PickableItemSpawnManager;
            object_to_link.enter_exit_dungeon = FindObjectOfType<EnterExitDungeon>() as EnterExitDungeon;

            object_to_link.ally_spider = FindObjectOfType<AllySpiderController>() as AllySpiderController;
            object_to_link.ally_dragonfly = FindObjectOfType<AllyDragonflyController>() as AllyDragonflyController;
            object_to_link.ally_praying_mantis = FindObjectOfType<AllyPrayingMantisController>() as AllyPrayingMantisController;

            //линкуем все
            object_to_link.gameplay_gui_man.weap_man = object_to_link.weap_man;

            object_to_link.hero_cont.weap_man = object_to_link.weap_man;
            object_to_link.hero_cont.game_man = object_to_link.game_man;
            //object_to_link.hero_cont.cinemachineFramingTransposer = object_to_link.cinemachine_camera.GetCinemachineComponent<CinemachineFramingTransposer>();

            object_to_link.gui_man_hp_score.game_man = object_to_link.game_man;

            object_to_link.weap_man.hero_animator_controller = object_to_link.hero_cont.GetComponent<Animator>();
            object_to_link.weap_man.gameplay_gui_manager = object_to_link.gameplay_gui_man;
            object_to_link.weap_man.hero_contr = object_to_link.hero_cont;

            object_to_link.npc_contr.game_man = object_to_link.game_man;            
            object_to_link.npc_contr.dialog_man = object_to_link.dialog_man;
            
            object_to_link.dialog_man.game_man = object_to_link.game_man;

            object_to_link.box_contr.game_man = object_to_link.game_man;

            object_to_link.level_compl_detect.game_man = object_to_link.game_man;

            object_to_link.pick_item.hero_cont = object_to_link.hero_cont;

            object_to_link.game_man.hero_contr = object_to_link.hero_cont;
            object_to_link.game_man.game_progr = object_to_link.game_progr;
            object_to_link.game_man.dialog_man = object_to_link.dialog_man;
            object_to_link.game_man.npc_contr = object_to_link.npc_contr;
            object_to_link.game_man.menu_man = object_to_link.menu_man;
            object_to_link.game_man.box_cont = object_to_link.box_contr;                            
            object_to_link.game_man.gui_manager_hp = object_to_link.gui_man_hp_score;
            object_to_link.game_man.pickable_object_spawn_manager = object_to_link.pickable_manager;            

            object_to_link.bg_scroll_cont.menu_man = object_to_link.menu_man;
            object_to_link.bg_scroll_cont.hero_rb2d = object_to_link.hero_cont.rb2d;

            object_to_link.pickable_manager.dna_parts_parent_go = GameObject.Find("/dna_parts");

            if (object_to_link.enter_exit_dungeon != null)
            {
                object_to_link.enter_exit_dungeon.game_man = object_to_link.game_man;
            }

            //линкуем насекомых-союзников

            object_to_link.ally_spider.weap_man = object_to_link.weap_man;
            object_to_link.ally_spider.attack_way[0] = object_to_link.hero_cont.gameObject.transform.Find("weapons/ally_spider/attack_start_point").gameObject;

            object_to_link.ally_dragonfly.weap_man = object_to_link.weap_man;
            object_to_link.ally_dragonfly.attack_way[0] = object_to_link.hero_cont.gameObject.transform.Find("weapons/ally_dragonfly/attack_way/p0").gameObject;
            object_to_link.ally_dragonfly.attack_way[1] = object_to_link.hero_cont.gameObject.transform.Find("weapons/ally_dragonfly/attack_way/p1").gameObject;
            object_to_link.ally_dragonfly.attack_way[2] = object_to_link.hero_cont.gameObject.transform.Find("weapons/ally_dragonfly/attack_way/p2").gameObject;
            object_to_link.ally_dragonfly.attack_way[3] = object_to_link.hero_cont.gameObject.transform.Find("weapons/ally_dragonfly/attack_way/p3").gameObject;
            object_to_link.ally_dragonfly.attack_way[4] = object_to_link.hero_cont.gameObject.transform.Find("weapons/ally_dragonfly/attack_way/p4").gameObject;

            object_to_link.ally_praying_mantis.weap_man = object_to_link.weap_man;
            object_to_link.ally_praying_mantis.attack_way[0] = object_to_link.hero_cont.gameObject.transform.Find("weapons/ally_praying_mantis/attack_way/p_start").gameObject;
            object_to_link.ally_praying_mantis.p_start  = object_to_link.hero_cont.gameObject.transform.Find("weapons/ally_praying_mantis/attack_way/p_start").gameObject;
            object_to_link.ally_praying_mantis.p_end    = object_to_link.hero_cont.gameObject.transform.Find("weapons/ally_praying_mantis/attack_way/p_end").gameObject;
            object_to_link.ally_praying_mantis.p_evac   = object_to_link.hero_cont.gameObject.transform.Find("weapons/ally_praying_mantis/attack_way/p_evac").gameObject;
            object_to_link.ally_praying_mantis.p_land_11   = object_to_link.hero_cont.gameObject.transform.Find("weapons/ally_praying_mantis/attack_way/landing_zone/p11").gameObject;
            object_to_link.ally_praying_mantis.p_land_12 = object_to_link.hero_cont.gameObject.transform.Find("weapons/ally_praying_mantis/attack_way/landing_zone/p12").gameObject;
            object_to_link.ally_praying_mantis.p_land_21 = object_to_link.hero_cont.gameObject.transform.Find("weapons/ally_praying_mantis/attack_way/landing_zone/p21").gameObject;
            object_to_link.ally_praying_mantis.p_land_22 = object_to_link.hero_cont.gameObject.transform.Find("weapons/ally_praying_mantis/attack_way/landing_zone/p22").gameObject;            

            //линкуем насекомых противников                       

            GameObject [] foes = GameObject.FindGameObjectsWithTag("Enemy");
            FoeBase cur_fb;
            object_to_link.game_man.foe_controller = new Dictionary<int, FoeBase>();

            for (int i = 0; i < foes.Length; i++)
            {
                cur_fb = foes[i].GetComponent<FoeBase>();                
                cur_fb.player = object_to_link.hero_cont.gameObject;

                object_to_link.game_man.foe_controller[i] = cur_fb;

                FoeTakeDamage ftd = foes[i].GetComponent<FoeTakeDamage>();
                ftd.weap_man = object_to_link.weap_man;
                ftd.game_man = object_to_link.game_man;
                ftd.ID = i;
            }

            //линкуем поля cinemachine

            object_to_link.cinemachine_camera.Follow = object_to_link.hero_cont.gameObject.transform;
            object_to_link.cinemachine_camera.LookAt = object_to_link.hero_cont.gameObject.transform;
            object_to_link.cinemachine_camera.m_Lens.OrthographicSize = 110;
            object_to_link.cinemachine_camera.transform.position = new Vector3(1,1,-50);
            object_to_link.cinemach_confiner.m_BoundingShape2D = GameObject.Find("camera_border").GetComponent<PolygonCollider2D>();

            //линкуем поля всех оружий - экземпляров класса weapon

            foreach (WeaponLinks weap_link in object_to_link.hero_cont.weapons_links)
            {

                object_to_link.weap_man.weapons_dict[weap_link.name].weapon_gameobj = weap_link.weap_go;
                object_to_link.weap_man.weapons_dict[weap_link.name].weapon_ps = weap_link.weap_ps;
                object_to_link.weap_man.weapons_dict[weap_link.name].gauss_gun = weap_link.gauss_gun_contr;

                if (weap_link.name.Equals("ally_spider"))
                    object_to_link.weap_man.weapons_dict[weap_link.name].ally_insect = object_to_link.ally_spider;
                if (weap_link.name.Equals("ally_dragonfly"))
                    object_to_link.weap_man.weapons_dict[weap_link.name].ally_insect = object_to_link.ally_dragonfly;
                if (weap_link.name.Equals("ally_praying_mantis"))
                    object_to_link.weap_man.weapons_dict[weap_link.name].ally_insect = object_to_link.ally_praying_mantis;

            }

            //линкуем функции кнопок управления игрока
            GameObject button;
            EventTrigger et;
            EventTrigger.Entry entry;
            
            //left
            button = object_to_link.UI_gameobject.transform.Find("gameplay/controls/move/left").gameObject;
            et = button.GetComponent<EventTrigger>();

            entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerDown;
            entry.callback.AddListener((eventData) => { object_to_link.hero_cont.press_move_left(); });
            et.triggers.Add(entry);

            entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerUp;
            entry.callback.AddListener((eventData) => { object_to_link.hero_cont.release_move_button(); });
            et.triggers.Add(entry);

            //right
            button = object_to_link.UI_gameobject.transform.Find("gameplay/controls/move/right").gameObject;
            et = button.GetComponent<EventTrigger>();

            entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerDown;
            entry.callback.AddListener((eventData) => { object_to_link.hero_cont.press_move_right(); });
            et.triggers.Add(entry);

            entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerUp;
            entry.callback.AddListener((eventData) => { object_to_link.hero_cont.release_move_button(); });
            et.triggers.Add(entry);

            //jump
            button = object_to_link.UI_gameobject.transform.Find("gameplay/controls/jump_attack/jump").gameObject;
            et = button.GetComponent<EventTrigger>();

            entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerDown;
            entry.callback.AddListener((eventData) => { object_to_link.hero_cont.press_jump(); });
            et.triggers.Add(entry);

            //attack
            button = object_to_link.UI_gameobject.transform.Find("gameplay/controls/jump_attack/attack").gameObject;
            et = button.GetComponent<EventTrigger>();

            entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerDown;
            entry.callback.AddListener((eventData) => { object_to_link.hero_cont.press_attack(); });
            et.triggers.Add(entry);

            entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerUp;
            entry.callback.AddListener((eventData) => { object_to_link.hero_cont.release_attack(); });
            et.triggers.Add(entry);
        }
        else
        {
            object_to_link.cinemachine_camera.LookAt = null;
            object_to_link.cinemachine_camera.LookAt = null;
            object_to_link.bg_scroll_cont = FindObjectOfType<BackgroundScrollController>() as BackgroundScrollController;

            object_to_link.evo_pan_man.game_man = object_to_link.game_man;
            object_to_link.evo_pan_man.menu_man = object_to_link.menu_man;

            object_to_link.game_man.bg_scroll_cont = object_to_link.bg_scroll_cont;            
            object_to_link.game_man.game_progr = object_to_link.game_progr;
            object_to_link.game_man.menu_man = object_to_link.menu_man;
            object_to_link.game_man.evo_pan_man = object_to_link.evo_pan_man;
            object_to_link.menu_man.game_man = object_to_link.game_man;
            object_to_link.game_man.audio_man = object_to_link.audio_man;            
        }
    }

    //evo-------------------------------------------------------------------------------------------------

    static int recalc_params_rounded(int init, float incr_perc)
    {        
        int increase_on = (int)(init * incr_perc);
        
        int remainder = increase_on % 10;

        if (remainder <= 5)
            increase_on += 5 - remainder;
        else
            increase_on += 10 - remainder;

        return init + increase_on;
    }

    static float recalc_params_percent(float init, float incr_perc)
    {
        return init + init * incr_perc / 100;
    }

    public static void align_weapons_characteristics(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != "start_menu")
        {
            EvoPanelManager evo_pam_mgr = object_to_link.evo_pan_man;
            WeaponManager weap_man = object_to_link.weap_man;

            foreach (Weapon w in weap_man.weapons)
            {
                w.recalc_weapon_data = w.initial_weapon_data;
                //w.recalc_weapon_data.max_charges = w.initial_weapon_data.max_charges;
                //w.recalc_weapon_data.damage_rate = w.initial_weapon_data.damage_rate;
                //w.recalc_weapon_data.weight = w.initial_weapon_data.weight;

                if (!w.weapon_name.Equals("roll"))
                {                    

                    if (evo_pam_mgr.evo_items[8].is_bought) //AMMO
                        w.recalc_weapon_data.charges_count = recalc_params_rounded(w.initial_weapon_data.charges_count, evo_pam_mgr.evo_items[9].value);

                    if (evo_pam_mgr.evo_items[9].is_bought)          //DAMAGE
                        w.recalc_weapon_data.damage_rate = recalc_params_rounded(w.initial_weapon_data.damage_rate, evo_pam_mgr.evo_items[10].value);

                    if (evo_pam_mgr.evo_items[10].is_bought)      //WEIGHT
                        w.recalc_weapon_data.weight = recalc_params_percent(w.initial_weapon_data.weight, evo_pam_mgr.evo_items[11].value);

                    if (evo_pam_mgr.evo_items[11].is_bought)       //RAND_AT_START
                    {

                    }

                    if (evo_pam_mgr.evo_items[12].is_bought)            //W4
                    {

                    }

                    if (evo_pam_mgr.evo_items[13].is_bought)            //W5
                    {

                    }

                    if (evo_pam_mgr.evo_items[14].is_bought)            //W6
                    {

                    }

                    if (evo_pam_mgr.evo_items[15].is_bought)            //W7
                    {

                    }
                }

            }
        }
    }

    public static void align_hero_characteristics(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != "start_menu")
        {
            HeroController hero_cont = object_to_link.hero_cont;
            EvoPanelManager evo_pan_man = object_to_link.evo_pan_man;

            hero_cont.recalc_hero_params.cur_hero_hp = hero_cont.initial_hero_params.cur_hero_hp;
            hero_cont.recalc_hero_params.max_hero_hp = hero_cont.initial_hero_params.max_hero_hp;
            hero_cont.recalc_hero_params.maxSpeed = hero_cont.initial_hero_params.maxSpeed;
            hero_cont.recalc_hero_params.moveForce = hero_cont.initial_hero_params.moveForce;
            hero_cont.recalc_hero_params.jumpForce = hero_cont.initial_hero_params.jumpForce;
            hero_cont.weap_man.weapons_dict["roll"].recalc_weapon_data.damage_rate = hero_cont.weap_man.weapons_dict["roll"].initial_weapon_data.damage_rate;
            hero_cont.recalc_hero_params.roll_reload_time = hero_cont.initial_hero_params.roll_reload_time;            
            hero_cont.recalc_hero_params.blink_and_inv_time = hero_cont.initial_hero_params.blink_and_inv_time;

            if (evo_pan_man.evo_items[0].is_bought)       //RUN_SPEED
                hero_cont.recalc_hero_params.maxSpeed += evo_pan_man.evo_items[0].value;

            if (evo_pan_man.evo_items[1].is_bought)       //B_JUMP_STRENGTH
                hero_cont.recalc_hero_params.jumpForce = recalc_params_percent(hero_cont.recalc_hero_params.jumpForce, evo_pan_man.evo_items[1].value);

            if (evo_pan_man.evo_items[2].is_bought)       //B_ROLL_DAMAGE
                hero_cont.weap_man.weapons_dict["roll"].recalc_weapon_data.damage_rate *= 2;

            if (evo_pan_man.evo_items[3].is_bought)                      //B_ROLL_RELOAD_TIME
                hero_cont.recalc_hero_params.roll_reload_time -= evo_pan_man.evo_items[3].value;

            if (evo_pan_man.evo_items[4].is_bought)                   //HP_PLUS_1
            {
                hero_cont.recalc_hero_params.max_hero_hp = 4;
                hero_cont.recalc_hero_params.cur_hero_hp = 4;
            } 

            if (evo_pan_man.evo_items[5].is_bought)           //B_INV_TIME
                hero_cont.recalc_hero_params.blink_and_inv_time += evo_pan_man.evo_items[5].value;
            

            if (evo_pan_man.evo_items[6].is_bought)           //B_PROB_DNA
            {
                
            }

            if (evo_pan_man.evo_items[7].is_bought)           //B_PROB_HP
            {

            }
        }
    }

    public static void update_gameplay(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != "start_menu")
        {            
            object_to_link.weap_man.take_away_weapons();            
            object_to_link.gameplay_gui_man.level_loaded();
            object_to_link.gui_man_hp_score.update_player_hp_sprite(object_to_link.hero_cont.recalc_hero_params.cur_hero_hp);
            object_to_link.bg_scroll_cont.initialize_bg_scroll();
            object_to_link.bg_scroll_cont.activate_bg_scroll(); 
            object_to_link.game_man.player_points.drop_cur_points();
            object_to_link.gui_man_hp_score.update_player_score(level_start:true);
            object_to_link.pickable_manager.instantiate_dna_parts();

            //if (object_to_link.game_man.scenes_dict[scene.name].is_dna_found)
            //    object_to_link.dna_contr.gameObject.SetActive(false);

            if (object_to_link.game_man.scenes_dict[scene.name].is_friend_saved)
                object_to_link.box_contr.destroy(pre_destr:true);
        }
        else
        {
            object_to_link.bg_scroll_cont.deactivate_bg_scroll(deact_go:true);
        }

    }

    public static void play_music(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "start_menu")
        {

        }
        else if (scene.name == "infinite_run")          
        {

        }
        else if(scene.name.Contains("boss"))
        {

        }
        else if (scene.name.Contains("act1"))
        {
            
        }
        else if (scene.name.Contains("act2"))
        {

        }
        else if (scene.name.Contains("act3"))
        {

        }


    }

    static public void run_enemies(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != "start_menu")
        {
            game_man.run_enemies();
        }
    }

    //other---------------------------------------------------------------

    public static void drop_singlton_counter(Scene scene, LoadSceneMode mode)
    {
        Singleton.num = 0;
    }

    static public void check_event_system(Scene scene, LoadSceneMode mode)
    {
        EventSystem menuEventSystem = object_to_link.event_sys;

        if (menuEventSystem == null)
        {
            GameObject obj = new GameObject("EventSystem");        
            menuEventSystem = obj.AddComponent<EventSystem>();
            obj.AddComponent<StandaloneInputModule>().forceModuleActive = true;
        }
    }

    static public void start_stop_cloud_animation(Scene scene, LoadSceneMode mode)
    {
        MenuManager menu_man = object_to_link.menu_man;
        if (scene.name == "start_menu")
            menu_man.cloud_anim.SetBool("play", true);
        else
            menu_man.cloud_anim.SetBool("play", false);
    }
    
}
