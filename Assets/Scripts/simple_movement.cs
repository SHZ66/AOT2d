using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class simple_movement : MonoBehaviour {
    
    public float GroundCheckRadius = 0.1f;
    public float MoveForce = 1.0f;
    public float JumpForce = 10.0f;
    public Vector2 MaxSpeed = new Vector2(5.0f, 5.0f);
    public LayerMask GroundMask;

    Rigidbody2D rb;
    CircleCollider2D feet;
    bool grounded = false;

    // Use this for initialization
    void Start () {
        rb = GetComponent<Rigidbody2D>();
        feet = GetComponent<CircleCollider2D>();
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        float fx = 0f;
        float fy = 0f;

        Vector2 feet_pos = rb.position;
        feet_pos.y -= feet.radius;
        grounded = Physics2D.OverlapCircle(feet_pos, GroundCheckRadius, GroundMask);

        if (Math.Abs(rb.velocity.x) < MaxSpeed.x)
            fx = Math.Sign(Input.GetAxis("Horizontal")) * MoveForce;
        if (grounded && Math.Abs(rb.velocity.y) < MaxSpeed.y)
            fy = Math.Sign(Input.GetAxis("Vertical")) * JumpForce;
        Vector2 force = new Vector2(fx, fy);

        Debug.Log(string.Format("fx = {0}, fy = {1}", fx, fy));
        Debug.Log(string.Format("vx = {0}, vy = {1}", rb.velocity.x, rb.velocity.y));
        Debug.Log(string.Format("x = {0}, y = {1}", feet_pos.x, feet_pos.y));
        Debug.Log(string.Format("grounded = {0}", grounded));
        rb.AddForce(force, ForceMode2D.Impulse);
	}
}
