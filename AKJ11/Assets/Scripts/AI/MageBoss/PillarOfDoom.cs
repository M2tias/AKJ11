using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PillarOfDoom : MonoBehaviour
{
    public GameSoundType ExplodeSound;

    private Collider2D coll;

    private float damageStart = 2.0f;
    private float damageEnd = 2.25f;
    private float damage = 8.0f;

    // Start is called before the first frame update
    void Start()
    {
        coll = GetComponent<Collider2D>();
        DisableDamage();
        Invoke("EnableDamage", damageStart);
        Invoke("DisableDamage", damageEnd);
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void EnableDamage()
    {
        if (SoundManager.main != null)
        {
            SoundManager.main.PlaySound(ExplodeSound);
        }
        coll.enabled = true;
    }

    private void DisableDamage()
    {
        coll.enabled = false;
    }

    void OnTriggerStay2D(Collider2D other)
    {
        var hurtable = other.GetComponent<Hurtable>();
        if (hurtable != null)
        {
            hurtable.Hurt(damage);
        }
    }

}
