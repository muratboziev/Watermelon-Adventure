using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombainManager : MonoBehaviour {

    public GameObject grabber;
    public float rotation_speed = 250f;

    public GameObject killEffectParticleSystem;
    public Transform ps_instPos;

    private Rigidbody2D rb2d;
    public float moveSpeed = -40f;
    Vector2 move_speed;

    // Use this for initialization
    void Start ()
    {
        rb2d = GetComponent<Rigidbody2D>();
        move_speed = new Vector2(moveSpeed, rb2d.velocity.y);
    }
	
	// Update is called once per frame
	void FixedUpdate()
    {
        rb2d.velocity = move_speed;
        grabber.transform.Rotate(0, 0, Time.deltaTime* rotation_speed);		
	}



    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            GameObject ps = Instantiate(killEffectParticleSystem, ps_instPos);
            //ps.transform.parent = null;
            //if (particle_mat)
            //    ps.GetComponent<ParticleSystem>().GetComponent<Renderer>().material = particle_mat;

            ParticleSystem particle_s = ps.GetComponent<ParticleSystem>();
            var start_color = particle_s.main.startColor;


            ps.SetActive(true);

        }
    }

}
