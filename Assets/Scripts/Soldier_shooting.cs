using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Soldier_shooting : MonoBehaviour {
    public GameObject ProjectilePrefab;
    public float FirePower = 10.0f;

    List<GameObject> projectiles = new List<GameObject>();
    Rigidbody2D self;

	// Use this for initialization
	void Start () {
        self = GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetButtonDown("Fire1"))
        {
            GameObject proj = Instantiate<GameObject>(ProjectilePrefab, transform.position, Quaternion.identity);
            Rigidbody2D rb = proj.GetComponent<Rigidbody2D>();
            rb.velocity = self.velocity;

            Vector2 force = new Vector2(FirePower, 0f);
            rb.AddForce(force, ForceMode2D.Impulse);
            projectiles.Add(proj);
        }
	}
}
