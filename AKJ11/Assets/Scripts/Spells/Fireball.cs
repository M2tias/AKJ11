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
    [SerializeField]
    private Sprite projectileSprite;

    private float started;

    private Rigidbody2D body;
    private SpriteRenderer renderer;
    private Collider2D collider;
    private Vector3 moveDir;

    private Experience playerExperience;

    [SerializeField]
    private SpellBaseConfig config;

    public ParticleSystem FireEffect;
    public ParticleSystem PoisonEffect;
    public ParticleSystem LightningEffect;

    public ParticleSystem FireExplosion;
    public ParticleSystem PoisonExplosion;
    public ParticleSystem LightningExplosion;

    bool killed = false;

    private List<ParticleSystem> trailEffects;

    public void Initialize(Vector2 position, Vector2 direction, Experience playerExp)
    {
        transform.position = new Vector3(position.x, position.y, 0);

        float angleDiff = Vector2.SignedAngle(transform.right, direction);
        transform.transform.Rotate(Vector3.forward, angleDiff);
        moveDir = direction.normalized;

        playerExperience = playerExp;

    }

    public void SetConfig(SpellBaseConfig config)
    {
        this.config = config;
        renderer = GetComponent<SpriteRenderer>();
        aoe = config.Aoe;
        bounces = config.Bounces;
        damage = config.Damage;
        dotDuration = config.Dot;
        dotTickDamage = config.DotTickDamage;
        speed = config.Speed;
        projectileSprite = config.ProjectileSprite;

        for (var i = 0; i < config.Aoe; i++)
        {
            Instantiate(FireEffect, transform);
        }
        for (var i = 0; i < config.Bounces; i++)
        {
            Instantiate(LightningEffect, transform);
        }
        for (var i = 0; i < config.Dot; i++)
        {
            Instantiate(PoisonEffect, transform);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        renderer = GetComponent<SpriteRenderer>();
        collider = GetComponent<Collider2D>();
        renderer.sprite = projectileSprite;
        started = Time.time;
        trailEffects = new List<ParticleSystem>(GetComponentsInChildren<ParticleSystem>());
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time - started > lifetime)
        {
            Kill();
        }
    }

    void FixedUpdate()
    {
        if (!killed)
        {
            body.velocity = moveDir * speed;
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

        Kill();
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
        foreach (Hurtable h in hurtables)
        {
            if (damage > 0)
            {
                h.Hurt(damage, playerExperience);
            }

            if (dotTickDamage > 0)
            {
                h.Dot(dotTickDamage, dotDuration);
            }
        }
    }

    private void Kill()
    {
        if (!killed)
        {
            killed = true;
            body.velocity = Vector2.zero;
            collider.enabled = false;
            renderer.enabled = false;
            trailEffects.ForEach(effect => effect.Stop());
            CreateExplosion();
            Invoke("Destroy", 0.5f);
        }
    }

    private void Destroy()
    {
        Destroy(gameObject);
    }

    private void CreateExplosion()
    {
        if (config.Aoe > 0)
        {
            var t = (float)config.Aoe / config.MaxAoeLevel;
            var scale = Mathf.Lerp(1.0f, 5.0f, t);
            var expl = Instantiate(FireExplosion);
            expl.transform.position = transform.position;
            expl.transform.localScale = new Vector3(scale, scale, scale);
        }
    }
}
