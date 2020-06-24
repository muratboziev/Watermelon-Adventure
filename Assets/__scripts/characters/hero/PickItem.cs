using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickItem : MonoBehaviour
{

    public HeroController hero_cont;
    string pickable_weapon = "pickable_weapon";
    string pickable_dna = "pickable_dna";
    string pickable_dna_part = "pickable_dna_part";    
    string pickable_pill = "pickable_pill";

    private void OnTriggerEnter2D(Collider2D collision)
    {        
        if (collision.gameObject.CompareTag(pickable_weapon))
        {
            hero_cont.collision_with_weapon(collision.gameObject);
        }
        else if (collision.gameObject.CompareTag(pickable_dna))
        {
            hero_cont.collision_with_dna(collision.gameObject);
        }
        else if (collision.gameObject.CompareTag(pickable_dna_part))
        {
            hero_cont.collision_with_dna_part(collision.gameObject);
        }
        else if (collision.gameObject.CompareTag(pickable_pill))
        {
            hero_cont.collision_with_pill(collision.gameObject);
        }
    }


}
