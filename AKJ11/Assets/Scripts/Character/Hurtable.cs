using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;

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

    public HealthScriptableObject healthConfig;

    private HealthScriptableObject config;

    private EnemyExperienceGainConfig expGainConfig;

    private bool invulnerable = false;
    private List<SpriteRenderer> spriteRenderers;
    private List<Color> origColors;

    private float damaged = -100f;

    private Experience playerExperience;

    public void Start()
    {
        if (healthConfig != null)
        {
            Initialize(healthConfig);
        }
    }

    public void Initialize(HealthScriptableObject config)
    {
        Initialize(config, null);
    }

    public void Initialize(HealthScriptableObject config, EnemyExperienceGainConfig expGainConfig)
    {
        this.config = config;
        this.expGainConfig = expGainConfig;
        if (config != null)
        {
            currentHealth = config.MaxHealth;
        }
        spriteRenderers = new List<SpriteRenderer>(GetComponentsInChildren<SpriteRenderer>());
        spriteRenderers.AddRange(GetComponents<SpriteRenderer>());
        origColors = spriteRenderers.Select(rend => rend.color).ToList();
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
        }

        tint();
    }

    public void Hurt(float damage)
    {
        Hurt(damage, null);
    }

    public void Hurt(float damage, Experience playerExp)
    {
        if(playerExp != null)
        {
            playerExperience = playerExp;
        }

        if (!invulnerable)
        {
            currentHealth -= damage;
            if (currentHealth <= 0)
            {
                if (deadAction != null)
                {
                    deadAction.Invoke();
                }

                if (playerExp != null && expGainConfig != null)
                {
                    playerExperience.AddExperience(expGainConfig.GainedExperience);
                }
            }
            else
            {
                if (config.InvulnerabilityDuration > 0.001f)
                {
                    invulnerable = true;
                    Invoke("DisableInvulnerability", config.InvulnerabilityDuration);
                }
                damaged = Time.time;
            }
        }
    }

    public void DisableInvulnerability()
    {
        invulnerable = false;
    }

    private void tint()
    {
        var t = (Time.time - damaged) / config.DamageTintDuration;
        if (t > 0.0f && t <= 1.0f)
        {
            var index = 0;
            foreach (var rend in spriteRenderers)
            {
                var color = Color.Lerp(config.DamageTint, origColors[index++], t);
                rend.color = color;
            }
        }
        else
        {
            var index = 0;
            spriteRenderers.ForEach(rend => rend.color = origColors[index++]);
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
