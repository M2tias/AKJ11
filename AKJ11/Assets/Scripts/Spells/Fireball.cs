using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : MonoBehaviour
{
    [SerializeField]
    private float speed;
    [SerializeField]
    private float damage;

    private Rigidbody2D body;
    private Vector3 moveDir;

    public void Initialize(Vector2 position, Vector2 direction)
    {
        transform.position = new Vector3(position.x, position.y, 0);

        //Vector3 targetDir = aimingReticule.transform.position - transform.position;
        float angleDiff = Vector2.SignedAngle(transform.right, direction);
        transform.transform.Rotate(Vector3.forward, angleDiff);
        moveDir = direction.normalized;
    }

    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        body.velocity = moveDir * speed * Time.deltaTime;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        var hurtable = other.GetComponent<Hurtable>();
        if (hurtable != null)
        {
            hurtable.Hurt(damage);
        }
    }
}
