using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct WeaponData
{
    public int damage_rate;
    public int max_charges;
    public int charges_left;
    public float weight;
    public bool needs_reload;
    public bool accumulates_charges;
    public bool distance_attack;

    public Impact impact_on_hero;
    public Impact impact_on_foe;
}

[System.Serializable]
public class Weapon
{
    public string weapon_name;
    public GameObject weapon_gameobj;

    [Header("Weapon data")]
    public WeaponData initial_weapon_data;                   //параметры оружия по умолчанию

    [Header("Sounds")]
    public AudioClip attack_sound;
    public AudioClip pickup_sound;
    public float attack_sound_delay;
    public bool turn_sound_off_after_attack;                                  //stop sound when attack button release
    public bool use_2nd_audiosource;                                  //insects use another audiosource for attack sound

    [Header("Animation params")]    
    public int anim_layer_index;
    public int animation_state;    
    
    [Header("Particle System")]
    public float emit_timeout;
    public int num_of_particles_to_emit;
    public ParticleSystem weapon_ps;

    [Header("Special weapons")]
    public GaussGunController gauss_gun;
    public AllyInsectController ally_insect;

    [HideInInspector]
    public bool reloaded;
    [HideInInspector]
    public WeaponData recalc_weapon_data;                    //пересчитанные параметры (после прокачки)

    //-------------------------------
    private float last_emit_time;
    private bool can_emit, attack_success;    

    private static string gauss_gun_str = "gauss_gun";
    private static string melee_weapons = "katana shoe";
    private static string weapons_with_ps = "insecticide flamethrower shotgun minigun";
    private static string insect_weapons = "ally_spider ally_dragonfly ally_praying_mantis";
    private static string roll_weapon = "roll";

    Weapon()
    {
        last_emit_time = 0f;
        can_emit = false;
        reloaded = true;
    }

    public void restore_charges(bool level_load=false)
    {
        if (level_load)
        {
            if (weapon_name.Equals("roll"))
                recalc_weapon_data.charges_left = 99999;
            else
                recalc_weapon_data.charges_left = 0;
        }
        else
        {
            if (recalc_weapon_data.accumulates_charges)
                recalc_weapon_data.charges_left += recalc_weapon_data.max_charges;
            else
                recalc_weapon_data.charges_left = recalc_weapon_data.max_charges;
        }
    }

    public void reload()
    {
        reloaded = true;
    }

    public bool attack()
    {
        attack_success = false;

        if (recalc_weapon_data.charges_left > 0)
        {
            if (weapons_with_ps.Contains(weapon_name))
            {
                can_emit = last_emit_time + emit_timeout < Time.time;
                if (can_emit)
                {
                    last_emit_time = Time.time;
                    weapon_ps.Emit(num_of_particles_to_emit);
                    attack_success = true;
                }
            }
            else if (gauss_gun_str.Contains(weapon_name))
            {                
                gauss_gun.attack();
                attack_success = true;
            }
            else if (insect_weapons.Contains(weapon_name))
            {
                attack_success = ally_insect.start_attack();                
            }
            else if (melee_weapons.Contains(weapon_name))
            {
                attack_success = true;
            }
            else if (roll_weapon.Contains(weapon_name))
            {
                attack_success = true;
            }

            reloaded = false;
        }

        return attack_success;
    }

}

public class WeaponManager : MonoBehaviour
{
    public HeroController hero_contr;
    public GUIManagerGameplay gameplay_gui_manager;
    public Animator hero_animator_controller;    
    public Weapon[] weapons;    

    //not seen in inspector
    public static int JUMP_DAMAGE = 40;
    public static Impact JUMP_IMPACT_ON_FOE = new Impact(0.2f, 300f, 0f);
    public static Impact foe_impact_on_hero = new Impact(0.5f, -400f, 20f);
    
    public Dictionary<string, Weapon> weapons_dict = new Dictionary<string, Weapon>();
    private Weapon current_weapon;
    private string new_weap_name;
    private bool attack_done = false;

    WaitForSeconds wait_before_weapon_switch = new WaitForSeconds(1f);
    bool weap_switch_completed = true;

    private void Start()
    {
        foreach (Weapon w in weapons)
        {
            weapons_dict.Add(w.weapon_name, w);
        }
    }
    //--------------------------------------------------------------------------------------------------------------

    public void take_away_weapons()
    {        

        foreach (Weapon w in weapons)
        {
            w.reloaded = true;              //непонятная ошибка - они становятся false хотя в конструкторе true
            w.restore_charges(level_load : true);            
        }

        //изначально в арсенале есть только roll
        current_weapon = weapons_dict["roll"];
    }

    //actions---------------------------------------------------------------------------------------------------------
    

    public void switch_weapon(string new_weapon_name)
    {
        if (!current_weapon.weapon_name.Equals(new_weapon_name))
        {
            hero_animator_controller.SetLayerWeight(current_weapon.anim_layer_index, 0);

            if (current_weapon.weapon_gameobj != null)          //если переключение не с roll
            {
                current_weapon.weapon_gameobj.SetActive(false);
            }

            current_weapon = weapons_dict[new_weapon_name];

            if (current_weapon.weapon_gameobj != null)          //если переключение не на roll      
                current_weapon.weapon_gameobj.SetActive(true);

            hero_animator_controller.SetLayerWeight(current_weapon.anim_layer_index, 1);

            hero_contr.weapon_switched();
        }
    }

    public void pick_weapon(GameObject weapon_picked)
    {        
        weapon_picked.SetActive(false);

        new_weap_name = weapon_picked.name;
        if (new_weap_name != current_weapon.weapon_name)            //подобрал не то же оружие что выбрано в данный момент
        {
            switch_weapon(new_weap_name);
        }
        else
        {
            hero_contr.play_weap_switched_sound();
        }

        current_weapon.restore_charges();
        gameplay_gui_manager.pick(ref new_weap_name);
    }

    public bool attack()
    {
        attack_done = current_weapon.attack();        
        if (attack_done && distance_attack)
            decrease_charges();
        return attack_done;
    }

    public void decrease_charges()
    {        
        current_weapon.recalc_weapon_data.charges_left--;
        
        gameplay_gui_manager.update_cell(null, false, true, weapon_name, charges_left);

        if (charges_left <= 0)
        {
            StartCoroutine(out_of_ammo(current_weapon.weapon_name));
        }
    }

    IEnumerator out_of_ammo(string weap_name)
    {
        weap_switch_completed = false;

        yield return wait_before_weapon_switch;

        weap_switch_completed = true;

        new_weap_name = gameplay_gui_manager.out_of_ammo(weap_name);
        switch_weapon(new_weap_name);
    }

    public void reload()
    {
        current_weapon.reload();
    }

    public int damage_rate(float gauss_distance = -1f)
    {
        if (gauss_distance == -1f)              //attack by any weapon            
            return current_weapon.recalc_weapon_data.damage_rate; //current_weapon.recalc_weapon_data.damage_rate;
        if (gauss_distance == -2f)              //attack by spider or dragonfly
            return 100;
        else                                    //attack by gauss_gun
        {
            int damage = current_weapon.recalc_weapon_data.damage_rate;
            if (gauss_distance < 10f)
            {
                return damage + 30;
            }
            else if (gauss_distance < 40f)
            {
                return damage + 20;
            }
            else if (gauss_distance < 80f)
            {
                return damage + 10;
            }
            else
            {
                return damage;
            }
        }
    }

    //properties-----------------------------------------------------------------------------------------------------

    public Impact impact_on_hero
    {
        get
        {            
            return current_weapon.recalc_weapon_data.impact_on_hero;
        }
    }

    public Impact impact_on_foe
    {
        get
        {
            return current_weapon.recalc_weapon_data.impact_on_foe;
        }
    }

    public string weapon_name
    {
        get
        {
            return current_weapon.weapon_name;
        }
    }

    public bool use_2nd_audiosource
    {
        get
        {
            return current_weapon.use_2nd_audiosource;
        }
    }

    public int anim_layer_index
    {
        get
        {
            return current_weapon.anim_layer_index;
        }
    }

    public int animation_state
    {
        get
        {
            return current_weapon.animation_state;
        }
    }

    public float weight
    {
        get
        {            
            return current_weapon.recalc_weapon_data.weight;
        }
    }

    public bool reloaded
    {
        get
        {
            return current_weapon.reloaded;
        }
    }

    public bool needs_reload
    {
        get
        {
            return current_weapon.recalc_weapon_data.needs_reload;
        }
    }

    public int charges_left
    {
        get
        {
            return current_weapon.recalc_weapon_data.charges_left;
        }
    }

    public int charges_left_of_weapon(string weapon_name)
    {
        return  weapons_dict[weapon_name].recalc_weapon_data.charges_left;
    }

    public int max_charges
    {
        get
        {
            return current_weapon.recalc_weapon_data.max_charges;
        }
    }

    public int max_charges_of_weapon(string weapon_name)
    {
        return weapons_dict[weapon_name].recalc_weapon_data.max_charges;
    }

    public AudioClip weapon_attack_sound
    {
        get
        {
            return current_weapon.attack_sound;
        }
    }

    public AudioClip weapon_pick_sound
    {
        get
        {
            return current_weapon.pickup_sound;
        }
    }

    public bool distance_attack
    {
        get
        {
            return current_weapon.recalc_weapon_data.distance_attack;
        }
    }

    public int num_of_particles_to_emit
    {
        get
        {
            return current_weapon.num_of_particles_to_emit;
        }
    }

    public bool weapon_switch_completed
    {
        get
        {
            return weap_switch_completed;
        }
    }

    public float attack_sound_delay
    {
        get
        {
            return current_weapon.attack_sound_delay;
        }
    }

    public bool turn_sound_off_after_attack
    {
        get
        {
            return current_weapon.turn_sound_off_after_attack;
        }
    }

    public void dectivate_cur_weapon()
    {
        if (!current_weapon.weapon_name.Equals("roll"))
            current_weapon.weapon_gameobj.SetActive(false);
    }

    public AudioClip cur_weapon_audio_clip
    {
        get
        {
            return current_weapon.attack_sound;
        }
    }

}
