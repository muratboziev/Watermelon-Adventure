using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroVulnerableArea : MonoBehaviour {

    public HeroController hero_cont;
    string foe_attack = "foe_attack_area";

    /*private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(foe_attack))
        {
            hero_cont.collision_with_foe(collision.gameObject);
        }
    }*/

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(foe_attack))
        {
            hero_cont.collision_with_foe(collision.gameObject);
        }
    }



}
