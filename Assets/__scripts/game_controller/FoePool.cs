using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class FoePrefabInfo
{
    public string name;
    public int hp;
    public GameObject foe_gameObject;
}

public class FoePool : MonoBehaviour
{
    //list of foes prefabs    
    public FoePrefabInfo[] foe_prefab;

    [Header("Links")]
    public WeaponManager weap_man;
    public FoeSpawnManager spawn_Manager;
    public GUIManagerHPScore gui_manager;

    [Header("Instantiation info")]    
    public int num_of_foe_to_inst = 1;              //number of foes to instantiate if foe stack is empty    

    //словарь стеков инстанциированных но не активированных врагов - строка : стек инстанцированных gameobject
    public Dictionary<string, Stack<GameObject>> foe_stack_dict = new Dictionary<string, Stack<GameObject>>();

    //словарь видов врагов - строка : foe_prefab_info
    public Dictionary<string, FoePrefabInfo> foe_data_dict = new Dictionary<string, FoePrefabInfo>();


    //временные переменные
    GameObject cur_go;
    FoeTakeDamage foe_take_dam;
    TakeDamageFromMeleeWeapon damage_from_weapon;

    
    void Start ()
    {
        foe_stack_dict = new Dictionary<string, Stack<GameObject>>();
        foe_data_dict = new Dictionary<string, FoePrefabInfo>();

        //первоначальное создание врагов каждого вида и их добавление в стеки
        Stack<GameObject> cur_stack;
        for (int i = 0; i < foe_prefab.Length; i++)
        {
            cur_stack = new Stack<GameObject>();
            instantiate_and_add_to_stack(cur_stack, foe_prefab[i].foe_gameObject, num_of_foe_to_inst);

            foe_stack_dict.Add(foe_prefab[i].name, cur_stack);
            foe_data_dict.Add(foe_prefab[i].name, foe_prefab[i]);            
        }
	}

    void instantiate_and_add_to_stack(Stack<GameObject> stack, GameObject foe_type, int count)
    {
        for (int i = 0; i < count; i++)
        {
            cur_go = Instantiate(foe_type);
            cur_go.transform.position = Vector3.zero;

            foe_take_dam = cur_go.GetComponent<FoeTakeDamage>();
            //foe_take_dam.foe_pool = this;
            foe_take_dam.weap_man = weap_man;
            //foe_take_dam.gui_manager = gui_manager;

            damage_from_weapon = cur_go.GetComponentInChildren<TakeDamageFromMeleeWeapon>();            
            stack.Push(cur_go);
        }
    }

    public void spawn_specific_foe(ref string species, Transform position)
    {
        if (foe_stack_dict[species].Count > 0)
        {
            cur_go = foe_stack_dict[species].Pop();
        }
        else
        {
            instantiate_and_add_to_stack(foe_stack_dict[species], foe_data_dict[species].foe_gameObject, num_of_foe_to_inst);
            cur_go = foe_stack_dict[species].Pop();
        }

        cur_go.transform.position = position.position;
        cur_go.SetActive(true);
        cur_go.GetComponent<FoeBase>().start_attack();
    }

    public void return_foe(ref string species, GameObject go, FoeTakeDamage vuln_area)
    {        
        //отключаем сам объект врага
        go.SetActive(false);

        //включаем его компоненты на случай последующего использования
        //vuln_area.foe_rb2d.simulated = true;

        //возвращаем hp
        //vuln_area.hp = foe_data_dict[species].hp;

        //возвращаем его в стек врагов соответствующего типа
        foe_stack_dict[species].Push(cur_go);
        spawn_Manager.foes_spawned_at_moment--;
    }

    public void spawn_random_foe(Transform position)
    {
        int random_foe_num = Random.Range(0, foe_prefab.Length - 1);
        spawn_specific_foe(ref foe_prefab[random_foe_num].name, position);
    }

}
