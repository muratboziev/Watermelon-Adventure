using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class GameProgressSaver : MonoBehaviour
{
    BinaryFormatter binary_formatter = new BinaryFormatter();
    FileStream filestream;

    string path_to_game_progress_data;
    string path_to_evolution_data;
    string path_to_player_points;
    string sound_level_str = "sound_level", music_level_str = "music_level";

    //--------------------------------------------------------

    public void load_game_progress(ref SceneInfo[] scenes, ref bool [] evo_item_buy_state, ref PlayerPoints player_p)
    {
        path_to_game_progress_data = Application.persistentDataPath + "/gameprogress.dat";
        path_to_evolution_data = Application.persistentDataPath + "/evolution.dat";
        path_to_player_points = Application.persistentDataPath + "/playerpoints.dat";

        PlayerPrefs.DeleteAll();
        if (File.Exists(path_to_game_progress_data))        
            File.Delete(path_to_game_progress_data);
        if (File.Exists(path_to_evolution_data))
            File.Delete(path_to_evolution_data);
        if (File.Exists(path_to_player_points))
            File.Delete(path_to_player_points);

        if (!PlayerPrefs.HasKey(sound_level_str))
        {
            PlayerPrefs.SetInt(music_level_str, 5);
            PlayerPrefs.SetInt(sound_level_str, 5);
        }

        scenes = load_story_progress(scenes);
        evo_item_buy_state = load_evolution_progress(evo_item_buy_state);
        player_p = load_player_points(player_p);
    }

    public void save_game_progress(SceneInfo[] level_progress, bool [] evo_item_buy_state, PlayerPoints player_progress)
    {
        if (path_to_game_progress_data != null)             //if not destroyed by singleton
        {
            if (File.Exists(path_to_game_progress_data))
            {
                filestream = File.Open(path_to_game_progress_data, FileMode.Open);
            }
            else
            {
                filestream = File.Create(path_to_game_progress_data);
            }
            binary_formatter.Serialize(filestream, level_progress);
            filestream.Close();
        }

        if (path_to_evolution_data != null)
        {
            if (File.Exists(path_to_evolution_data))
            {
                filestream = File.Open(path_to_evolution_data, FileMode.Open);
            }
            else
            {
                filestream = File.Create(path_to_evolution_data);
            }
            binary_formatter.Serialize(filestream, evo_item_buy_state);
            filestream.Close();
        }

        if (path_to_player_points != null)
        {
            if (File.Exists(path_to_player_points))
            {
                filestream = File.Open(path_to_player_points, FileMode.Open);
            }
            else
            {
                filestream = File.Create(path_to_player_points);
            }
            binary_formatter.Serialize(filestream, player_progress);
            filestream.Close();
        }
    }

    //---------------------------------------------------------

    bool [] make_initial_evo_items(bool [] evo_items_buy_state)
    {        
        for (int i = 0; i < evo_items_buy_state.Length; i++)
            evo_items_buy_state[i] = false;

        return evo_items_buy_state;
    }

    public SceneInfo [] load_story_progress(SceneInfo [] scene_inf)
    {
        if (File.Exists(path_to_game_progress_data))
        {
            filestream = File.Open(path_to_game_progress_data, FileMode.Open);
            scene_inf = (SceneInfo [])binary_formatter.Deserialize(filestream);
            filestream.Close();
        }

        return scene_inf;
    }

    public bool [] load_evolution_progress(bool [] evo_items_buy_state)
    {
        if (File.Exists(path_to_evolution_data))
        {
            filestream = File.Open(path_to_evolution_data, FileMode.Open);
            evo_items_buy_state = (bool []) binary_formatter.Deserialize(filestream);
            filestream.Close();
        }
        else
        {
            evo_items_buy_state = make_initial_evo_items(evo_items_buy_state);
        }

        return evo_items_buy_state;
    }

    public PlayerPoints load_player_points(PlayerPoints player_p)
    {
        if (File.Exists(path_to_player_points))
        {
            filestream = File.Open(path_to_player_points, FileMode.Open);
            player_p = (PlayerPoints)binary_formatter.Deserialize(filestream);
            filestream.Close();
        }

        player_p.drop_cur_points();

        return player_p;
    }

}
