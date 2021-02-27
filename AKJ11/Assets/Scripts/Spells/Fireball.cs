using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    // [SerializeField]
    private float bounceDistance = 5;
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
        List<Hurtable> hurtables = new List<Hurtable>();
        Hurtable hurtable = other.GetComponent<Hurtable>();
        List<GameObject> enemies = GameObject.FindGameObjectsWithTag("Enemy").ToList();

        // TODO: aoe against player?
        if (aoe > 0 && other.tag == "Enemy")
        {
            // this includes the hit target
            hurtables = GetAoeTargets(enemies);
        }
        // no aoe, damage only the target hit
        else if(hurtable != null)
        {
            hurtables.Add(hurtable);
        }

        if(bounces > 0)
        {
            List<GameObject> eligibleForHits = new List<GameObject>(enemies);

            // bounce doesn't hit already hit enemies
            foreach (Hurtable h in hurtables)
            {
                eligibleForHits.Remove(h.gameObject);
            }


            GetBounceHits(bounces, hurtables, eligibleForHits, new List<GameObject>());
        }

        hitTargets(hurtables);

        Destroy(gameObject);
    }

    private void GetBounceHits(float bounces, List<Hurtable> hurtables, List<GameObject> eligibleForHits, List<GameObject> alreadyHit)
    {
        if (bounces <= 0) return;

        foreach (GameObject candidate in eligibleForHits)
        {
            if (alreadyHit.Contains(candidate)) continue;

            float distance = Vector2.Distance(candidate.transform.position, transform.position);
            if (bounceDistance > distance)
            {
                Hurtable enemy = candidate.GetComponent<Hurtable>();
                if (enemy != null)
                {
                    hurtables.Add(enemy);
                }

                alreadyHit.Add(candidate);
                GetBounceHits(bounces - 1, hurtables, eligibleForHits.Where(x => x != candidate).ToList(), alreadyHit);
            }
        }
    }

    private List<Hurtable> GetAoeTargets(List<GameObject> enemies)
    {
        List<Hurtable> hurtables = new List<Hurtable>();
        foreach (GameObject candidate in enemies)
        {
            float distance = Vector2.Distance(candidate.transform.position, transform.position);
            if (aoe > distance)
            {
                Hurtable enemy = candidate.GetComponent<Hurtable>();
                if (enemy != null)
                {
                    hurtables.Add(enemy);
                }
            }
        }

        return hurtables;
    }

    private void hitTargets(List<Hurtable> hurtables)
    {
        Debug.Log("Hits " + hurtables.Count + " targets with the aoe of " + aoe);
        foreach (Hurtable h in hurtables)
        {
            if (damage > 0)
            {
                h.Hurt(damage);
            }

            if (dotTickDamage > 0)
            {
                h.Dot(dotTickDamage, dotDuration);
            }
        }
    }
}
