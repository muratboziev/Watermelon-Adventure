using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcBoxController : MonoBehaviour
{
    [Header("Params")]    
    public int particles_to_emit_hit;
    public int particles_to_emit_destroyed;

    [Header("Links")]
    public GameManager game_man;
    public GameObject normal_state;
    public GameObject destroyed_state;
    public GameObject evac_point;
    public ParticleSystem ps;
    public Collider2D vuln_area_collider;
    bool destroyed = false;

    int hp = 3;    

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!destroyed && collision.gameObject.CompareTag("weapon_melee"))
        {
            hit();
        }
    }

    public void hit()
    {
        if (!destroyed)
        {
            hp--;

            if (hp > 0)
            {
                ps.Emit(particles_to_emit_hit);
                game_man.audio_man.play_box_damaged();
            }
            else
            {
                destroy();
            }
        }
    }

    public void destroy(bool pre_destr=false)
    {
        hp = 0;        
        destroyed_state.SetActive(true);
        normal_state.SetActive(false);
        vuln_area_collider.enabled = false;
        destroyed = true;

        if (!pre_destr)
        {
            ps.Emit(particles_to_emit_destroyed);
            game_man.audio_man.play_box_destroyed();
        }       

        game_man.box_destroyed(pre_destr);        
    }


}
