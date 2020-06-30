using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.SceneManagement;
//123
public class GameManager : MonoBehaviour
{
    [Header("Links")]
    public GameProgressSaver game_progr;
    public HeroController hero_contr;
    public BackgroundScrollController bg_scroll_cont;
    public MenuManager menu_man;
    public DialogManager dialog_man;
    public EvoPanelManager evo_pan_man;
    public AudioManager audio_man;
    public NPCController npc_contr;
    public NpcBoxController box_cont;
    public GUIManagerHPScore gui_manager_hp;
    public FoeBase[] foe_controller;
    public PickableItemSpawnManager pickable_object_spawn_manager;    

    [Header("Game data")]
    public SceneInfo[] scenes = new SceneInfo[33];
    public CharacterData[] character_data = new CharacterData[33];    

    public PlayerPoints player_points = new PlayerPoints();
    [HideInInspector]
    public string playable_character;

    public Dictionary<string, SceneInfo> scenes_dict = new Dictionary<string, SceneInfo>();
    public Dictionary<string, CharacterData> npc_scene_dict = new Dictionary<string, CharacterData>();    
    public Dictionary<string, CharacterData> npc_name_dict = new Dictionary<string, CharacterData>();

    [Header("Animation")]
    public Animator score_panel_animator;    

    bool[] evo_item_buy_state = new bool[16];             //сериализуем этот массив т.к. evo_item не получается (т.к. там поля типа Sprite)    
    
    string sound_level_str = "sound_level", music_level_str = "music_level";
    string kinematic_foes = "larva|caterpillar_web|sawfly|rose_sawfly|cab_butterfly|thrips|ladybug_larva|thrips_fly";

    Vector2 force_to_dna_part = new Vector2();

    List<AudioSource> paused_audioSources = new List<AudioSource>();

    [HideInInspector]
    public Dictionary<string, Color> preset_colors = new Dictionary<string, Color>();

    //--------------------------------------------------------------------------------

    private void Start()
    {
        preset_colors["red"] = Color.red;
        preset_colors["orange"] = Color.yellow;
        preset_colors["yellow"] = Color.yellow;
        preset_colors["green"] = Color.green;
        preset_colors["cyan"] = Color.cyan;
        preset_colors["blue"] = Color.blue;
        preset_colors["magenta"] = Color.magenta;

        game_progr.load_game_progress(ref scenes, ref evo_item_buy_state, ref player_points);

        for (int i = 0; i < evo_pan_man.evo_items.Length; i++)
        {
            evo_pan_man.evo_items[i].is_bought = evo_item_buy_state[i];
        }

        foreach (SceneInfo sc in scenes)
        {
            scenes_dict[sc.name] = sc;
        }

        foreach (CharacterData cur_npc_data in character_data)
        {
            npc_scene_dict[cur_npc_data.scene_name] = cur_npc_data;
            npc_name_dict[cur_npc_data.name] = cur_npc_data;
        }        
    }

    private void OnDisable()
    {
        if (game_progr != null)                  //проверка для экземпляра уничтожаемого singletonом (при возвращении на сцену главного меню из сцены уровня)
        {
            for (int i = 0; i < evo_pan_man.evo_items.Length; i++)
            {
                evo_item_buy_state[i] = evo_pan_man.evo_items[i].is_bought;
            }

            game_progr.save_game_progress(scenes, evo_item_buy_state, player_points);
        }
    }//123

    //--------------------------------------------------------------------------------

    public string cur_scene_name
    {
        get
        {
            return SceneManager.GetActiveScene().name;
        }
    }

    public void run_enemies()
    {
        foreach (FoeBase fb in foe_controller)
        {
            fb.start_attack();
            if (kinematic_foes.Contains(fb.gameObject.name))
                fb.gameObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
            else
                fb.gameObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        }
    }

    public void stop_enemies()
    {
        foreach (FoeBase fb in foe_controller)
        {
            fb.stop_attack();
            fb.stop_and_idle();
            fb.stop_audioSources();
            fb.gameObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
        }
    }

    public void block_hero_control(bool block_control = false, bool stop_move = false)
    {
        hero_contr.block_hero_control(block_control, stop_move);
    }

    public void level_complete_zone_1()
    {
        stop_enemies();
        block_hero_control(block_control: true, stop_move: true);

        CinemachineVirtualCamera cinemach_camera;
        SceneInfo cur_scene;                

        cinemach_camera = FindObjectOfType<CinemachineVirtualCamera>() as CinemachineVirtualCamera;
        cinemach_camera.Follow = null;
        cinemach_camera.LookAt = null;

        bg_scroll_cont.deactivate_bg_scroll(deact_go: false);

        //-----------------
        
        StartCoroutine(hero_contr.evacuate());

        //-----------------

        cur_scene = scenes_dict[cur_scene_name];
        scenes_dict[cur_scene.next_scene_name].opened = true;
        cur_scene.completed = true;

        bool friend_saved = cur_scene.is_friend_saved || player_points.is_friend_saved_level;
        bool dna_found = cur_scene.is_dna_found || player_points.is_dna_found_level;

        menu_man.level_complete(friend_saved, dna_found, npc_scene_dict[cur_scene_name]);

    }

    public void box_destroyed(bool pre_destr)
    {
        if (!pre_destr)
        {
            block_hero_control(block_control: true, stop_move: true);
            hero_contr.check_and_change_direction(npc_contr.gameObject);
            npc_contr.have_dialog_and_evac();            
        }
        else
        {
            npc_contr.gameObject.SetActive(false);
            gui_manager_hp.update_player_score(friend_saved: true);
        }
    }

    //----------------------------------------------------------------------

    public void instantiate_dna_parts(Transform position)
    {
        GameObject go;
        int k;

        go = pickable_object_spawn_manager.spawn_dna(position, 0);

        k = Random.Range(-1, 1);
        force_to_dna_part.x = Random.Range(3000, 5000) * Mathf.Sign(k);
        force_to_dna_part.y = Random.Range(8000, 15000);
        go.GetComponent<Rigidbody2D>().AddForce(force_to_dna_part);

        k = Random.Range(-1, 1);
        go = pickable_object_spawn_manager.spawn_dna(position, 1);
        force_to_dna_part.x = Random.Range(3000, 5000) * Mathf.Sign(k);
        force_to_dna_part.y = Random.Range(8000, 15000);
        go.GetComponent<Rigidbody2D>().AddForce(force_to_dna_part);

        k = Random.Range(-1, 1);
        go = pickable_object_spawn_manager.spawn_dna(position, 2);
        force_to_dna_part.x = Random.Range(3000, 5000) * Mathf.Sign(k);
        force_to_dna_part.y = Random.Range(8000, 15000);
        go.GetComponent<Rigidbody2D>().AddForce(force_to_dna_part);

        k = Random.Range(-1, 1);
        go = pickable_object_spawn_manager.spawn_dna(position, 3);
        force_to_dna_part.x = Random.Range(3000, 5000) * Mathf.Sign(k);
        force_to_dna_part.y = Random.Range(8000, 15000);
        go.GetComponent<Rigidbody2D>().AddForce(force_to_dna_part);
    }


    //----------------------------------------------------------------------

    public string make_level_name(int act, int level)
    {
        if (level == 11)
            return "act" + act.ToString() + "_boss";
        return "act" + act.ToString() + "_level" + level.ToString();
    }

    public int sound_level
    {
        get { return PlayerPrefs.GetInt(sound_level_str); }
        set { PlayerPrefs.SetInt(sound_level_str, value); }
    }

    public int music_level
    {
        get { return PlayerPrefs.GetInt(music_level_str); }
        set { PlayerPrefs.SetInt(music_level_str, value); }
    }

    public void pause_audiosources()
    {
        var FoundAudioSources = FindObjectsOfType<AudioSource>();

        paused_audioSources.Clear();

        foreach (AudioSource audiosource in FoundAudioSources)
        {
            if (audiosource.isPlaying)
            {
                audiosource.Pause();
                paused_audioSources.Add(audiosource);
            }
        }
    }

    public void unpause_audiosources()
    {
        foreach (AudioSource audiosource in paused_audioSources)
        {
            audiosource.UnPause();            
        }
    }

    //----------------------------------------------------------------------
        public HeroHealthSprite cur_hero_health_sprite() 
    {
        return npc_name_dict[playable_character].health_sprites;
    }

}
