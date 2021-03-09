using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealProjectile : MonoBehaviour
{
    private Rigidbody2D rb;

    private Vector2 target;

    private float speed = 20.0f;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        rb.velocity = (target - (Vector2)transform.position).normalized * speed;
    }

    public void Launch(Vector2 target)
    {
        this.target = target;
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        var boss = collision.collider.GetComponent<Boss>();
        if (boss != null)
        {
            boss.Heal();
        }
        Destroy(gameObject);
    }
}
