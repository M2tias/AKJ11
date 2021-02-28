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
    private int bounces;
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

    [SerializeField]
    private SpellBaseConfig config;

    public ParticleSystem ArcaneEffect;
    public ParticleSystem FireEffect;
    public ParticleSystem PoisonEffect;
    public ParticleSystem LightningEffect;

    public ParticleSystem FireExplosion;
    public ParticleSystem PoisonExplosion;
    public ParticleSystem LightningExplosion;
    public ParticleSystem ArcaneExplosion;

    public ParticleSystem BounceEffect;

    bool killed = false;

    private List<ParticleSystem> trailEffects;
    private SpellLevelRuntime spellRuntime;

    public void Initialize(Vector2 position, Vector2 direction, SpellLevelRuntime runtime)
    {
        transform.position = new Vector3(position.x, position.y, 0);

        float angleDiff = Vector2.SignedAngle(transform.right, direction);
        transform.transform.Rotate(Vector3.forward, angleDiff);
        moveDir = direction.normalized;
        spellRuntime = runtime;
    }

    public void SetConfig(SpellBaseConfig config)
    {
        this.config = config;
        renderer = GetComponent<SpriteRenderer>();
        aoe = config.Aoe[0];
        bounces = config.Bounces[0];
        damage = config.Damage[0];
        dotDuration = config.Dot[0];
        dotTickDamage = config.DotTickDamage[0];
        speed = config.Speed[0];
        projectileSprite = config.ProjectileSprite;
    }

    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        renderer = GetComponent<SpriteRenderer>();
        collider = GetComponent<Collider2D>();
        renderer.sprite = projectileSprite;
        started = Time.time;

        aoe = config.Aoe[spellRuntime.AoeLevel];
        bounces = config.Bounces[spellRuntime.BouncesLevel];
        damage = config.Damage[spellRuntime.DamageLevel];
        dotDuration = config.Dot[spellRuntime.DotLevel];
        dotTickDamage = config.DotTickDamage[spellRuntime.DotTickDamageLevel];
        speed = config.Speed[spellRuntime.SpeedLevel];
        projectileSprite = config.ProjectileSprite;

        if (aoe > 0.01f)
        {
            for (var i = 0; i < aoe; i++)
            {
                Instantiate(FireEffect, transform);
            }
        }
        else
        {
            for (var i = 0; i < damage; i = i + 3)
            {
                Instantiate(ArcaneEffect, transform);
            }
        }
        for (var i = 0; i < bounces; i++)
        {
            Instantiate(LightningEffect, transform);
        }
        for (var i = 0; i < dotDuration; i++)
        {
            Instantiate(PoisonEffect, transform);
        }
        trailEffects = new List<ParticleSystem>(GetComponentsInChildren<ParticleSystem>());
    }

    // Update is called once per frame
    void Update()
    {
        if (nextTarget != null)
        {
            var newPos = Vector3.MoveTowards(transform.position, nextTarget.transform.position, bounceSpeed * Time.deltaTime);
            transform.position = newPos;

            if (Vector2.Distance(transform.position, nextTarget.transform.position) < 0.1f)
            {
                hitTarget(nextTarget.GetComponent<Hurtable>());
                CreateExplosion();
                DoAoeDamage(nextTarget);
                DoBounces();
            }
        }

        if (bouncing && nextTarget == null)
        {
            Kill();
        }

        if (Time.time - started > lifetime)
        {
            Kill();
        }
    }

    void FixedUpdate()
    {
        if (!bouncing && !killed)
        {
            body.velocity = moveDir * speed;
        }
        else
        {
            body.velocity = Vector2.zero;
        }
    }

    private bool bouncing = false;
    private int remainingBounces;
    private List<GameObject> bouncedTargets = new List<GameObject>();
    private GameObject nextTarget;
    private float bounceSpeed = 100f;

    void OnTriggerEnter2D(Collider2D other)
    {
        Hurtable hurtable = other.GetComponent<Hurtable>();
        hitTarget(hurtable);
        DoAoeDamage(other.gameObject);

        bouncedTargets.Add(other.gameObject);
        remainingBounces = bounces;
        DoBounces();
    }

    private void DoAoeDamage(GameObject exclude)
    {
        List<GameObject> enemies = GameObject.FindGameObjectsWithTag("Enemy").ToList();
        var hurtables = GetAoeTargets(enemies, exclude);
        hitTargets(hurtables);
    }

    private void DoBounces()
    {
        if (remainingBounces > 0)
        {
            if (!BounceEffect.isPlaying)
            {
                BounceEffect.Play();
            }
            bouncing = true;
            nextTarget = GetNextBounceTarget();
            collider.enabled = false;
            if (nextTarget == null)
            {
                Kill();
            }
            remainingBounces--;
        }
        else
        {
            Kill();
        }
    }

    private GameObject GetNextBounceTarget()
    {
        List<GameObject> enemies = GameObject.FindGameObjectsWithTag("Enemy").ToList();

        var nearestDist = 10000f;
        GameObject nearest = null;
        foreach (var candidate in enemies)
        {
            if (bouncedTargets.Contains(candidate)) continue;

            float distance = Vector2.Distance(candidate.transform.position, transform.position);
            if (bounceDistance > distance && distance < nearestDist)
            {
                nearestDist = distance;
                nearest = candidate;
                bouncedTargets.Add(candidate);
            }
        }
        return nearest;
    }

    private List<Hurtable> GetAoeTargets(List<GameObject> enemies, GameObject exclude)
    {
        List<Hurtable> hurtables = new List<Hurtable>();
        foreach (GameObject candidate in enemies)
        {
            if (candidate == exclude) continue;

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
                h.Hurt(damage, Experience.main);
            }

            if (dotTickDamage > 0)
            {
                h.Dot(dotTickDamage, dotDuration);
            }
        }
    }

    private void hitTarget(Hurtable hurtable)
    {
        if (hurtable == null) return;
        if (damage > 0)
        {
            hurtable.Hurt(damage, Experience.main);
        }

        if (dotTickDamage > 0)
        {
            hurtable.Dot(dotTickDamage, dotDuration);
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
            BounceEffect.Stop();
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
        if (aoe > 0.01f)
        {
            var scale = aoe * 1.0f;
            CreateExplosion(FireExplosion).transform.localScale = new Vector3(scale, scale, scale);
        }
        else
        {
            CreateExplosion(ArcaneExplosion);
        }
    }

    private ParticleSystem CreateExplosion(ParticleSystem prefab)
    {
        var expl = Instantiate(prefab);
        expl.transform.position = transform.position;
        return expl;
    }

    public int PlayerLevel
    {
        get => Experience.main.GetLevel();
    }
}
