using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnterExitDungeon : MonoBehaviour
{
    [Header("Links")]
    public GameManager game_man;
    
    [Header("Links")]
    public int dungeon_num;
    public EnterExitDungeon linked_dungeon;    
    public GameObject enter_exit_pos;

    private bool mobile_input;
    private int cave_layer_mask;
    private Camera main_camera;
    private string player_tag = "Player";
    private bool player_is_in = false;
    
    Vector2 click_pos_2d = new Vector2();
    Vector3 touch_pos = new Vector3();
    //List<Vector3> touch_pos_list = new List<Vector3>();

    bool click_condition = false;

    string gui_text, gui_text_2="0";

    GUIStyle guiStyle = new GUIStyle();

    void Start()
    {
        guiStyle.fontSize = 50;

        main_camera = FindObjectOfType<Camera>() as Camera;
        cave_layer_mask = 1 << LayerMask.NameToLayer("Dungeon_InOut");
        mobile_input = (Application.platform == RuntimePlatform.Android);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag.Equals(player_tag))
        {
            player_is_in = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag.Equals(player_tag))
        {            
            player_is_in = false;
        }
    }

    void Update()
    {
        click_condition = false;

#if UNITY_EDITOR
#endif
        gui_text_2 = "touch began = 0";

        if (player_is_in)
        {
            if (mobile_input == false && Input.GetMouseButtonDown(0))
            {
                click_condition = true;
                touch_pos = Input.mousePosition;
                //touch_pos_list.Add(Input.mousePosition);
            }
            else if (mobile_input == true && Input.touchCount > 0)
            {

                int k = 0;

                foreach (Touch t in Input.touches)
                {
                    if (t.phase == TouchPhase.Began)
                    {
                        click_condition = true;
                        touch_pos = t.position;
                        k++;
                    }
                    //    touch_pos_list.Add(t.position);

                }

                gui_text_2 = "touch began = " + k.ToString(); ;
            }

            if (click_condition)
            {
                //foreach (Vector3 cur_touch_pos in touch_pos_list)
                //{
                touch_pos = main_camera.ScreenToWorldPoint(touch_pos);

                click_pos_2d.x = touch_pos.x;
                click_pos_2d.y = touch_pos.y;

                RaycastHit2D hit = Physics2D.Raycast(click_pos_2d, Vector2.zero, 100, cave_layer_mask);
                if (hit.collider != null)
                {
                    Debug.Log(hit.collider.gameObject.name);
                    gui_text = hit.collider.gameObject.name + " " + click_pos_2d;

                    game_man.menu_man.cur_menu_state = game_man.menu_man.make_transition(game_man.menu_man.cur_menu_state.transition[3]);
                }
                else
                    gui_text = "nope" + " " + click_pos_2d;
                //}

                //touch_pos_list.Clear();
            }
            //else
            //    gui_text = "no click";
        }

    }

    private void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 100, 20), gui_text, guiStyle);
        GUI.Label(new Rect(10, 80, 100, 20), gui_text_2, guiStyle);
    }


}
