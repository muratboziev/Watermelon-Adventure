/*
 epic phrases
 oh boya
 yes! this is what im talking about
 this is it
 here we go
 this is getting serious
 thats aint no joke
 im serious this time
  */

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


//UI-------------------------------------

[System.Serializable]
public class uiPanel
{
    public GameObject gameobj;
    public CanvasGroup canvas;
}

[System.Serializable]
public class Panels
{
    public uiPanel mainmenuPanel;
    public uiPanel gameModeSelectPanel;
    public uiPanel optionsPanel_mainmenu;
    public uiPanel optionsPanel_pause;
    public uiPanel evolutionPanel;
    public uiPanel buyevoPanel;
    public uiPanel pausePanel;
    public uiPanel actSelection;
    public uiPanel levelSelection;
    public uiPanel infiniterun;
    public uiPanel gameplayContoller;
    public uiPanel levelComplete;
    public uiPanel loadingPanel;
    public uiPanel backgroundPanel;
    public uiPanel clickBlocker;
}

[System.Serializable]
public class levelSelectionImage
{
    public Image level_image;
    public GameObject achieve_panel;
    public Image[] achievments;
}

[System.Serializable]
public class GUISprites
{
    public Sprite[] act_opened_sprite;
    public Sprite act_closed_sprite;
    public Sprite level_opened_sprite;
    public Sprite level_closed_sprite;

    public Sprite star_gained;
    public Sprite star_not_gained;
    public Sprite ally_saved_sprite;
    public Sprite ally_not_saved_sprite;
    public Sprite dna_found_sprite;
    public Sprite dna_not_found_sprite;
        
    public Sprite [] actText;

}

[System.Serializable]
public class GUIImages
{
    [Header("Level selection panel")]
    public Image[] act_image;
    public Image act_text;
    public levelSelectionImage[] level_selection_image;

    [Header("Level complete panel")]
    public Image star_image;
    public Image friend_image;
    public Image dna_image;

    public Text level_complete_text;
    public Text friend_saved_text;
    public Text friend_name_text;
    public Text dna_text;
}

[System.Serializable]
public class OptionsGUI
{
    public Slider slider_music_mainmenu;
    public Slider slider_sound_mainmenu;
    public Slider slider_music_pause;
    public Slider slider_sound_pause;
}

//menu transitions-------------------------------------------------

public class MenuTransition
{    
    public Menu_State state_from;
    public Menu_State state_to;    

    public bool with_fading;
    public bool with_scene_loading;
    
    public MenuTransition(Menu_State from, Menu_State to, bool fade, bool load)
    {
        state_from = from;
        state_to = to;
        with_fading = fade;
        with_scene_loading = load;        
    }

}

public class Menu_State
{    
    public uiPanel panel;
    public MenuTransition[] transition;
    public MenuTransition back_transition;
    public string name;

    public Menu_State(string name_)
    {
        name = name_;
    }

    /*public static bool operator ==(Menu_State st1, Menu_State st2)
    {        
        return st1.name.Equals(st2.name);
    }

    public static bool operator !=(Menu_State st1, Menu_State st2)
    {
        return !st1.name.Equals(st2.name);
    }*/
}

