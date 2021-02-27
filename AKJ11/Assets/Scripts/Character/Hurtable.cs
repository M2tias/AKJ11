using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Hurtable : MonoBehaviour
{
    private float currentHealth = 5;
    [SerializeField]
    private UnityEvent deadAction;

    private EnemyConfig config;

    public void Initialize(EnemyConfig config)
    {
        this.config = config;
        if (config.HealthConfig != null) {
            currentHealth = config.HealthConfig.MaxHealth;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Hurt(float damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            if (deadAction != null) {
                deadAction.Invoke();
            }
        }
    }
}
