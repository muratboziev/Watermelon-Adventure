using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class snap : MonoBehaviour {

    public float PPU = 1; // pixels per unit (your tile size)
    Vector3 position;

    private void LateUpdate()
    {

        position = transform.position;
        position.x = (Mathf.Round(transform.position.x * PPU) / PPU);// - transform.position.x;
        position.y = (Mathf.Round(transform.position.y * PPU) / PPU);// - transform.position.y;

        transform.position = position;
    }
}
