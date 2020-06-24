using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PillController : MonoBehaviour
{
    public bool pill;
    public GUIManagerHPScore gui_man_hp;
    public HeroController hero_contr;
    public Animator take_pill_anim;    
    
    string player_tag = "Player";
    WaitForSeconds wait_for_take_pill;

    private void Start()
    {        
        wait_for_take_pill = new WaitForSeconds(1.4f);
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(player_tag))
        {
            if (hero_contr.recalc_hero_params.cur_hero_hp < hero_contr.recalc_hero_params.max_hero_hp || pill)
            {
                if (pill)
                    hero_contr.recalc_hero_params.cur_hero_hp = hero_contr.recalc_hero_params.max_hero_hp + 1;
                else
                    hero_contr.recalc_hero_params.cur_hero_hp += 1;

                gui_man_hp.update_player_hp_sprite(hero_contr.recalc_hero_params.cur_hero_hp, took_pill: pill);

                gameObject.transform.parent = hero_contr.gameObject.transform.Find("pill_position");
                gameObject.transform.localPosition = Vector3.zero;
                StartCoroutine(take_pill());
            }
        }
    }

    IEnumerator take_pill()
    {   
        take_pill_anim.Play("take_pill");
        yield return wait_for_take_pill;
        gameObject.SetActive(false);
        Destroy(gameObject);
    }


}
