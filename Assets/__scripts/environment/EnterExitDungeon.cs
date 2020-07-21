using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnterExitDungeon : MonoBehaviour
{

    bool mobile_input;
    int cave_layer_mask;
    Camera camera;
    
    Vector2 click_pos_2d = new Vector2();
    Vector3 touch_pos = new Vector3();
    List<Vector3> touch_pos_list = new List<Vector3>();

    bool click_condition = false;

    string gui_text;

    GUIStyle guiStyle = new GUIStyle();

    void Start()
    {
        guiStyle.fontSize = 50;

        camera = FindObjectOfType<Camera>() as Camera;
        cave_layer_mask = 1 << LayerMask.NameToLayer("Dungeon_InOut");
        mobile_input = (Application.platform == RuntimePlatform.Android);
    }

    
    void Update()
    {
        click_condition = false;

#if UNITY_EDITOR
#endif
        if (mobile_input == false && Input.GetMouseButtonDown(0))
        {
            click_condition = true;
            touch_pos_list.Add(Input.mousePosition);
        }
        else if (mobile_input == true && Input.touchCount > 0)
        {
            click_condition = true;
            foreach (Touch t in Input.touches)
            {
                touch_pos_list.Add(t.position);
            }
        }

        if (click_condition)
        {
            foreach (Vector3 cur_touch_pos in touch_pos_list)
            {
                touch_pos = camera.ScreenToWorldPoint(cur_touch_pos);

                click_pos_2d.x = touch_pos.x;
                click_pos_2d.y = touch_pos.y;

                RaycastHit2D hit = Physics2D.Raycast(click_pos_2d, Vector2.zero, 100, cave_layer_mask);
                if (hit.collider != null)
                {
                    Debug.Log(hit.collider.gameObject.name);
                    gui_text = hit.collider.gameObject.name;
                }
                else
                    gui_text = "nope";
            }

            touch_pos_list.Clear();
        }

    }

    private void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 100, 20), gui_text, guiStyle);
    }


}
