using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossHealerPart : MonoBehaviour
{
    public bool IsAlive = true;
    public BossHealer healer;
    public ParticleSystem effect;

    private Collider2D coll;
    private Renderer rend;
    private Hurtable hurtable;


    // Start is called before the first frame update
    void Start()
    {
        healer.RegisterPart(this);

        coll = GetComponent<Collider2D>();
        rend = GetComponent<Renderer>();
        hurtable = GetComponent<Hurtable>();
        hurtable.Immune = true;
        coll.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsAlive)
        {
            effect.Stop();
            coll.enabled = false;
            hurtable.Immune = true;
        }
    }

    public void Die()
    {
        IsAlive = false;
        coll.enabled = false;
        rend.enabled = false;
        effect.Stop();
    }

    public void Activate()
    {
        if (IsAlive)
        {
            hurtable.Immune = false;
            coll.enabled = true;
            effect.Play();
        }
    }

    public void Deactivate()
    {
        hurtable.Immune = true;
        coll.enabled = false;
        effect.Stop();
    }
}
