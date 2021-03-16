using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealProjectile : MonoBehaviour
{
    private Rigidbody2D rb;
    private Collider2D collider;
    private List<ParticleSystem> trailEffects;

    private Boss target;

    private float speed = 25.0f;

    private bool killed = false;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        collider = GetComponent<Collider2D>();
        trailEffects = new List<ParticleSystem>(GetComponentsInChildren<ParticleSystem>());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        if (!killed)
        {
            rb.velocity = ((Vector2)target.transform.position - (Vector2)transform.position).normalized * speed;

            if (Vector2.Distance(transform.position, target.transform.position) < speed * Time.deltaTime + 0.5f)
            {
                transform.position = target.transform.position;
                target.Heal();
                Kill();
            }
        }
    }

    public void Launch(Boss target)
    {
        this.target = target;
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (killed)
        {
            return;
        }
        Kill();
    }

    private void Kill()
    {
        if (!killed)
        {
            killed = true;
            rb.velocity = Vector2.zero;
            collider.enabled = false;
            trailEffects.ForEach(effect => effect.Stop());
            Invoke("Destroy", 0.5f);
        }
    }

    private void Destroy()
    {
        Destroy(gameObject);
    }
}
