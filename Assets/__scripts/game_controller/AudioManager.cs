using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Audio sources")]
    public AudioSource bg_music;    
    public AudioSource menu_sound;
    public AudioSource friend_saved;
    public AudioSource pick_dna;    
    public AudioSource pick_pill;
    public AudioSource box_audio_source;    

    [Header("Audio clips")]
    public AudioClip menu_item_pressed;
    public AudioClip menu_item_blocked;
    public AudioClip level_completed;
    public AudioClip level_achievement_friend;
    public AudioClip level_achievement_dna;    
    public AudioClip evo_bought;    

    public AudioClip main_menu_music;
    public AudioClip infinite_run_music;
    public AudioClip act_1_music;
    public AudioClip act_2_music;
    public AudioClip act_3_music;
    public AudioClip boss_fight_music;

    public AudioClip pick_dna_sound;
    public AudioClip pick_dna_part_sound;
    public AudioClip pick_pill_sound;
    public AudioClip pick_fertilizer_sound;

    public AudioClip damaged_sound;
    public AudioClip destroyed_sound;

    public void play_friend_saved()
    {
        friend_saved.Play();
    }

    public void play_menu_transition_enabled()
    {
        menu_sound.clip = menu_item_pressed;
        menu_sound.Play();
    }

    public void play_menu_transition_disabled()
    {
        menu_sound.clip = menu_item_blocked;
        menu_sound.Play();
    }

    public void play_evo_bought()
    {
        menu_sound.clip = evo_bought;
        menu_sound.Play();
    }

    public void play_pick_dna()
    {
        pick_dna.clip = pick_dna_sound;
        pick_dna.Play();
    }

    public void play_pick_dna_part()
    {
        pick_dna.clip = pick_dna_part_sound;
        pick_dna.Play();
    }

    public void play_pick_pill()
    {
        pick_dna.clip = pick_pill_sound;
        pick_dna.Play();
    }

    public void play_pick_fertilizer()
    {
        pick_dna.clip = pick_fertilizer_sound;
        pick_dna.Play();
    }

    public void play_box_damaged()
    {
        box_audio_source.clip = damaged_sound;
        box_audio_source.Play();
    }

    public void play_box_destroyed()
    {
        box_audio_source.clip = destroyed_sound;
        box_audio_source.Play();
    }

    public void play_level_complete()
    {
        menu_sound.clip = level_completed;
        menu_sound.Play();
    }

    public void play_achievement_friend()
    {
        menu_sound.clip = level_achievement_friend;
        menu_sound.Play();
    }

    public void play_achievement_dna()
    {
        menu_sound.clip = level_achievement_dna;
        menu_sound.Play();
    }


}
