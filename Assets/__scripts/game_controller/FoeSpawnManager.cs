using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoeSpawnManager : MonoBehaviour {

    [Header("Links")]
    public FoePool foe_pool;
    public Transform[] foe_inst_position;
    public GameObject hero;

    [Header("Spawn Vars")]
    public int max_foes_at_moment;
    public int total_foes_in_game;
    public float spawn_time_delta = 3f;

    private float last_spawn_time = 0;    
    [HideInInspector]
    public int foes_spawned_at_moment = 0;
    private int foes_spawned_in_game = 0;

    Transform cur_pos;
    int pos;

	void Update ()
    {
		if ((last_spawn_time + spawn_time_delta < Time.time) && (foes_spawned_at_moment < max_foes_at_moment) && (foes_spawned_in_game < total_foes_in_game))
        {
            spawn_foe(at_nearest : true);
            last_spawn_time = Time.time;
        }
	}

    void spawn_foe(bool at_nearest)
    {
        if (at_nearest)
        {
            pos = nearest_spawn_point_index();
        }
        else
        {
            pos = Random.Range(0, foe_inst_position.Length - 1);
        }
        cur_pos = foe_inst_position[pos];
        foe_pool.spawn_random_foe(cur_pos);
        //string s = "caterpillar_hair";
        //foe_pool.spawn_specific_foe(ref s, cur_pos);

        foes_spawned_at_moment++;
        foes_spawned_in_game++;
    }

    int nearest_spawn_point_index()
    {
        int n = foe_inst_position.Length;
        int min = 0;
        float cur_len = 0f, min_len = 999f;
        for(int i = 0; i < n; i++)
        {
            cur_len = Vector3.Distance(hero.transform.position, foe_inst_position[i].position);
            if (cur_len < min_len)
            {
                min_len = cur_len;
                min = i;
            }
        }

        return min;
    }
}
