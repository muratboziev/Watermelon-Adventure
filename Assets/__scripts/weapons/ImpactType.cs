using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Impact
{
    public float impact_time;
    public Vector2 impact_val;

    public Impact(float imp_time, float impact_hor, float impact_ver)
    {
        impact_time = imp_time;
        impact_val = new Vector2(impact_hor, impact_ver);
    }

    public Impact(Impact imp)
    {
        impact_time = imp.impact_time;
        impact_val = new Vector2(imp.impact_val.x, imp.impact_val.y);
    }

    public void set_impact_value(float imp_time, float impact_hor, float impact_ver)
    {
        impact_time = imp_time;
        impact_val.x = impact_hor;
        impact_val.y = impact_ver;
    }
}

public class ImpactType //: MonoBehaviour
{
    static Impact imp = new Impact(0, 0, 0);
    static float impact_time, impact_hor, impact_ver, sign;

    public static Impact recoil(Impact weapon_imp_on_hero, bool facingRight)
    {
        impact_time = weapon_imp_on_hero.impact_time;        
        impact_hor = weapon_imp_on_hero.impact_val.x * get_direction(facingRight);
        impact_ver = weapon_imp_on_hero.impact_val.y; 

        imp.set_impact_value(impact_time, impact_hor, impact_ver);
        return imp;
    }

    public static Impact damage(Impact weapon_imp_on_foe, Transform go_transform, Transform weapon_transform, Rigidbody2D foe_rb2d=null)
    {
        impact_time = weapon_imp_on_foe.impact_time;

        sign = Mathf.Sign(go_transform.position.x - weapon_transform.position.x);
        impact_hor = sign * weapon_imp_on_foe.impact_val.x;
        impact_ver = weapon_imp_on_foe.impact_val.y;

        if (foe_rb2d)                                   //если есть этот параметр то для расчета прилагаемой силы учитываем скорость объекта (используется только для врагов)
        {
                if (foe_rb2d.velocity.x * sign <= 0.2)    //враг идет на героя
                {
                    impact_hor = -1 * foe_rb2d.velocity.x * weapon_imp_on_foe.impact_val.x * 2;  //сила с которой толкает оружие = от 1 до 10. макс скорость насекомых 70-90
                    if (Mathf.Abs(impact_hor) < 1 || Mathf.Abs(foe_rb2d.velocity.y) > 0.5f)     //если враг почти стоит на месте или летит то прилагаем фиксированную силу
                    {
                        impact_hor = 250 * sign;
                    }                   
                    
                }
                else                                    //враг идет от героя
                {
                    //impact_hor = foe_rb2d.velocity.x * weapon_imp_on_foe.impact_val.x * 2;
                }            
        }

        imp.set_impact_value(impact_time, impact_hor, impact_ver);
        
        return imp;
    }

    static int get_direction(bool d)
    {
        if (d) return 1;
        return -1;
    }

    public static IEnumerator push(Rigidbody2D rb2d, Impact imp)
    {
        Impact imp_self = new Impact(imp);
        float finish_time = Time.timeSinceLevelLoad + imp.impact_time;

        while (Time.timeSinceLevelLoad < finish_time)
        {            
            rb2d.AddForce(imp_self.impact_val);            
            yield return null; 
        }

        yield return null;
    }
}



/*
 * 
 * 
 
    //толкане врага с остановкой его процедуры атаки. 

    public static IEnumerator push(Rigidbody2D rb2d, impact imp)
    {
        impact imp_self = new impact(imp);                                  //чтобы избежать изменения направления уже движущегося объекта при изменении стаитческой переменной impact_type.imp 
                                                                            //которая используется для хранения промежуточного результата расчета направления
        float finish_time = Time.timeSinceLevelLoad + imp.impact_time;

        
        foe_base foeattack = null;
        bool enemy = rb2d.gameObject.CompareTag("Enemy");
        bool stoped_foe = false;                                            //семафор того что враг остановлен этим потоком. 

        if (enemy)
        {
            foeattack = rb2d.gameObject.GetComponent<foe_base>();

            if (foeattack != null)
            {

                if (!foeattack.impact_semaphore)
                {
                    foeattack.impact_semaphore = true;                      //семафор того что враг был остановлен каким нибудь потоком
                    stoped_foe = true;

                    Debug.Log("stop");
                    foeattack.stop_attack();
                    rb2d.velocity = Vector2.zero;
                }
            }
        }
        
        while (Time.timeSinceLevelLoad < finish_time && stoped_foe)
        {            
            rb2d.AddForce(imp_self.impact_val);
            yield return null; 
        }

        if (enemy)
        {            
            if (foeattack != null && stoped_foe == true)
            {
                yield return new WaitForSeconds(0.1f);
                Debug.Log("start");
                foeattack.impact_semaphore = false;
                foeattack.start_attack();
            }
        }

        yield return null;
    }


 * 
 */
