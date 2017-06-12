using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class projectile_flying : MonoBehaviour {

    Rigidbody2D self;

    // Use this for initialization
    void Start () {
        self = GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void Update () {
        transform.rotation = Quaternion.FromToRotation(Vector3.right, self.velocity);
	}

    private void FixedUpdate()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer != LayerMask.NameToLayer("Player"))
            if (collision.gameObject.layer != LayerMask.NameToLayer("Projectile"))
                self.simulated = false;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer != LayerMask.NameToLayer("Player"))
            if (collision.gameObject.layer != LayerMask.NameToLayer("Projectile"))
                self.simulated = false;
    }
}
