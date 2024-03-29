using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;

public class GameEntityEnemy : GameEntity
{
    private float pathingFrequency = 0.1f;
    private float idlePatrolMinDelay = 3.0f;
    private float idlePatrolMaxDelay = 6.0f;
    private float idlePatrolDistance = 1.0f;

    private Transform target;
    private Weapon weapon;

    private NavMeshPath path;
    private int cornerIndex;

    private Rigidbody2D rb;
    private Vector2 targetPos;
    private Vector2 moveTargetPos;

    private State state = State.IDLE;

    private int aggroLayerMask;

    public GameEntityConfig gameEntityConfig;
    private EnemyConfig config;

    private bool sleeping = true;

    private Animator anim;
    private SpriteRenderer rend;

    private bool dashing = false;
    private float dashSpeedDecay = 10.0f;
    private float moveSpeed;

    private Collider2D collider;

    public void Start()
    {
        if (config != null)
        {
            Initialize(gameEntityConfig, null);
            WakeUp();
        }
    }


    public override void Initialize(GameEntityConfig entityConfig, MapNode node)
    {
        base.Initialize(entityConfig, node);
        gameEntityConfig = entityConfig;
        target = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody2D>();
        weapon = GetComponentInChildren<Weapon>();
        if (node != null)
        {
            transform.position = (Vector2)node.Position;
        }
        this.config = entityConfig.EnemyConfig;
        

        InvokeRepeating("UpdatePathing", pathingFrequency, pathingFrequency);
        RandomizeTargetPosition();

        aggroLayerMask = LayerMask.GetMask("Player", "Wall");
        Hurtable hurtable = GetComponent<Hurtable>();
        if (hurtable != null) {
            hurtable.Initialize(config.HealthConfig, config.ExpGainConfig, delegate {
                Die();
            });
        }
        anim = GetComponent<Animator>();
        rend = GetComponent<SpriteRenderer>();
        rend.color = config.ColorTint;

        weapon.Initialize(config.WeaponConfig);

        anim.runtimeAnimatorController = config.AnimatorController;

        moveSpeed = config.MoveSpeed;

        collider = GetComponent<Collider2D>();
    }

    public override void WakeUp() {
        base.WakeUp();
        sleeping = false;
    }

    public override void Die()
    {
        base.Die();
    }

    // Update is called once per frame
    void Update()
    {
        if (sleeping) {
            return;
        }
        handleState();
        switch (state)
        {
            case State.IDLE:
                idleRoutine();
                break;
            case State.ATTACK:
                attackRoutine();
                break;
        }

        if (HasPath())
        {
            if (IsLastCorner() && distanceToNextCorner() < 0.1f)
            {
                moveTargetPos = transform.position;
            }
            else
            {
                moveTargetPos = getNextCorner();
            }
        }

        if (rb.velocity.magnitude > 0.1f)
        {
            anim.SetBool("Walk", true);
        }
        else
        {
            anim.SetBool("Walk", false);
        }
        
        if (rb.velocity.x > 0.1f)
        {
            rend.flipX = true;
        }
        if (rb.velocity.x < -0.1f)
        {
            rend.flipX = false;
        }
    }

    private void handleState()
    {
        Vector2 targetDir = target.position - transform.position;

        if (state == State.IDLE)
        {
            if (damaged)
            {
                state = State.ATTACK;
                if (config.DashSpeed > 0.0f)
                {
                    Invoke("Dash", Random.Range(config.DashMinDelay, config.DashMaxDelay));
                }
            }

            if (Vector2.Distance(transform.position, target.position) < config.AggroRange)
            {
                var hit = Physics2D.Raycast(transform.position, targetDir, 100, aggroLayerMask);

                if (hit != null && hit.transform == target)
                {
                    state = State.ATTACK;
                    if (config.DashSpeed > 0.0f)
                    {
                        Invoke("Dash", Random.Range(config.DashMinDelay, config.DashMaxDelay));
                    }
                }
            }
        }
    }

    private bool damaged = false;

    public void Damaged()
    {
        damaged = true;
        
        List<GameObject> enemies = GameObject.FindGameObjectsWithTag("Enemy").ToList();
        enemies.Where(it => Vector2.Distance(transform.position, it.transform.position) < 3.0f)
            .Select(it => it.GetComponent<GameEntityEnemy>())
            .Where(it => it != null)
            .ToList().ForEach(it => it.Aggro());
    }

    public void Aggro()
    {
        damaged = true;
    }

    private void idleRoutine()
    {
        weapon.LookAt((Vector2)transform.position + Vector2.up);
    }

    private void RandomizeTargetPosition()
    {
        if (state == State.IDLE)
        {
            targetPos = (Vector2)transform.position + new Vector2(Random.Range(-idlePatrolDistance, idlePatrolDistance), Random.Range(-idlePatrolDistance, idlePatrolDistance));
            Invoke("RandomizeTargetPosition", Random.Range(idlePatrolMinDelay, idlePatrolMaxDelay));
        }
    }

    private void attackRoutine()
    {
        if (dashing)
        {
            collider.enabled = false;
            if (moveSpeed <= config.MoveSpeed / 2.0f || rb.velocity.magnitude < 0.1f)
            {
                dashing = false;
                moveSpeed = config.MoveSpeed;
                anim.SetBool("Dash", false);
            }
            else
            {
                moveSpeed -= dashSpeedDecay * Time.deltaTime;
            }

            if (Vector2.Distance(targetPos, transform.position) < 1.0f)
            {
                dashing = false;
                moveSpeed = config.MoveSpeed;
                anim.SetBool("Dash", false);
                targetPos = target.position;
                UpdatePathing();
            }
        }
        else
        {
            collider.enabled = true;
        }

        weapon.LookAt(target.position);
        if (Vector2.Distance(target.position, transform.position) < config.AttackRange)
        {
            if (!dashing)
            {
                targetPos = transform.position;
            }
            weapon.Attack();
        }
        else
        {
            if (!dashing)
            {
                targetPos = target.position;
            }
        }

    }

    private void Dash()
    {
        dashing = true;
        moveSpeed = config.DashSpeed;
        var dashDir = target.position - transform.position;
        targetPos = transform.position + Quaternion.AngleAxis(Random.Range(-60f, 60f), Vector3.forward) * dashDir * config.DashDistance;
        anim.SetBool("Walk", false);
        anim.SetBool("Dash", true);
        UpdatePathing();
        Invoke("Dash", Random.Range(config.DashMinDelay, config.DashMaxDelay));
    }

    void FixedUpdate()
    {
        if (sleeping) {
            return;
        }
        if (Vector2.Distance(transform.position, moveTargetPos) < 0.1f)
        {
            rb.velocity = Vector3.zero;
        }
        else
        {
            var moveDir = moveTargetPos - (Vector2)transform.position;
            rb.velocity = moveDir.normalized * moveSpeed;
        }
    }

    private Vector2 getNextCorner()
    {
        while (!IsLastCorner() && distanceToNextCorner() < 0.1f)
        {
            cornerIndex++;
        }
        return path.corners[cornerIndex];
    }

    private float distanceToNextCorner()
    {
        return Vector2.Distance(path.corners[cornerIndex], transform.position);
    }

    private bool IsLastCorner()
    {
        return cornerIndex >= path.corners.Length - 1;
    }

    private bool HasPath()
    {
        return path != null && path.corners.Length > 0;
    }

    private float GetRemainingPathDistance(NavMeshPath navMeshPath, int currentCornerIndex)
    {
        float sum = 0;
        Vector2 prevPos = transform.position;
        for (int i = currentCornerIndex; i < navMeshPath.corners.Length; i++)
        {
            sum += Vector2.Distance(prevPos, navMeshPath.corners[i]);
            prevPos = navMeshPath.corners[i];
        }
        return sum;
    }

    private void UpdatePathing()
    {
        if (sleeping) {
            return;
        }
        path = GetPathTo(targetPos);
        cornerIndex = 0;
    }

    private NavMeshPath GetPathTo(Vector2 target)
    {
        NavMeshPath newPath = new NavMeshPath();
        NavMesh.CalculatePath((Vector2)transform.position, target, NavMesh.AllAreas, newPath);
        return newPath;
    }
}


enum State
{
    IDLE,
    ATTACK
}
