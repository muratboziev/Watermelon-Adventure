using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GaussGunController : MonoBehaviour
{
    //эти поля нужны только для оружия gauss_gun. у остальных видов оружия они есть но не заполняются
    
    public GameObject ray_origin;    
    public WaitForSeconds wait_before_erase_gauss_line = new WaitForSeconds(0.1f);
    public float laser_ray_length = 399f;

    LineRenderer line_renderer;

    Ray2D ray = new Ray2D();
    int enemy_layer_mask;
    string foe_vuln_area = "foe_vuln_area_weapon";
    float direction;

    private void Start()
    {
        line_renderer = GetComponent<LineRenderer>();
        enemy_layer_mask = 1 << LayerMask.NameToLayer("Enemy");
    }

    public void attack()
    {        
        StartCoroutine(gauss_gun_attack_coroutine());
    }

    private IEnumerator gauss_gun_attack_coroutine()
    {
        //определяем точку начала луча оружия
        Vector3 ray_start_position = ray_origin.transform.position;
        Vector3 ray_end_position;

        //определяем направление луча по разнице координат оружия и точки начала луча
        direction = Mathf.Sign(ray_start_position.x - gameObject.transform.position.x);

        //определяем точку конца луча оружия
        ray.origin = ray_start_position;
        ray.direction = Vector2.right;
        ray_end_position = ray.GetPoint(laser_ray_length * direction);

        //рисуем луч задав его начальную и конечную точки
        line_renderer.positionCount = 2;
        line_renderer.SetPosition(0, ray_start_position);
        line_renderer.SetPosition(1, ray_end_position);

        //находим объекты с которыми луч столкнулся и передаем им урон
        RaycastHit2D[] rch2d = Physics2D.LinecastAll(ray_start_position, ray_end_position, enemy_layer_mask);

        foreach (RaycastHit2D rc in rch2d)
        {
            Debug.Log(rc.collider.gameObject.name);
        }

        Collider2D c2d;
        FoeTakeDamage fd;
        NpcBoxController bd;
        GameObject c2d_go;

        foreach (RaycastHit2D i in rch2d)
        {            
            c2d = i.collider;
            c2d_go = c2d.gameObject;
            if (c2d.isTrigger && c2d_go.CompareTag(foe_vuln_area))
            {
                fd = c2d_go.GetComponentInParent<FoeTakeDamage>();
                if (fd != null)
                {                    
                    fd.get_damage(gameObject, distance: i.distance);
                }
                else
                {
                    bd = c2d_go.GetComponentInParent<NpcBoxController>();
                    bd.hit();
                }
            }
        }

        //оставляем луч нарисованным на заданное время
        yield return wait_before_erase_gauss_line;

        //стираем луч переместив конечную точку к начальной
        line_renderer.SetPosition(1, ray_start_position);

        yield return null;
    }

}
