using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : MonoBehaviour
{
    [SerializeField]
    private float speed;
    [SerializeField]
    private float damage;

    [SerializeField]
    private float aoe;
    [SerializeField]
    private float bounces;
    [SerializeField]
    private float dotTickDamage;
    [SerializeField]
    private float dotDuration;
    [SerializeField]
    private float cooldown; // probably not needed here...
    [SerializeField]
    private float lifetime = 5;
    private float started;

    private Rigidbody2D body;
    private Vector3 moveDir;


    public void Initialize(Vector2 position, Vector2 direction)
    {
        transform.position = new Vector3(position.x, position.y, 0);

        //Vector3 targetDir = aimingReticule.transform.position - transform.position;
        float angleDiff = Vector2.SignedAngle(transform.right, direction);
        transform.transform.Rotate(Vector3.forward, angleDiff);
        moveDir = direction.normalized;
        Debug.Log("Dot: " + dotTickDamage + " dmg, " + dotDuration + "s");
    }

    public void SetConfig(float damage, float aoe, float bounces, float dotTickDamage, float dotDuration)
    {
        this.aoe = aoe;
        this.bounces = bounces;
        this.damage = damage;
        this.dotDuration = dotDuration;
        this.dotTickDamage = dotTickDamage;
        Debug.Log("Dot: " + dotTickDamage + " dmg, " + dotDuration + "s");
    }

    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        started = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        body.velocity = moveDir * speed * Time.deltaTime;

        if (Time.time - started > lifetime)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        var hurtable = other.GetComponent<Hurtable>();
        if (hurtable != null)
        {
            if (damage > 0)
            {
                hurtable.Hurt(damage);
            }

            if (dotTickDamage > 0)
            {
                hurtable.Dot(dotTickDamage, dotDuration);
            }
        }

        if (other.tag == "Wall")
        {
            Destroy(gameObject);
            return;
        }
    }
}
