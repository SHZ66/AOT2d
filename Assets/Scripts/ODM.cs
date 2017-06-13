using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ODM : MonoBehaviour {
    public Camera PlayerCamera;
    public GameObject ProjectilePrefab;
    public float FirePower = 15.0f;
    public float SquareMaxRopeLength = 100.0f;
    public List<GameObject> Anchors = new List<GameObject>();
    public List<float> sqL0 = new List<float>();
    public float K = 1.0f;
    public float RopeChangeRate = 0.05f;

    int last_anchor = 0;
    const int NUM_ANCHORS = 2;

    Rigidbody2D rb;
    Animator anim;
    LineRenderer rope_renderer;

    // Use this for initialization
    void Start () {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        rope_renderer = GetComponent<LineRenderer>();
        Material whiteDiffuseMat = new Material(Shader.Find("Unlit/Texture"));
        rope_renderer.material = whiteDiffuseMat;
        rope_renderer.material.color = Color.black;
        rope_renderer.startColor = Color.black;
        rope_renderer.endColor = Color.black;

        // initiate two anchors
        for (int i = 0; i < NUM_ANCHORS; i++)
        {
            GameObject anchor = Instantiate<GameObject>(ProjectilePrefab);
            anchor.GetComponent<SpriteRenderer>().enabled = false;
            anchor.GetComponent<Rigidbody2D>().simulated = false;
            anchor.GetComponent<projectile_flying>().Shooter = gameObject;
            Anchors.Add(anchor);

            sqL0.Add(0f);
        }
    }
	
	// Update is called once per frame
	void Update () {
        rope_renderer.SetPosition(0, transform.position);
        rope_renderer.SetPosition(1, transform.position);
        rope_renderer.SetPosition(2, transform.position);

        for (int i = 0; i < Anchors.Count; i++)
        {
            GameObject proj = Anchors[i];
            Rigidbody2D proj_rb = proj.GetComponent<Rigidbody2D>();

            // determine if a projectile is out of range
            if (proj_rb.simulated)
            {
                Vector2 dpos = proj.transform.position - transform.position;

                if (dpos.SqrMagnitude() > SquareMaxRopeLength)
                {
                    proj.GetComponent<SpriteRenderer>().enabled = false;
                    proj_rb.simulated = false;
                }
            }

            // render rope
            if (proj.GetComponent<projectile_flying>().Attached)
            {
                int j;
                if (i == 0) j = 0; else j = 2;
                rope_renderer.SetPosition(j, proj.transform.position);
            }
        }

        if (Input.GetButtonDown("Fire2"))
        {
            Vector2 objPos = transform.position; 
            Vector2 mousePos = Input.mousePosition; 
            mousePos = PlayerCamera.ScreenToWorldPoint(mousePos);
            Vector2 direction = mousePos - objPos;
            Quaternion rotation = Quaternion.FromToRotation(Vector3.right, direction);

            // initialize the new anchor
            GameObject anchor = Anchors[getNextAnchor()];
            anchor.transform.position = transform.position;
            anchor.transform.rotation = rotation;
            anchor.GetComponent<SpriteRenderer>().enabled = true;
            anchor.GetComponent<projectile_flying>().Attached = false;
            Rigidbody2D proj_rb = anchor.GetComponent<Rigidbody2D>();
            proj_rb.velocity = rb.velocity;
            proj_rb.simulated = true;
            Vector2 force = direction.normalized * FirePower;
            proj_rb.AddForce(force, ForceMode2D.Impulse);
 
            anim.SetBool("ODMing", true);
        }
	}

    private void FixedUpdate()
    {
        // update ODM forces
        for (int i = 0; i < Anchors.Count; i++)
        {
            GameObject anchor = Anchors[i];
            if (!anchor.GetComponent<projectile_flying>().Attached)
                continue;

            Vector2 anchor_pos = anchor.transform.position;
            Vector2 soldier_pos = transform.position;
            Vector2 dPos = anchor_pos - soldier_pos;
            float sqL = dPos.sqrMagnitude;

            if (sqL > sqL0[i])
            {
                float L = (float) Math.Sqrt(sqL);
                float L0 = (float)Math.Sqrt(sqL0[i]);

                float force = K * (L-L0);
                rb.AddForce(force * dPos.normalized, ForceMode2D.Impulse);
            }
        }
    }

    private void OnDestroy()
    {
        Destroy(rope_renderer.material);
    }

    int getNextAnchor()
    {
        int next_anchor = last_anchor + 1;
        if (next_anchor >= NUM_ANCHORS)
            next_anchor = 0;
        return next_anchor;
    }

    public void OnAttached(GameObject projectile)
    {
        last_anchor = getNextAnchor();
        sqL0[last_anchor] = (Anchors[last_anchor].transform.position - transform.position).sqrMagnitude;
        //Debug.Log(string.Format("Last anchor = {0}", last_anchor));
    }
}
