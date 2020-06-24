using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoeInfo
{

    public bool walks;
    public bool flies;
    public bool idles;

    public float pause_wf;
    public float pause_fw;
    public float pause_before_turn;
    public float pause_after_turn;

    public float walk_speed;
    public float walk_distance;
    public float fly_speed;
    public float landing_delta;

    public bool grounded;
    public bool grounded_prev;

    public bool facing_right;


    public FoeInfo()
    {

    }
}
