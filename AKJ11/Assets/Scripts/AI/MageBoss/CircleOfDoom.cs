using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleOfDoom : MonoBehaviour
{
    public GameSoundType FireSound;

    private Collider2D coll;
    private Transform target;

    private float trackingDuration = 1.0f;
    private float damageStart = 1.25f;
    private float damageEnd = 6.0f;
    private bool tracking = true;
    private float damage = 15.0f;

    // Start is called before the first frame update
    void Start()
    {
        coll = GetComponent<Collider2D>();
        DisableDamage();
    }

    // Update is called once per frame
    void Update()
    {
        if (tracking)
        {
            transform.position = target.position;
        }
    }

    public void Launch(Transform target)
    {
        this.target = target;
        Invoke("StopTracking", trackingDuration);
        Invoke("EnableDamage", damageStart);
        Invoke("DisableDamage", damageEnd);
    }

    private void StopTracking()
    {
        tracking = false;
    }

    private void EnableDamage()
    {
        coll.enabled = true;

        if (SoundManager.main != null)
        {
            SoundManager.main.PlaySound(FireSound);
        }
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
