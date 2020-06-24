/*
https://docs.unity3d.com/Manual/SpriteAtlas.html

https://www.google.ru/search?newwindow=1&ei=BCA9W6aPDMGzswGI3pm4Ag&q=pixel+perfect+unity&oq=pixel+perfect+unity&gs_l=psy-ab.3..0j0i203k1l4j0i22i30k1l5.20689.21799.0.22422.6.6.0.0.0.0.141.557.5j1.6.0....0...1.1.64.psy-ab..0.6.555...0i67k1j0i20i263k1.0.ee_d_cnEs6E
https://www.youtube.com/watch?v=ZxKM18yY0ZM
https://docs.unity3d.com/ru/530/ScriptReference/Canvas-pixelPerfect.html
https://assetstore.unity.com/packages/tools/camera/pixel-perfect-camera-64563
https://forum.unity.com/threads/the-best-pixel-perfect-method.509323/
http://lakmus.nudl.net/blog/archives/49
https://hackernoon.com/making-your-pixel-art-game-look-pixel-perfect-in-unity3d-3534963cad1d
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class pixelperf : MonoBehaviour {

    //Vector2 displace_cam;
    //float width;
    Camera cam;
    public GameObject background_1, background_2;


    
    void Start()
    {
        //displace_cam = new Vector2(transform.position.x - .1f, transform.position.y - .1f);
        Debug.Log(gameObject.name);
        cam = GetComponent<Camera>();
        cam.orthographicSize = Screen.height / 2;
        //transform.position = displace_cam;

        //width = Screen.width;
        
	}

    private void Update()
    {
        GetComponent<Camera>().orthographicSize = Screen.height / 2;

        
        
    }

    void LateUpdate ()
    {
        //transform.position += displace_cam;
    }
}
