using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BossHealer : MonoBehaviour
{
    public HealProjectile projectile;
    public BossShield shield;

    public bool IsAlive = true;

    private float healInterval = 0.5f;

    private bool sleeping = true;
    private bool initialized = false;
    private bool isHealing;
    
    private Boss healTarget;

    private List<BossHealerPart> parts = new List<BossHealerPart>();

    public void RegisterPart(BossHealerPart part)
    {
        parts.Add(part);
    }

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

        initialized = true;
        shield.Activate();
    }

    public void WakeUp()
    {
        sleeping = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (IsAlive && parts.Count > 0 && parts.Where(part => part.IsAlive).Count() == 0)
        {
            Die();
        }
    }

    public void Die()
    {
        IsAlive = false;
        shield.Deactivate();
    }

    public void StartHealing()
    {
        if (!IsAlive)
        {
            return;
        }

        isHealing = true;
        Invoke("LaunchHeal", healInterval);
        parts.ForEach(part => part.Activate());
        shield.Deactivate();
    }

    public void StopHealing()
    {
        if (!IsAlive)
        {
            return;
        }

        isHealing = false;
        parts.ForEach(part => part.Deactivate());
        shield.Activate();
    }

    public void SetHealTarget(Boss target)
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
        proj.Launch(healTarget);
        Invoke("LaunchHeal", healInterval);
    }
}
