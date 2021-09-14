using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;

public class Hurtable : MonoBehaviour
{
    private float currentHealth = 5;
    public float Health { get { return currentHealth; } }
    private float damageTaken = 0;
    public float DamageTaken { get { return damageTaken; } }
    private float dotDamage = 0;
    private float dotDuration = 0;
    private float dotStarted = 0;
    private float dotLastDamage = 0;
    private float dotPeriod = 1;

    private GameEntityEnemy enemy;

    [SerializeField]
    private UnityEvent<float> damagedCallback;

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

    public Color DotTint;

    private bool dotting = false;
    private bool initialized = false;

    private Vector2 xpOffset = new Vector2(-0.75f, 0.5f);
    private Vector2 dmgOffset = new Vector2(0.2f, 0.2f);

    public bool Immune = false;

    private UnityAction deathAction;

    public void Start()
    {
        if (healthConfig != null)
        {
            Initialize(healthConfig);
        }
    }

    public void Initialize(HealthScriptableObject config)
    {
        Initialize(config, null, null);
    }

    public void Initialize(HealthScriptableObject config, EnemyExperienceGainConfig expGainConfig, UnityAction deathAction)
    {
        this.deathAction = deathAction;
        this.config = config;
        this.expGainConfig = expGainConfig;
        if (config != null)
        {
            currentHealth = config.MaxHealth;
        }
        spriteRenderers = new List<SpriteRenderer>(GetComponentsInChildren<SpriteRenderer>());
        spriteRenderers.AddRange(GetComponents<SpriteRenderer>());
        origColors = spriteRenderers.Select(rend => rend.color).ToList();
        if (gameObject.tag == "Player")
        {
            if (UIHealth.main != null)
            {
                UIHealth.main.SetHp(config.MaxHealth, healthConfig.MaxHealth);
            }
        }

        enemy = GetComponent<GameEntityEnemy>();
        initialized = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!initialized)
        {
            return;
        }
        if (dotDamage > 0 && Time.time - dotStarted > dotDuration)
        {
            dotDamage = 0;
            dotStarted = 0;
            dotting = false;
        }

        if (dotDamage > 0 && Time.time - dotLastDamage > dotPeriod)
        {
            Hurt(dotDamage);
            dotLastDamage = Time.time;

            if (dotStarted + dotDuration < Time.time + dotPeriod)
            {
                dotting = false;
            }
        }

        tint();
    }

    public bool Hurt(float damage)
    {
        return Hurt(damage, null);
    }

    public bool Hurt(float damage, Experience playerExp)
    {
        bool wasKilledByDamage = false;
        if (Immune)
        {
            return false;
        }

        if (enemy != null)
        {
            enemy.Damaged();
        }

        if (playerExp != null)
        {
            playerExperience = playerExp;
        }

        if (SoundManager.main != null)
        {
            SoundManager.main.PlaySound(config.HitSound);
        }

        if (!invulnerable)
        {
            if (damagedCallback != null)
            {
                damagedCallback.Invoke(damage);
            }
            currentHealth -= damage;
            if (UIWorldCanvas.main != null && config.ShowDamageNumber)
            {
                UIWorldCanvas.main.ShowNumber((Vector2)transform.position + dmgOffset, -damage, ResourceType.HP, false);
            }
            damageTaken += damage;
            if (currentHealth <= 0)
            {
                Die();
                wasKilledByDamage = true;
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
        if (gameObject.tag == "Player")
        {
            if (UIHealth.main != null)
            {
                UIHealth.main.SetHp(currentHealth, healthConfig.MaxHealth);
            }
        }
        return wasKilledByDamage;
    }

    public void Die()
    {
        if (deadAction != null)
        {
            deadAction.Invoke();
        }

        if (deathAction != null)
        {
            deathAction.Invoke();
        }

        if (SoundManager.main != null)
        {
            SoundManager.main.PlaySound(config.DeathSound);
        }

        if (playerExperience != null && expGainConfig != null)
        {
            Experience.main.AddExperience(expGainConfig.GainedExperience);
            if (UIWorldCanvas.main != null)
            {
                UIWorldCanvas.main.ShowNumber((Vector2)transform.position + xpOffset, expGainConfig.GainedExperience, ResourceType.XP);
            }
        }
    }

    public void GainHealth(int amount)
    {
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, config.MaxHealth);
        if (gameObject.tag == "Player")
        {
            if (UIHealth.main != null)
            {
                UIHealth.main.SetHp(currentHealth, healthConfig.MaxHealth);
            }
        }
    }

    public bool HasMaxHealth() {
        return currentHealth >= config.MaxHealth;
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
                var origColor = dotting ? DotTint : origColors[index++];
                var color = Color.Lerp(config.DamageTint, origColor, t);
                rend.color = color;
            }
        }
        else
        {
            var index = 0;
            foreach (var rend in spriteRenderers)
            {
                var origColor = dotting ? DotTint : origColors[index++];
                rend.color = origColor;
            }
        }

    }

    public void Dot(float damage, float duration)
    {
        if (config.ImmuneToPoison || Immune)
        {
            return;
        }

        if (dotting)
        {
            dotStarted = Time.time;
            dotDuration = duration;
        }
        else
        {
            dotDamage = damage;
            dotDuration = duration;
            dotStarted = Time.time;
            dotLastDamage = Time.time;
            if (damage > 0.1f)
            {
                dotting = true;
            }
        }
    }

}
