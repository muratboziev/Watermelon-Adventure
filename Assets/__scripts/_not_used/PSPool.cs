using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class go_with_ps
{
    public ParticleSystem ps;
    public GameObject go;

    public go_with_ps(GameObject game_obj)
    {
        go = game_obj;
        ps = go.GetComponent<ParticleSystem>();
    }
}

public class PSPool : MonoBehaviour
{
    public float ps_time;
    WaitForSeconds ps_alive;
    public GameObject killedInsPS, woundedInsPS;

    Stack<go_with_ps> killedInsPS_Pool, woundedInsPS_Pool;
    go_with_ps ps_go;
    ParticleSystem particle_s;

    // Use this for initialization
    void Start ()
    {
        ps_alive = new WaitForSeconds(ps_time);

        killedInsPS_Pool = new Stack<go_with_ps>();
        woundedInsPS_Pool = new Stack<go_with_ps>();

        ps_go = new go_with_ps(Instantiate(killedInsPS));
        killedInsPS_Pool.Push(ps_go);
        ps_go = new go_with_ps(Instantiate(woundedInsPS));
        woundedInsPS_Pool.Push(ps_go);        
    }	

    public void set_wounded_ps(Transform pos, Color col)
    {
        if (killedInsPS_Pool.Count == 0)
        {
            ps_go = new go_with_ps(Instantiate(killedInsPS));
            killedInsPS_Pool.Push(ps_go);            
        }

        //poping ps from stack and setting its position
        ps_go = killedInsPS_Pool.Pop();
        ps_go.go.transform.position = pos.position;

        //setting ps color
        particle_s = ps_go.ps;
        var main = particle_s.main;
        main.startColor = col;                

        //emmiting particles
        ps_go.go.SetActive(true);
        particle_s.Play();

        StartCoroutine(deact_ps(ps_go));
    }

    public void release_wounded_ps(go_with_ps ps)
    {
        ps.go.SetActive(false);
        killedInsPS_Pool.Push(ps);
    }

    IEnumerator deact_ps(go_with_ps go)
    {
        yield return ps_alive;
        release_wounded_ps(go);
    }

}
