using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossHealer : MonoBehaviour
{
    public HealProjectile projectile;

    public bool IsAlive = true;

    private float healInterval = 0.5f;

    private bool sleeping = true;
    private bool initialized = false;
    private bool isHealing;

    private Hurtable hurtable;
    private Collider2D coll;
    private Transform healTarget;


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
        if (node != null)
        {
            transform.position = (Vector2)node.Position;
        }
        
        hurtable = GetComponent<Hurtable>();
        hurtable.Immune = true;
        coll = GetComponent<Collider2D>();

        initialized = true;
    }

    public void WakeUp()
    {
        sleeping = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Die()
    {
        IsAlive = false;
        coll.enabled = false;
    }

    public void StartHealing()
    {
        if (!IsAlive)
        {
            return;
        }

        isHealing = true;
        hurtable.Immune = false;
        Invoke("LaunchHeal", healInterval);
    }

    public void StopHealing()
    {
        if (!IsAlive)
        {
            return;
        }

        isHealing = false;
        hurtable.Immune = true;
    }

    public void SetHealTarget(Transform target)
    {
        healTarget = target;
    }

    void LaunchHeal()
    {
        if (!isHealing || !IsAlive)
        {
            return;
        }
        var proj = Instantiate(projectile);
        proj.transform.position = transform.position;
        proj.Launch(healTarget.position);
        Invoke("LaunchHeal", healInterval);
    }
}
