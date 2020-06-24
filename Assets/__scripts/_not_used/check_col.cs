using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class check_col : MonoBehaviour {

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("trigger enter " + gameObject.name);       
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("collision enter " + gameObject.name);
    }


    private void OnTriggerStay2D(Collider2D collision)
    {
        Debug.Log("trigger stay " + gameObject.name);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        Debug.Log("collision stay " + gameObject.name);
        
    }

}
