using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class soldier_shooting_old : MonoBehaviour {
    public Camera PlayerCamera;
    public GameObject ProjectilePrefab;
    public float FirePower = 10.0f;
    public float SquareMaxRopeLength = 100.0f;

    List<GameObject> projectiles = new List<GameObject>();
    Queue<GameObject> anchors = new Queue<GameObject>();

    Rigidbody2D rb;
    Animator anim;

    // Use this for initialization
    void Start () {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }
	
	// Update is called once per frame
	void Update () {
        GameObject removed;

        for (int i = 0; i < projectiles.Count; i++)
        {
            GameObject proj = projectiles[i];
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

            // determine if a projectile should be considered as an new anchor
            bool attached = proj.GetComponent<projectile_flying>().Attached;
            if (attached && !anchors.Contains(proj))
            {
                anchors.Enqueue(proj);
                // if there are more than 2 anchors then remove the older one
                if (anchors.Count > 2)
                {
                    removed = anchors.Dequeue();
                    removed.GetComponent<projectile_flying>().Attached = false;
                    removed.GetComponent<SpriteRenderer>().enabled = false;
                }
            }


        }

        if (Input.GetButtonDown("Fire1"))
        {
            Vector2 objPos = transform.position; 
            Vector2 mousePos = Input.mousePosition; 
            mousePos = PlayerCamera.ScreenToWorldPoint(mousePos);
            Vector2 direction = mousePos - objPos;
            
            GameObject proj = createProjectile(transform.position, direction);
            if (anchors.Count == 2)
            {
                removed = anchors.Dequeue();
                removed.GetComponent<projectile_flying>().Attached = false;
                removed.GetComponent<SpriteRenderer>().enabled = false;
            }
            
            if (!projectiles.Contains(proj))
                projectiles.Add(proj);

            anim.SetBool("ODMing", true);
        }
	}

    GameObject createProjectile(Vector2 pos, Vector2 direction)
    {
        GameObject new_proj = null;
        Rigidbody2D proj_rb;

        Quaternion rotation = Quaternion.FromToRotation(Vector3.right, direction);

        for (int i = 0; i <= projectiles.Count; i++)
        {
            if (i == projectiles.Count)
            {
                new_proj = Instantiate<GameObject>(ProjectilePrefab, pos, rotation);
            }
            else
            {
                GameObject proj = projectiles[i];

                proj_rb = proj.GetComponent<Rigidbody2D>();
                if (!proj_rb.simulated && !anchors.Contains(proj))
                {
                    new_proj = proj;
                    new_proj.transform.position = pos;
                    new_proj.transform.rotation = rotation;
                    break;
                }
            }
        }

        new_proj.GetComponent<SpriteRenderer>().enabled = true;
        proj_rb = new_proj.GetComponent<Rigidbody2D>();
        proj_rb.velocity = rb.velocity;
        proj_rb.simulated = true;
        Vector2 force = direction.normalized * FirePower;
        proj_rb.AddForce(force, ForceMode2D.Impulse);

        return new_proj;
    }
}
