using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class projectile_flying : MonoBehaviour {

    public bool Attached = false;
    public GameObject Shooter;

    Rigidbody2D self;

    // Use this for initialization
    void Start () {
        self = GetComponent<Rigidbody2D>();
        Attached = false;
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
        //Debug.Log(collision.gameObject.ToString());
        Attached = true;
        self.simulated = false;

        if (Shooter != null)
        {
            ODM script = Shooter.GetComponent<ODM>();
            if (script != null)
                script.OnAttached(gameObject);
        }
    }

}
