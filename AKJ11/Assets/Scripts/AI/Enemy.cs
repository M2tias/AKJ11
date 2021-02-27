using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
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

    private EnemyConfig config;

    private bool sleeping = true;

    public void Initialize(EnemyConfig config, MapNode node)
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody2D>();
        weapon = GetComponentInChildren<Weapon>();
        transform.position = (Vector2)node.Position;
        this.config = config;


        InvokeRepeating("UpdatePathing", pathingFrequency, pathingFrequency);
        RandomizeTargetPosition();

        aggroLayerMask = LayerMask.GetMask("Player", "Wall");
        Hurtable hurtable = GetComponent<Hurtable>();
        if (hurtable != null) {
            hurtable.Initialize(config);
        }
    }

    public void WakeUp() {
        sleeping = false;
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
    }

    private void handleState()
    {
        Vector2 targetDir = target.position - transform.position;

        if (state == State.IDLE)
        {
            if (Vector2.Distance(transform.position, target.position) < config.AggroRange)
            {
                var hit = Physics2D.Raycast(transform.position, targetDir, 100, aggroLayerMask);

                if (hit != null && hit.transform == target)
                {
                    state = State.ATTACK;
                }
            }
        }
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
        weapon.LookAt(target.position);
        if (Vector2.Distance(target.position, transform.position) < config.AttackRange)
        {
            targetPos = transform.position;
            weapon.Attack();
        }
        else
        {
            targetPos = target.position;
        }
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
            rb.velocity = moveDir.normalized * config.MoveSpeed;
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
