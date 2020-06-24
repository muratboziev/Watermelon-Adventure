using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

[System.Serializable]
public class WeaponSprite
{
    public string weapon_name;
    public Sprite[] weapon_image;
    public bool show_charges_num;
}

[System.Serializable]
public class WeaponCell
{
    public Image image;
    public Image selected;
    public Text charges_left;    
}

public class GUIManagerGameplay : MonoBehaviour
{
    [Header("Links")]
    public WeaponManager weap_man;

    [Header("Weapons GUI elements")]    
    public Sprite default_cell_image;
    public Sprite selected_cell_image;
    public Button button_up, button_down;
    public WeaponCell[] weapons_cell;
    public WeaponSprite[] weapon_gui_image;

    //----------------------------------
    [HideInInspector]
    public Dictionary<string, WeaponSprite> weapImagesDict = new Dictionary<string, WeaponSprite>();

    [HideInInspector]
    public LinkedList<string> picked_weapons = new LinkedList<string>();
    [HideInInspector]
    public LinkedListNode<string> scroll_beginning, selected_cell;
    LinkedListNode<string> temp_cell;


    void OnEnable()
    {
        foreach (WeaponSprite im in weapon_gui_image)
        {
            weapImagesDict.Add(im.weapon_name, im);
        }
    }

    public void level_loaded()      //called when scene is loaded 
    {
        picked_weapons.Clear();
        scroll_beginning = picked_weapons.AddLast("roll");
        selected_cell = scroll_beginning;
        update_weapon_panel(scroll_beginning);
    }

    //-----------------------------------------------------------------------------------------

    public void pick(ref string weapon_name)
    {
        if (picked_weapons.Contains(weapon_name))
        {
            selected_cell = picked_weapons.Find(weapon_name); 
        }
        else
        {
            selected_cell = picked_weapons.AddLast(weapon_name);
        }
        scroll_beginning = get_scroll_beginning(selected_cell);
        update_weapon_panel(scroll_beginning);
    }

    public string out_of_ammo(string weap_to_remove)
    {
        if (weap_to_remove == selected_cell.Value)          //если сразу после окончания боеприпасов не было выбрано другое оружие
        {
            if (selected_cell.Next != null)
                selected_cell = selected_cell.Next;
            else if (selected_cell.Previous != null)
                selected_cell = selected_cell.Previous;
            else         
                selected_cell = null;            
        }

        picked_weapons.Remove(weap_to_remove);
        scroll_beginning = get_scroll_beginning(selected_cell);
        update_weapon_panel(scroll_beginning);

        if (selected_cell == null)
            return "roll";

        return selected_cell.Value;
    }

    //-----------------------------------------------------------------------------------------

    public void select_from_gui(int weapon_cell_id)
    {
        temp_cell = scroll_beginning;
        for (int i = 0; i < weapon_cell_id; i++)        //от первого показываемого оружия идем вниз на weapon_cell_id (берется из компонента Button)
        {
            if (temp_cell != null)
                temp_cell = temp_cell.Next;
            else
                break;
        }

        if (temp_cell != null)
        {
            selected_cell = temp_cell;
            update_weapon_panel(scroll_beginning);
            weap_man.switch_weapon(selected_cell.Value);
        }        
    }    

    public void scroll_up()
    {
        if (scroll_beginning.Previous != null)
        {
            scroll_beginning = scroll_beginning.Previous;
            update_weapon_panel(scroll_beginning);
        }
    }

    public void scroll_down()
    {
        temp_cell = scroll_beginning;

        for (int i = 0; i < weapons_cell.Length; i++)
        {
            if (temp_cell.Next != null)
            {
                temp_cell = temp_cell.Next;
            }
            else
            {
                return;
            }
        }

        scroll_beginning = scroll_beginning.Next;
        update_weapon_panel(scroll_beginning);        
    }

    //-----------------------------------------------------------------------------------------

    public void update_cell(WeaponCell cell, bool cell_is_empty, bool cell_is_selected, string weapon_name, int charges_left)
    {
        int weap_image_id = 0;
        WeaponSprite cur_weap_sprite;

        if (cell == null)                //при вызове после атаки ищем активную ячейку
        {
            temp_cell = scroll_beginning;
            for (int i = 0; i < weapons_cell.Length; i++)
            {
                if (temp_cell == selected_cell)
                {                    
                    cell = weapons_cell[i];                    
                    break;
                }

                if (temp_cell.Next != null)
                    temp_cell = temp_cell.Next;
                else
                    break;
            }
        }

        if (cell != null)                       //если ячейка в области видимости
        { 
            if (!cell_is_empty )                 //обновляем не пустую ячейку и если она в области видимости
            {
                weap_image_id = actual_weapon_image_id(weapon_name);

                cur_weap_sprite = weapImagesDict[weapon_name];
                cell.image.sprite = cur_weap_sprite.weapon_image[weap_image_id];
                
                if (cell_is_selected)
                {
                    cell.selected.sprite = selected_cell_image;
                }
                else
                {
                    cell.selected.sprite = default_cell_image;
                }

                if (cur_weap_sprite.show_charges_num)
                    cell.charges_left.text = charges_left.ToString();
                else
                   cell.charges_left.text = string.Empty;
            }
            else
            {
                cell.image.sprite = default_cell_image;
                cell.selected.sprite = default_cell_image;
                cell.charges_left.text = "";
            }
        }

    }

    public void update_weapon_panel(LinkedListNode<string> scroll_beg)
    {   
        bool cell_is_selected, cell_is_empty;
        int charges_left = 0;
        string weapon_name = "";

        //updating up button
        if (scroll_beg != picked_weapons.First)
            button_up.interactable = true;
        else
            button_up.interactable = false;


        //updating cells
        temp_cell = scroll_beg;        
        foreach (WeaponCell cur_cell in weapons_cell)
        {
            cell_is_empty = temp_cell == null;
            cell_is_selected = temp_cell == selected_cell;
            if (!cell_is_empty)
            {
                weapon_name = temp_cell.Value;
                charges_left = weap_man.charges_left_of_weapon(weapon_name);
            }

            update_cell(cur_cell, cell_is_empty, cell_is_selected, weapon_name, charges_left);

            if (temp_cell != null)
            {
                temp_cell = temp_cell.Next;
            }
        }

        //updating down button
        if (temp_cell != null)                 //temp_cell указывает на следующую за нижней из видимых ячеек
            button_down.interactable = true;
        else
            button_down.interactable = false;
    }

    //-----------------------------------------------------------------------------------------

    LinkedListNode<string> get_scroll_beginning(LinkedListNode<string> selected)        //от выбранного оружия идем наверх на кол-во ячеек (3) и т.о. находим первое отображаемое оружие
    {
        if (selected != null)
        {
            for (int i = 0; i < weapons_cell.Length - 1; i++)
            {
                if (selected.Previous != null)
                    selected = selected.Previous;
                else
                    break;
            }
        }

        return selected;
    }

    int actual_weapon_image_id(string weapon_name)
    {
        
        int charges_left_grade = weap_man.max_charges_of_weapon(weapon_name) / 3;
        int charges_left = weap_man.charges_left_of_weapon(weapon_name);
        
        if (charges_left >= charges_left_grade * 2 || weapon_name == "roll")
            return 0;

        if (charges_left >= charges_left_grade)
            return 1;        

        return 2;
    }

}
