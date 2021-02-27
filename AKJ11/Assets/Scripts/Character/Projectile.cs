using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 100.0f;
    public float maxTimeToLive = 10.0f;

    public ParticleSystem DestroyEffect;

    private Vector2 dir;
    private float damage;
    private Rigidbody2D rb;
    private SpriteRenderer rend;

    // Start is called before the first frame update
    void Start()
    {
        faceForward();
        rb = GetComponent<Rigidbody2D>();
        rend = GetComponentInChildren<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        faceForward();
    }

    void FixedUpdate()
    {
        rb.velocity = dir.normalized * speed;
    }

    public void Launch(Vector2 target, float damage)
    {
        dir = target - (Vector2)transform.position;
        Invoke("Kill", maxTimeToLive);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Kill();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        var hurtable = other.GetComponent<Hurtable>();
        if (hurtable != null)
        {
            hurtable.Hurt(damage);
        }
        Kill();
    }

    private void faceForward()
    {
        var angle = Vector2.SignedAngle(transform.up, dir);
        transform.Rotate(Vector3.forward, angle);
    }

    void Kill()
    {
        if (DestroyEffect != null)
        {
            Instantiate(DestroyEffect).transform.position = transform.position;
        }
        Destroy(gameObject);
    }
}
