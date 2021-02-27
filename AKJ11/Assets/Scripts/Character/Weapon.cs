using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public bool DebugAttack;

    public float damage;
    public Type type;
    public Projectile projectile;
    public Transform projectileRoot;

    private Transform root;
    private Animator animator;
    private bool playingAttackAnimation = false;
    private bool attacking = false;

    private float maxRotateSpeed = 1000;

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
        if (!attacking)
        {
            var dir = position - (Vector2)root.position;
            var angle = Vector2.SignedAngle(root.up, dir);
            angle = Mathf.Sign(angle) * Mathf.Min(maxRotateSpeed * Time.deltaTime, Mathf.Abs(angle));
            root.Rotate(Vector3.forward, angle);
        }
    }

    public void Attack()
    {
        if (!playingAttackAnimation)
        {
            animator.SetBool(getAnimationFor(type), true);
            attacking = true;
            playingAttackAnimation = true;
        }
    }

    void AttackDone()
    {
        attacking = false;
    }

    void AttackAnimationDone()
    {
        foreach(var enumType in Enum.GetValues(typeof(Type)))
        {
            animator.SetBool(getAnimationFor((Type)enumType), false);
        }
        playingAttackAnimation = false;
    }

    void LaunchProjectile()
    {
        if (projectile != null)
        {
            var proj = Instantiate(projectile);
            proj.transform.position = projectileRoot.position;
            proj.Launch(transform.position + root.up, damage);
        }
    }

    string getAnimationFor(Type type)
    {
        switch(type)
        {
            case Type.THRUST:
                return "Thrust";
            case Type.SLASH:
                return "Slash";
            case Type.SHOOT:
                return "Shoot";
            default:
                return "";
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        var hurtable = other.GetComponent<Hurtable>();
        if (hurtable != null)
        {
            hurtable.Hurt(damage);
        }
    }


}

public enum Type
{
    THRUST,
    SLASH,
    SHOOT
}
