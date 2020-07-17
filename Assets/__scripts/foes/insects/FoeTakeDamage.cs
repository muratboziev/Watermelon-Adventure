using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class FoeTakeDamage : MonoBehaviour
{
    public int hp_init;
    private int hp_left;
    [HideInInspector]
    public int ID;
    //public string cur_species;

    [Header("Links")]
    public GameManager game_man;
    public WeaponManager weap_man;
    public GameObject ps_parent, audiosources_parent, line_renderer;
    public DestroyGameObjectOnTimer ps_destroyer, audiosource_destroyer;

    //variables related to particle systems 
    [Header("Particle systems")]
    public Color ps_damage_color;
    public ParticleSystem ps_insect_wounded, ps_insect_killed;
    public int wounded_ps_to_emit = 8;
    public int kill_ps_to_emit = 8;
    
    private Rigidbody2D foe_rb2d;
    private FoeBase foe_base;

    //damage and impact received from hero    
    int damage_rate = 0;
    Impact impact_rate;

    private void Start()
    {
        hp_left = hp_init;

        foe_base = gameObject.GetComponent<FoeBase>();
        foe_rb2d = gameObject.GetComponent<Rigidbody2D>();        

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
            disable_foe();
        }        
    }

    public IEnumerator inst_dna_parts()
    {
        game_man.instantiate_dna_parts(gameObject.transform);
        yield return null;
    }

    public void disable_foe()
    {
        foe_base.stop_attack();
        foe_base.stop_audioSources();
        foe_rb2d.simulated = false;

        ps_parent.transform.parent = null;
        audiosources_parent.transform.parent = null;
        if (line_renderer != null)
            line_renderer.transform.parent = null;

        ps_destroyer.DestroyGO();
        audiosource_destroyer.DestroyGO();

        gameObject.SetActive(false);

        game_man.foe_controller.Remove(ID);

        Destroy(gameObject);        

        //foe_pool.return_foe(ref cur_species, gameObject, this);
    }

}
