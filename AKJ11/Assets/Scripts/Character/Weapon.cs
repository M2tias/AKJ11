using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public bool DebugAttack;

    private float damage;
    private WeaponType type;
    private Projectile projectile;
    public Transform projectileRoot;

    private Transform root;
    private Animator animator;
    private SpriteRenderer rend;
    private bool playingAttackAnimation = false;
    private bool attacking = false;

    private float maxRotateSpeed = 1000;

    private WeaponConfig config;

    public void Initialize(WeaponConfig config)
    {
        this.config = config;
        root = transform;
        animator = GetComponentInChildren<Animator>();
        rend = GetComponentInChildren<SpriteRenderer>();

        rend.sprite = config.WeaponSprite;
        damage = config.Damage;
        projectile = config.Projectile;
        type = config.type;
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
        foreach(var enumType in Enum.GetValues(typeof(WeaponType)))
        {
            animator.SetBool(getAnimationFor((WeaponType)enumType), false);
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

    string getAnimationFor(WeaponType type)
    {
        switch(type)
        {
            case WeaponType.THRUST:
                return "Thrust";
            case WeaponType.SLASH:
                return "Slash";
            case WeaponType.SHOOT:
                return "Shoot";
            case WeaponType.GATLING:
                return "Burst";
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

public enum WeaponType
{
    THRUST,
    SLASH,
    SHOOT,
    GATLING
}
