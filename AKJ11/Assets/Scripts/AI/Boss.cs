using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Boss : MonoBehaviour
{
    public Transform arms;
    public ParticleSystem SpawnEffect;
    public Projectile SpellProjectile;
    public Transform SpellRoot1, SpellRoot2;
    public GameObject CircleOfDoom;
    public GameObject PillarOfDoom;

    private Transform target;
    private bool sleeping = true;

    private Animator anim;

    private Rigidbody2D rb;

    private int aggroLayerMask;

    bool initialized = false;

    Quaternion initialArmsRotation;
    private float maxArmsRotateSpeed = 360;
    private float aggroRange = 3.0f;
    private float speedModifier = 10.0f;

    private float delayAfterSpawn = 4.0f;
    private float minCooldown = 3.0f;
    private float maxCooldown = 5.0f;

    private float ringInitialCooldown = 20.0f;
    private float ringCooldown = 12.5f;

    private float channelingDuration = 10.0f;
    private float channelInterval = 0.25f;
    private float channelRadius = 5.0f;
    
    private bool ringAvailable = false;
    private bool homingSpells = false;
    private float channelTimer;

    private BossState state = BossState.WAIT;


    public void Start()
    {
        if (!initialized)
        {
            Initialize(null);
            WakeUp();
        }
    }

    public void Initialize(MapNode node)
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody2D>();
        if (node != null)
        {
            transform.position = (Vector2)node.Position;
        }

        aggroLayerMask = LayerMask.GetMask("Player", "Wall");
        Hurtable hurtable = GetComponent<Hurtable>();
        if (hurtable != null)
        {
            //hurtable.Initialize(config.HealthConfig, config.ExpGainConfig);
        }
        anim = GetComponent<Animator>();

        initialArmsRotation = arms.localRotation;

        initialized = true;
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
            var circle = Instantiate(PillarOfDoom);
            circle.transform.position = (Vector2)target.position + new Vector2(Random.Range(-channelRadius, channelRadius), Random.Range(-channelRadius, channelRadius));
            channelTimer = Time.time + channelInterval;
        }
    }

    private void Spawn()
    {
        state = BossState.SPAWN;
        anim.SetBool("Spawn", true);
        SpawnEffect.Play();
    }

    private void queueNextAttack()
    {
        var cooldown = Random.Range(minCooldown, maxCooldown) / speedModifier;
        Invoke("prepareForAttack", cooldown - 0.5f);
        Invoke("startNextAttack", cooldown);
        state = BossState.COOLDOWN;
    }

    private void prepareForAttack()
    {
        state = BossState.PREPARE;
    }

    private void startNextAttack()
    {
        if (ringAvailable)
        {
            anim.SetBool("Spell1", true);
            homingSpells = true;
            state = BossState.ATTACK;
            ringAvailable = false;
            Invoke("enableRing", ringCooldown);
        }
        else
        {
            var attack = Random.Range(1, 4);
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
        proj.Launch(spellTarget, 5.0f);
        if (SoundManager.main != null)
        {
            //SoundManager.main.PlaySound(config.ProjectileSound);
        }
    }

    public void ShootSpell2()
    {
        var spellTarget = homingSpells ? target.position : SpellRoot2.position + SpellRoot1.up;
        var proj = Instantiate(SpellProjectile);
        proj.transform.position = SpellRoot2.position;
        proj.Launch(spellTarget, 5.0f);
        if (SoundManager.main != null)
        {
            //SoundManager.main.PlaySound(config.ProjectileSound);
        }
    }

    public void CastCircle()
    {
        var circle = Instantiate(CircleOfDoom);
        circle.transform.position = target.position;
    }

    public void Damaged(float damage)
    {

    }

}

enum BossState
{
    WAIT,
    SPAWN,
    COOLDOWN,
    PREPARE,
    ATTACK,
    CHANNEL
}