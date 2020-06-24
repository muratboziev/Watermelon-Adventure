using System.Collections;
using System;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class DialogReplica
{
    public bool is_heroes_phrase;
    public string phrase;
}

[Serializable]
public class Dialog
{
    public DialogReplica[] replica;
}

public class DialogManager : MonoBehaviour
{
    public GameManager game_man;

    [Header("Params")]
    public float fade_duration = 0.65f;
    public float pause_between_phrases = 2.2f;

    [Header("Gameplay UI")]
    public CanvasGroup weapon_panel_canvas;
    public CanvasGroup score_panel_canvas;
    public CanvasGroup hero_hp_canvas;
    public GameObject weapon_panel_go;
    public GameObject score_panel_go;
    public GameObject hero_hp_go;

    [Header("Dialog UI")]
    public CanvasGroup dialog_panel_canvas;
    public GameObject dialog_panel_go;
    [Header("Hero")]
    public Text hero_text;
    public GameObject hero_name_go;
    [Header("NPC")]
    public Image npc_image;
    public Text npc_name;
    public GameObject npc_name_go;
    public Text npc_text;

    [HideInInspector]
    public bool having_dialog = false;

    Coroutine dialog_coroutine = null;    
    float elapsed_time;
    bool phrase_skiped;

    public void have_dialog()
    {
        game_man.stop_enemies();

        having_dialog = true;

        string cur_scene = game_man.cur_scene_name;
        CharacterData npc_data = game_man.npc_scene_dict[cur_scene];

        dialog_coroutine = StartCoroutine(do_dialog(npc_data));
    }

    public void skip_phrase()
    {
        phrase_skiped = true;
    }

    IEnumerator do_dialog(CharacterData npc_data)
    {       
        npc_image.sprite = npc_data.saved_npc_sprite;
        npc_text.text = String.Empty;
        npc_name.text = npc_data.full_name;
        npc_name.color = game_man.preset_colors[npc_data.name_text_color];
        npc_name_go.SetActive(false);

        hero_text.text = String.Empty;
        hero_name_go.SetActive(false);

        //hide gameplay panels
        elapsed_time = 0f;

        while (elapsed_time < fade_duration)
        {
            elapsed_time += Time.deltaTime;
            weapon_panel_canvas.alpha = Mathf.Lerp(1, 0, elapsed_time / fade_duration);
            score_panel_canvas.alpha = Mathf.Lerp(1, 0, elapsed_time / fade_duration);
            hero_hp_canvas.alpha = Mathf.Lerp(1, 0, elapsed_time / fade_duration);
            yield return null;
        }

        weapon_panel_go.SetActive(false);
        score_panel_go.SetActive(false);
        hero_hp_go.SetActive(false);

        //show dialog uia

        dialog_panel_go.SetActive(true);
        elapsed_time = 0f;

        while (elapsed_time < fade_duration)
        {
            elapsed_time += Time.deltaTime;
            dialog_panel_canvas.alpha = Mathf.Lerp(0, 1, elapsed_time / fade_duration);            
            yield return null;
        }

        //have dialog

        float cur_pause_between_phrases = pause_between_phrases;
        float cur_phrase_start_time = 0f;

        foreach (DialogReplica cur_replica in npc_data.dialog.replica)
        {
            hero_text.text = String.Empty;
            npc_text.text = String.Empty;
            npc_name_go.SetActive(false);
            hero_name_go.SetActive(false);

            if (cur_replica.is_heroes_phrase)
            {
                hero_name_go.SetActive(true);
                hero_text.text = cur_replica.phrase;
            }
            else
            {
                npc_name_go.SetActive(true);
                npc_text.text = cur_replica.phrase;
            }
            
            if (cur_replica.phrase.Length > 30)
                cur_pause_between_phrases += 0.5f;

            cur_phrase_start_time = Time.time;
            while (Time.time < cur_phrase_start_time + cur_pause_between_phrases && !phrase_skiped)
                yield return null;

            phrase_skiped = false;
        }

        having_dialog = false;
        game_man.block_hero_control(block_control: false);

        //show gameplay panels, hide dialog uia

        elapsed_time = 0f;

        while (elapsed_time < fade_duration)
        {
            elapsed_time += Time.deltaTime;
            dialog_panel_canvas.alpha = Mathf.Lerp(1, 0, elapsed_time / fade_duration);
            yield return null;
        }

        dialog_panel_go.SetActive(false);

        weapon_panel_go.SetActive(true);
        score_panel_go.SetActive(true);
        hero_hp_go.SetActive(true);

        elapsed_time = 0f;

        while (elapsed_time < fade_duration)
        {
            elapsed_time += Time.deltaTime;
            weapon_panel_canvas.alpha = Mathf.Lerp(0, 1, elapsed_time / fade_duration);
            score_panel_canvas.alpha = Mathf.Lerp(0, 1, elapsed_time / fade_duration);
            hero_hp_canvas.alpha = Mathf.Lerp(0, 1, elapsed_time / fade_duration);
            yield return null;
        }

        game_man.run_enemies();
    }

    public void end_dialog()
    {
        StopCoroutine(dialog_coroutine);

        dialog_panel_canvas.alpha = 0;
        dialog_panel_go.SetActive(false);

        weapon_panel_go.SetActive(true);
        score_panel_go.SetActive(true);
        hero_hp_go.SetActive(true);

        weapon_panel_canvas.alpha = 1;
        score_panel_canvas.alpha = 1;
        hero_hp_canvas.alpha = 1;

        having_dialog = false;
        game_man.block_hero_control(block_control: false);
    }

}
