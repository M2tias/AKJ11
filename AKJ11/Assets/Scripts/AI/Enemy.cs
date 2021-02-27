using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    private float pathingFrequency = 0.1f;
    

    [SerializeField]
    private float moveSpeed;

    [SerializeField]
    private float attackRange = 1.0f;

    private Transform target;
    private Weapon weapon;

    private NavMeshPath path;
    private int cornerIndex;

    private Rigidbody2D rb;
    private Vector2 targetPos;
    private Vector2 moveTargetPos;



    // Start is called before the first frame update
    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
        InvokeRepeating("UpdatePathing", pathingFrequency, pathingFrequency);
        rb = GetComponent<Rigidbody2D>();
        weapon = GetComponentInChildren<Weapon>();
    }

    // Update is called once per frame
    void Update()
    {
        targetPos = target.position;
        weapon.LookAt(targetPos);
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

        if (Vector2.Distance(target.position, transform.position) < attackRange)
        {
            weapon.Attack();
        }
    }

    void FixedUpdate()
    {
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
