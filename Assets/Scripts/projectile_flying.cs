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
}
