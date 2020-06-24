using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PSCollisionDetection : MonoBehaviour
{
    FoeTakeDamage foe_tack_damage;
    NpcBoxController box_controller;
    

    string tag_enemy = "Enemy";
    string tag_box = "box";

    void OnParticleCollision(GameObject other)
    {        
        //Debug.Log(other.gameObject.name);
        if (other.CompareTag(tag_enemy))
        {
            foe_tack_damage = other.GetComponentInParent<FoeTakeDamage>();            
            foe_tack_damage.get_damage(gameObject, by_particle:true);
        }
        else if (other.CompareTag(tag_box))
        {
            box_controller = other.GetComponentInParent<NpcBoxController>();
            box_controller.hit();
        }
            

    }



}
