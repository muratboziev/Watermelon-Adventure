using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class move_bg : MonoBehaviour {

    public Material mat;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        mat.mainTextureOffset = new Vector2(Time.time / -5, 0);
	}
}
