using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DnaController : MonoBehaviour
{
    public PlayerPoints player_progress;
    public GameObject dna_sprite;
    public ParticleSystem ps;               //ps is on player
    
    public int particles_to_emit;
    string player_tag = "Player";

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(player_tag))
        {
            dna_sprite.SetActive(false);

            player_progress.evo_points_gained_level += 1;
            player_progress.is_dna_found_level = true;

            ps.Emit(particles_to_emit);
            
            gameObject.SetActive(false);
        }
    }
}
