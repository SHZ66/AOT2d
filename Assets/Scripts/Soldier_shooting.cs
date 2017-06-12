using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class soldier_shooting : MonoBehaviour {
    public Camera PlayerCamera;
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
            Vector2 objPos = transform.position; 
            Vector2 mousePos = Input.mousePosition; 
            mousePos = PlayerCamera.ScreenToWorldPoint(mousePos);
            Vector2 direction = mousePos - objPos;

            Quaternion rotation = Quaternion.FromToRotation(Vector3.right, direction);
            GameObject proj = Instantiate<GameObject>(ProjectilePrefab, transform.position, rotation);
            Rigidbody2D rb = proj.GetComponent<Rigidbody2D>();
            rb.velocity = self.velocity;

            Vector2 force = direction.normalized * FirePower;
            rb.AddForce(force, ForceMode2D.Impulse);
            projectiles.Add(proj);
        }
	}
}
