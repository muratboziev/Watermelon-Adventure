using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class FoeTakeDamage : MonoBehaviour
{
    public int hp_init;
    public int hp_left;
    public string cur_species;

    [Header("Links")]
    public GameManager game_man;
    public WeaponManager weap_man;

    //object to deactivate after foe is killed        
    [Header("Deactivate after death")]
    public GameObject foe_attack_area;
    public GameObject foe_vulnerable_area;        
    public GameObject foe_body;
    public Rigidbody2D foe_rb2d;
    private FoeBase foe_base;

    //variables related to particle systems 
    [Header("Particle systems")]
    public Color ps_damage_color;
    public ParticleSystem ps_insect_wounded, ps_insect_killed;
    public int wounded_ps_to_emit = 8;
    public int kill_ps_to_emit = 8;
    WaitForSeconds time_before_deactivating_foe;

    //damage and impact received from hero    
    int damage_rate = 0;
    Impact impact_rate;

    private void Start()
    {
        hp_left = hp_init;

        foe_base = gameObject.GetComponent<FoeBase>();
        foe_rb2d = gameObject.GetComponent<Rigidbody2D>();
        time_before_deactivating_foe = new WaitForSeconds(ps_insect_killed.main.startLifetime.constant);

        //задае цвет particle system получения урона
        ps_damage_color = new Color(ps_damage_color.r, ps_damage_color.g, ps_damage_color.b);
        var ps_settings_main_section = ps_insect_wounded.main;
        ps_settings_main_section.startColor = ps_damage_color;        
    }

    public void get_damage(GameObject weapon, bool by_jump=false, bool by_melee=false, bool by_particle=false, float distance = -1f)
    {
        if (by_melee)// && !weap_man.weapon_name.Equals("roll") )
            weap_man.decrease_charges();


        if (by_jump)                    
        {
            impact_rate = WeaponManager.JUMP_IMPACT_ON_FOE;
            damage_rate = WeaponManager.JUMP_DAMAGE;
        }        
        else                            
        {
            impact_rate = weap_man.impact_on_foe;
            damage_rate = weap_man.damage_rate(distance);
        }

        if (by_particle && weap_man.num_of_particles_to_emit == 0)       //если частица коснулась противника после переключения на другое оружие
        {
            damage_rate = 20;
        }        

        hp_left -= damage_rate;        

        if (hp_left > 0)
        {
            foe_base.play_hit_sound();
            ps_insect_wounded.Emit(wounded_ps_to_emit);

            //impact_rate = ImpactType.damage(impact_rate, gameObject.transform, weapon.transform, foe_rb2d);            
            //StartCoroutine(ImpactType.push(foe_rb2d, impact_rate));
        }
        else
        {
            foe_base.play_killed_sound();            
            ps_insect_killed.Emit(kill_ps_to_emit);            
            StartCoroutine(inst_dna_parts());
            StartCoroutine(disable_foe());
        }        
    }

    public IEnumerator inst_dna_parts()
    {
        game_man.instantiate_dna_parts(foe_body.transform);
        yield return null;
    }

    public IEnumerator disable_foe()
    {
        foe_base.stop_attack();
        foe_base.stop_and_idle();
        foe_attack_area.SetActive(false);        
        foe_rb2d.simulated = false;        
        foe_vulnerable_area.SetActive(false);        
        foe_body.SetActive(false);        

        yield return time_before_deactivating_foe;                  //ждем чтобы ps сразу не исчезла

        //foe_pool.return_foe(ref cur_species, gameObject, this);
    }

}
