﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class soldier_movement : MonoBehaviour {
    
    public float GroundCheckRadius = 0.01f;
    public float MoveForce = 1.0f;
    public float JumpForce = 10.0f;
    public Vector2 MaxSpeed = new Vector2(5.0f, 5.0f);
    public LayerMask GroundMask;

    Rigidbody2D self;
    CircleCollider2D feet;
    Animator anim;
    bool grounded = false;
    bool right = true;

    // Use this for initialization
    void Start () {
        self = GetComponent<Rigidbody2D>();
        feet = GetComponent<CircleCollider2D>();
        anim = GetComponent<Animator>();
    }
	
	// FixedUpdate is called upon a fixed amount of time passed
	void FixedUpdate () {
        // movements
        float fx = 0f;

        Vector2 feet_pos = self.position + feet.offset;
        feet_pos.y -= feet.radius;
        grounded = Physics2D.OverlapCircle(feet_pos, GroundCheckRadius, GroundMask);

        if (Math.Abs(self.velocity.x) < MaxSpeed.x)
            fx = Math.Sign(Input.GetAxis("Horizontal")) * MoveForce;
        Vector2 force = new Vector2(fx, 0f);
        self.AddForce(force, ForceMode2D.Impulse);

        anim.SetFloat("SpeedX", Math.Abs(self.velocity.x));
        anim.SetFloat("SpeedY", self.velocity.y);
        anim.SetBool("Grounded", grounded);

        // facing
        if (fx > 0 && !right)
            Flip();
        else if (fx < 0 && right)
            Flip();

        // debug
        Debug.Log(string.Format("vx = {0}, vy = {1}", self.velocity.x, self.velocity.y));
        Debug.Log(string.Format("x = {0}, y = {1}", feet_pos.x, feet_pos.y));
        Debug.Log(string.Format("grounded = {0}", grounded));
	}

    // Update is called once per frame
    void Update()
    {
        bool jump = Input.GetButtonDown("Jump");

        if (jump && grounded && Math.Abs(self.velocity.y) < MaxSpeed.y)
        {
            Vector2 force = new Vector2(0f, JumpForce);
            self.AddForce(force, ForceMode2D.Impulse);
        }

    }

        void Flip()
    {
        right = !right;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }
}