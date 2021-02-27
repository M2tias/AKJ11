using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public bool DebugAttack;

    private Transform root;
    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        root = transform;
        animator = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (DebugAttack)
        {
            Attack();
            DebugAttack = false;
        }   
    }

    public void LookAt(Vector2 position)
    {
        var dir = position - (Vector2)root.position;
        var angle = Vector2.SignedAngle(root.up, dir);
        root.Rotate(Vector3.forward, angle);
    }

    public void Attack()
    {
        animator.SetBool("Thrust", true);
    }

    void AttackCallback()
    {
        animator.SetBool("Thrust", false);
    }
}
