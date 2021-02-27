using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Hurtable : MonoBehaviour
{
    private float currentHealth = 5;
    private float dotDamage = 0;
    private float dotDuration = 0;
    private float dotStarted = 0;
    private float dotLastDamage = 0;
    private float dotPeriod = 1;

    [SerializeField]
    private UnityEvent deadAction;

    private EnemyConfig config;

    public void Initialize(EnemyConfig config)
    {
        this.config = config;
        if (config.HealthConfig != null)
        {
            currentHealth = config.HealthConfig.MaxHealth;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (dotDamage > 0 && Time.time - dotStarted > dotDuration)
        {
            dotDamage = 0;
            dotStarted = 0;
        }

        if(dotDamage > 0 && Time.time - dotLastDamage > dotPeriod)
        {
            Hurt(dotDamage);
            dotLastDamage = Time.time;
            Debug.Log(currentHealth);
        }
    }

    public void Hurt(float damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            if (deadAction != null)
            {
                deadAction.Invoke();
            }
        }
    }

    public void Dot(float damage, float duration)
    {
        dotDamage = damage;
        dotDuration = duration;
        dotStarted = Time.time;
        dotLastDamage = Time.time;
    }
}
