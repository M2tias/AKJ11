using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    private Rigidbody2D body;
    private SpriteRenderer renderer;

    [SerializeField]
    private GameObject hand;
    private SpriteRenderer handRenderer;

    [SerializeField]
    private float speed;
    private float horizontal;
    private float vertical;

    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        renderer = GetComponent<SpriteRenderer>();
        handRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");

        if (horizontal < 0)
        {
            renderer.flipX = true;
        }
        if (horizontal > 0)
        {
            renderer.flipX = false;
        }
    }

    void FixedUpdate()
    {
        var dir = new Vector2(horizontal, vertical).normalized;
        body.velocity = dir * speed;
    }
}
