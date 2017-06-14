using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vectrosity;

public class ODM : MonoBehaviour {
    public Camera PlayerCamera;
    public GameObject ProjectilePrefab;
    public float FirePower = 15.0f;
    public float SquareMaxRopeLength = 100.0f;
    public List<GameObject> Anchors = new List<GameObject>();
    public List<float> sqL0 = new List<float>();
    public float K = 1.0f;
    public float RopeChangeRate = 1f;
    public float RopeWidth = 1f;

    int last_anchor = 0;
    const int NUM_ANCHORS = 2;

    Rigidbody2D rb;
    Animator anim;
    //LineRenderer rope_renderer;
    VectorLine rope_renderer;

    // Use this for initialization
    void Start () {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        //rope_renderer = GetComponent<LineRenderer>();
        rope_renderer = new VectorLine("Rope", new List<Vector3>{ new Vector3(), new Vector3(), new Vector3() }, RopeWidth, LineType.Continuous);

        rope_renderer.Draw();
        VectorLine.canvas.renderMode = RenderMode.ScreenSpaceCamera;
        VectorLine.SetCanvasCamera(Camera.main);
        VectorLine.canvas.sortingLayerName = "Projectile";

        //Debug.Log(string.Format("{0}", VectorLine.canvas.sortingLayerName));

        // initiate two anchors
        for (int i = 0; i < NUM_ANCHORS; i++)
        {
            GameObject anchor = Instantiate<GameObject>(ProjectilePrefab);
            anchor.GetComponent<SpriteRenderer>().enabled = false;
            anchor.GetComponent<Rigidbody2D>().simulated = false;
            anchor.GetComponent<projectile_flying>().Shooter = gameObject;
            Anchors.Add(anchor);

            sqL0.Add(SquareMaxRopeLength);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // controls
        // shoot an anchor
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

            // reset rope equilibrium length
            sqL0[getNextAnchor()] = SquareMaxRopeLength;

            anim.SetBool("Shooting", true);
        }

        // remove all anchors
        if (Input.GetButtonDown("Fire3"))
        {
            for (int i = 0; i < NUM_ANCHORS; i++)
            {
                Anchors[i].GetComponent<projectile_flying>().Attached = false;
                Anchors[i].GetComponent<SpriteRenderer>().enabled = false;
            }

            anim.SetBool("Shooting", false);
        }

        float y = Input.GetAxis("Vertical");
        for (int i = 0; i < Anchors.Count; i++)
        {
            sqL0[i] -= y * RopeChangeRate;
            if (sqL0[i] < 0) sqL0[i] = 0;
        }

        // mechanism or graphics updates
        float[] sq_rope_lengths = new float[NUM_ANCHORS];
        //rope_renderer.SetPosition(0, transform.position);
        //rope_renderer.SetPosition(1, transform.position);
        //rope_renderer.SetPosition(2, transform.position);
        for (int i = 0; i < 3; i++)
        {
            rope_renderer.points3[i] = transform.position;
        }

        bool odming = false;
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

            sq_rope_lengths[i] = 0f;
            // render rope
            bool attached = proj.GetComponent<projectile_flying>().Attached;
            if (attached || proj.GetComponent<Rigidbody2D>().simulated)
            {
                int j;
                if (i == 0) j = 0; else j = 2;
                //rope_renderer.SetPosition(j, proj.transform.position);
                rope_renderer.points3[j] = proj.transform.position;
                sq_rope_lengths[i] = (proj.transform.position - transform.position).sqrMagnitude;
            }

            if (attached)
                odming = true;
        }
        anim.SetBool("ODMing", odming);
        
        // draw rope
        float[] p = new float[NUM_ANCHORS];

        for (int i = 0; i < NUM_ANCHORS; i++)
        {
            if (sq_rope_lengths[i] > 0)
                p[i] = 1 -(float) Math.Sqrt(sqL0[i] / sq_rope_lengths[i]);
            else
                p[i] = 1 - 0f;

            if (p[i] < 0) p[i] = 0f;

            Color color = new Color(1f, 1f- p[i], 1f- p[i])*0.8f;
            color.a = 1f;
            rope_renderer.SetColor(color, i);
        }

        /*
        float alpha = 1.0f;
        Gradient gradient = new Gradient();

        gradient.SetKeys(
        new GradientColorKey[] { new GradientColorKey(Color.blue, p),
                                 new GradientColorKey(Color.red, 1f)},
        new GradientAlphaKey[] { new GradientAlphaKey(alpha, 0.0f), new GradientAlphaKey(alpha, 1.0f) }
        );
        
        gradient.mode = GradientMode.Fixed;
        rope_renderer.colorGradient = gradient;
        */
        
        rope_renderer.Draw();
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
        VectorLine.Destroy(ref rope_renderer);
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
