using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class deactivate_ps : MonoBehaviour {

    public float duration = 2;
    public PSPool ps_pool;

    void Start ()
    {        
        //float durationOfCollectedParticleSystem = gameObject.GetComponent<ParticleSystem>().main.startLifetime.constant;
        Invoke("DeactivateParticleSystem", duration);
    }

    void DeactivateParticleSystem()
    {
        //Destroy(gameObject);
        //ps_pool.release_wounded_ps(gameObject);
    }
}
