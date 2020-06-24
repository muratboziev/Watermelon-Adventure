using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton : MonoBehaviour
{

    public static GameObject [] self = new GameObject [10];
    public static int num = 0;

    private void OnEnable()
    {
        num++;
     
        if (self[num] == null)
        {            
            self[num] = this.gameObject;
            DontDestroyOnLoad(gameObject);
            //Debug.Log("ddol " + gameObject.name);
        }
        else if (self[num] != this)
        {
            //Debug.Log("destr " + gameObject.name);
            Destroy(gameObject);                
        }
    }


}
