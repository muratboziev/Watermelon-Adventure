using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TakeDamageFromBody : MonoBehaviour
{

    public FoeTakeDamage foe_take_damage;
    string player_tag = "Player";    

    private void OnCollisionEnter2D(Collision2D collision)      //hit by body (but not when rolling)    
    {        
        if (collision.gameObject.CompareTag(player_tag))
        {
            foe_take_damage.get_damage(collision.gameObject, by_jump : true);            
        }
    }

}
