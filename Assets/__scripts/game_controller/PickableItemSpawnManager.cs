using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PickableItemPrefabData
{
    public string name;    
    public GameObject prefab;    
    public int initial_count;
    public int reinstantiate_number;
    public Stack<GameObject> stack;
}

public class PickableItemSpawnManager : MonoBehaviour
{
    public PickableItemPrefabData [] pickable_dna;
    public GameObject dna_parts_parent_go;

    //словарь стеков инстанциированных но не активированных pickable_dna
    public Dictionary<string, PickableItemPrefabData> pickable_dna_dict = new Dictionary<string, PickableItemPrefabData>();

    public void instantiate_dna_parts()
    {
        pickable_dna_dict.Clear();

        for (int i = 0; i < pickable_dna.Length; i++)
        {
            pickable_dna[i].stack = new Stack<GameObject>();
            instantiate_and_add_to_stack(pickable_dna[i], pickable_dna[i].initial_count);
            pickable_dna_dict.Add(pickable_dna[i].name, pickable_dna[i]);
        }
    }

    void instantiate_and_add_to_stack(PickableItemPrefabData pickable_data, int count)
    {
        GameObject cur_go;

        for (int i = 0; i < count; i++)
        {
            cur_go = Instantiate(pickable_data.prefab);
            cur_go.transform.position = Vector3.zero;
            cur_go.transform.SetParent(dna_parts_parent_go.transform);
            cur_go.SetActive(false);

            pickable_data.stack.Push(cur_go);
        }
    }

    GameObject spawn_item(PickableItemPrefabData pickable_data, Transform position)
    {
        GameObject cur_go;

        cur_go = pickable_data.stack.Pop();

        if (pickable_data.stack.Count == 0)
        {        
            instantiate_and_add_to_stack(pickable_data, pickable_data.reinstantiate_number);            
        }

        cur_go.transform.position = position.position;
        cur_go.SetActive(true);

        return cur_go;
    }

    public GameObject spawn_dna(Transform position, int num)
    {
        string name = pickable_dna[num].name;
        return spawn_item(pickable_dna_dict[name], position);
    }

    public void return_dna_part_to_stack(GameObject dna_part_to_return)
    {
        dna_part_to_return = dna_part_to_return.transform.parent.gameObject;
        dna_part_to_return.SetActive(false);
        string name = dna_part_to_return.name.Replace("(Clone)", string.Empty);
        pickable_dna_dict[name].stack.Push(dna_part_to_return);
    }

}
