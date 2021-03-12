using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;

public class Boss : MonoBehaviour
{
    public Transform arms;
    public ParticleSystem SpawnEffect;
    public Projectile SpellProjectile;
    public Transform SpellRoot1, SpellRoot2;
    public CircleOfDoom CircleOfDoomPrefab;
    public PillarOfDoom PillarOfDoomPrefab;
    public BossShield shield;

    private Transform target;
    private bool sleeping = true;

    private Animator anim;
    private Rigidbody2D rb;
    private Collider2D coll;
    private Hurtable hurtable;

    private int aggroLayerMask;

    bool initialized = false;

    Quaternion initialArmsRotation;

    private float maxHealth = 50;
    private float health;
    private float healingDuration = 10.0f;
    private float selfHealPerTick = 2.0f;
    private float selfHealTickInterval = 1.0f;
    private float healIndicatorRange = 0.5f;

    private float maxArmsRotateSpeed = 360;
    private float aggroRange = 3.0f;
    private float speedModifier = 1.0f;
    private float maxSpeedModifier = 4.0f;
    private float minSpeedModifier = 1.0f;

    private float delayAfterSpawn = 4.0f;
    private float minCooldown = 3.0f;
    private float maxCooldown = 5.0f;

    private float spellDamage = 1.0f;

    private float ringInitialCooldown = 20.0f;
    private float ringCooldown = 15f;

    private float channelingDuration = 10.0f;
    private float channelInterval = 0.25f;
    private float channelRadius = 3.0f;
    private bool channelIntroduced = false;

    private bool ringAvailable = false;
    private bool homingSpells = false;
    private float channelTimer;

    private float moveTimer;
    private float moveInterval = 2f;
    private float moveChance = 0.5f;
    private float initialMoveDelay = 5.0f;
    private float minMoveDistance = 1.0f;
    private float maxMoveDistance = 2.0f;
    private Vector2 moveTarget;
    private float moveSpeed = 5.0f;
    private float desiredDistance = 5.0f;

    private List<BossHealer> healers;

    private BossState state = BossState.WAIT;


    public void Start()
    {
        if (!initialized)
        {
            Initialize(null, null);
            WakeUp();
        }
    }

    public void Initialize(GameEntityConfig entityConfig, MapNode node)
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody2D>();
        if (node != null)
        {
            transform.position = (Vector2)node.Position;
        }

        aggroLayerMask = LayerMask.GetMask("Player", "Wall");
        hurtable = GetComponent<Hurtable>();
        if (hurtable != null)
        {
            //hurtable.Initialize(config.HealthConfig, config.ExpGainConfig);
        }
        anim = GetComponent<Animator>();
        coll = GetComponent<Collider2D>();
        coll.enabled = false;

        initialArmsRotation = arms.localRotation;

        health = maxHealth;

        healers = new List<BossHealer>(
            GameObject.FindGameObjectsWithTag("BossHealer")
                .Select(it => it.GetComponent<BossHealer>())
        );
        healers.ForEach(it => it.SetHealTarget(this));

        shield.Deactivate();

        initialized = true;
        resetMove();
    }

    public void WakeUp()
    {
        sleeping = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (sleeping)
        {
            return;
        }

        if (healers.Count > 0)
        {
            var healersAlive = countAliveHealers() / healers.Count;
            speedModifier = Mathf.Lerp(maxSpeedModifier, minSpeedModifier, healersAlive);
        }
        else
        {
            speedModifier = maxSpeedModifier;
        }

        switch (state)
        {
            case BossState.WAIT:
                handleWaiting();
                break;
            case BossState.SPAWN:
                handleSpawning();
                break;
            case BossState.PREPARE:
                handlePrepare();
                break;
            case BossState.ATTACK:
                handleAttack();
                break;
            case BossState.CHANNEL:
                handleChanneling();
                break;
            case BossState.COOLDOWN:
                handleCooldown();
                break;
            case BossState.HEALING:
                handleHealing();
                break;
        }
    }

    void FixedUpdate()
    {
        if (sleeping)
        {
            return;
        }
        if (!canMove() || Vector2.Distance(transform.position, moveTarget) < 0.1f)
        {
            rb.velocity = Vector3.zero;
        }
        else
        {
            if (canMove())
            {
                var moveDir = moveTarget - (Vector2)transform.position;
                rb.velocity = moveDir.normalized * moveSpeed;
            }
        }
    }

    private void handleWaiting()
    {
        if (Vector2.Distance(transform.position, target.position) < aggroRange)
        {
            Spawn();
        }
    }

    private void handleSpawning()
    {
        normalizeArms();
    }

    private void handleCooldown()
    {
        if (moveTimer < Time.time)
        {
            if (Vector2.Distance(transform.position, target.position) > desiredDistance)
            {
                var dir = transform.position - target.position;
                moveTarget = target.position + dir.normalized * desiredDistance;
            }
            else
            {
                if (Random.Range(0.0f, 1.0f) <= moveChance)
                {
                    moveTarget = transform.position + Quaternion.AngleAxis(Random.Range(0, 360), Vector3.forward) * Vector2.up * Random.Range(minMoveDistance, maxMoveDistance);
                }
            }
            moveTimer = Time.time + moveInterval;
        }

        rotateArms();
    }

    private void handlePrepare()
    {
        rotateArms();
    }

    private void handleAttack()
    {
        rotateArms();
    }

    private void handleChanneling()
    {
        normalizeArms();

        if (channelTimer < Time.time)
        {
            var pillar = Instantiate(PillarOfDoomPrefab);
            pillar.transform.position = (Vector2)target.position + new Vector2(Random.Range(-channelRadius, channelRadius), Random.Range(-channelRadius, channelRadius));
            channelTimer = Time.time + channelInterval;
        }
    }

    private void handleHealing()
    {
    }

    private void Spawn()
    {
        coll.enabled = true;
        state = BossState.SPAWN;
        anim.SetBool("Spawn", true);
        SpawnEffect.Play();
        resetMove();
        moveTimer = Time.time + initialMoveDelay;
    }

    private void queueNextAttack()
    {
        if (state == BossState.HEALING || state == BossState.EXHAUSTED)
        {
            return;
        }

        var cooldown = Random.Range(minCooldown, maxCooldown) / speedModifier;
        Invoke("prepareForAttack", cooldown - 0.5f);
        Invoke("startNextAttack", cooldown);
        state = BossState.COOLDOWN;
    }

    private void prepareForAttack()
    {
        if (state == BossState.HEALING || state == BossState.EXHAUSTED)
        {
            return;
        }
        state = BossState.PREPARE;
    }

    private void startNextAttack()
    {
        if (state == BossState.HEALING || state == BossState.EXHAUSTED)
        {
            return;
        }

        var healerCount = countAliveHealers();

        if (healerCount < 3 && ringAvailable)
        {
            anim.SetBool("Spell1", true);
            homingSpells = true;
            state = BossState.ATTACK;
            ringAvailable = false;
            Invoke("enableRing", ringCooldown);
        }
        else
        {
            var numberOfAttacks = healerCount == 4 ? 2 : 3;
            var attack = Random.Range(1, numberOfAttacks + 1);

            if (healerCount < 4 && !channelIntroduced)
            {
                attack = 3;
                channelIntroduced = true;
            }

            switch (attack)
            {
                case 1:
                    anim.SetBool("Spell2", true);
                    homingSpells = false;
                    state = BossState.ATTACK;
                    break;
                case 2:
                    anim.SetBool("Spell3", true);
                    homingSpells = false;
                    state = BossState.ATTACK;
                    break;
                case 3:
                    anim.SetBool("Channel", true);
                    homingSpells = false;
                    channelTimer = Time.time + channelInterval;
                    state = BossState.CHANNEL;
                    Invoke("ChannelingFinished", channelingDuration);
                    break;

            }
        }
    }

    private void resetMove()
    {
        moveTarget = transform.position;
    }

    private bool canMove()
    {
        return state == BossState.COOLDOWN || state == BossState.PREPARE;
    }

    private void enableRing()
    {
        ringAvailable = true;
    }

    private void normalizeArms()
    {
        Vector2 dir = Vector2.down;
        var angle = Vector2.SignedAngle(-arms.up, dir);
        angle = Mathf.Sign(angle) * Mathf.Min(maxArmsRotateSpeed * Time.deltaTime, Mathf.Abs(angle));
        arms.Rotate(Vector3.forward, angle);
    }

    private void rotateArms()
    {
        Vector2 dir = target.position - arms.position;
        var angle = Vector2.SignedAngle(-arms.up, dir);
        angle = Mathf.Sign(angle) * Mathf.Min(maxArmsRotateSpeed * Time.deltaTime, Mathf.Abs(angle));
        arms.Rotate(Vector3.forward, angle);
    }

    public void SpellFinished()
    {
        anim.SetBool("Spell1", false);
        anim.SetBool("Spell2", false);
        anim.SetBool("Spell3", false);
        queueNextAttack();
    }

    public void ChannelingFinished()
    {
        if (state == BossState.HEALING || state == BossState.EXHAUSTED)
        {
            return;
        }

        anim.SetBool("Channel", false);
        queueNextAttack();
    }

    public void Spawned()
    {
        Invoke("startNextAttack", delayAfterSpawn);
        Invoke("enableRing", ringInitialCooldown);
        state = BossState.COOLDOWN;
    }

    public void ShootSpell1()
    {
        var spellTarget = homingSpells ? target.position : SpellRoot1.position + SpellRoot1.up;
        var proj = Instantiate(SpellProjectile);
        proj.transform.position = SpellRoot1.position;
        proj.Launch(spellTarget, spellDamage);
        if (SoundManager.main != null)
        {
            //SoundManager.main.PlaySound(config.ProjectileSound);
        }
    }

    public void ShootSpell2()
    {
        var spellTarget = homingSpells ? target.position : SpellRoot2.position + SpellRoot2.up;
        var proj = Instantiate(SpellProjectile);
        proj.transform.position = SpellRoot2.position;
        proj.Launch(spellTarget, spellDamage);
        if (SoundManager.main != null)
        {
            //SoundManager.main.PlaySound(config.ProjectileSound);
        }
    }

    public void CastCircle()
    {
        var circle = Instantiate(CircleOfDoomPrefab);
        circle.Launch(target);
    }

    public void Damaged(float damage)
    {
        health -= damage;

        if (health < 0)
        {
            if (state != BossState.EXHAUSTED)
            {
                Exhausted();
            }
            else
            {
                if (health < -50)
                {
                    Die();
                }
            }
        }
    }

    public void Exhausted()
    {
        resetMove();
        health = 0;
        anim.SetBool("Exhausted", true);
        anim.SetBool("Channel", false);
        anim.SetBool("Spell1", false);
        anim.SetBool("Spell2", false);
        anim.SetBool("Spell3", false);
        if (countAliveHealers() > 0)
        {
            state = BossState.HEALING;
            hurtable.Immune = true;
            Invoke("SelfHeal", selfHealTickInterval);
            Invoke("Respawn", healingDuration);
            shield.Activate();
            coll.enabled = false;
            startHealers();
        }
        else
        {
            state = BossState.EXHAUSTED;
        }
    }

    public void Respawn()
    {
        state = BossState.COOLDOWN;
        resetMove();
        hurtable.Immune = false;
        anim.SetBool("Exhausted", false);
        queueNextAttack();
        stopHealers();
        shield.Deactivate();
        coll.enabled = true;
    }

    public void Die()
    {
        anim.SetBool("Die", true);
        Invoke("DieReally", 15.0f);
    }

    public void DieReally()
    {
        Destroy(gameObject);
    }

    private int countAliveHealers()
    {
        return healers.Where(it => it.IsAlive).Count();
    }

    private void startHealers()
    {
        healers.ForEach(it => it.StartHealing());
    }

    private void stopHealers()
    {
        healers.ForEach(it => it.StopHealing());
    }

    private void SelfHeal()
    {
        if (state != BossState.HEALING)
        {
            return;
        }

        Heal(5);
        Invoke("SelfHeal", selfHealTickInterval);
    }

    public void Heal()
    {
        Heal(5);
    }

    private void Heal(float amount)
    {
        health += amount;
        var healIndicatorOffset = new Vector2(Random.Range(-healIndicatorRange, healIndicatorRange), Random.Range(-healIndicatorRange, healIndicatorRange));
        UIWorldCanvas.main.ShowNumber((Vector2)transform.position + healIndicatorOffset, amount, ResourceType.HP);

        if (health > maxHealth)
        {
            health = maxHealth;
        }
    }

}

enum BossState
{
    WAIT,
    SPAWN,
    COOLDOWN,
    PREPARE,
    ATTACK,
    CHANNEL,
    HEALING,
    EXHAUSTED
}