using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyGameObjectOnTimer : MonoBehaviour {

    public float time_before_destroy = 2;    

    void Start ()
    {
        //float time_before_destroy = gameObject.GetComponent<ParticleSystem>().main.startLifetime.constant;
        //float time_before_destroy = gameObject.GetComponent<AudioSource>().clip.length;        
    }

    public void DestroyGO()
    {
        Invoke("DestroyGameObject", time_before_destroy);
    }

    void DestroyGameObject()
    {
        Destroy(gameObject);        
    }
}
