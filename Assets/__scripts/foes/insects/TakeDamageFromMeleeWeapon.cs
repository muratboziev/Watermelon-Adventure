using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TakeDamageFromMeleeWeapon : MonoBehaviour  //take_damage_from_melee_weapon because dist attack go throw pscollisiondetection.cs
{
    public FoeTakeDamage ftd;    
    //string weapon_tag_dist = "weapon_dist";
    string weapon_tag_melee = "weapon_melee";

    private void OnTriggerEnter2D(Collider2D collision)     //hit by melee weapon  (and rolling)
    {
        bool melee_attack = collision.gameObject.CompareTag(weapon_tag_melee);
        if (//collision.gameObject.CompareTag(weapon_tag_dist) || 
            melee_attack)
        {            
            ftd.get_damage(collision.gameObject, by_melee:true);
        }
    }

    public FoeTakeDamage get_foe_take_damage()
    {
        return ftd;
    }

}
