using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HealthConfig", menuName = "Configs/Hurtable/HealthConfig", order = 0)]
public class HealthScriptableObject : ScriptableObject
{
    [SerializeField]
    private float health;
    public float MaxHealth { get => health; }

    [field: SerializeField]
    public float InvulnerabilityDuration { get; private set; }

    [field: SerializeField]
    public Color DamageTint { get; private set; }

    [field: SerializeField]
    public float DamageTintDuration { get; private set; }

    [field: SerializeField]
    public GameSoundType HitSound { get; private set; }

    [field: SerializeField]
    public GameSoundType DeathSound { get; private set; }

    public bool ImmuneToPoison = false;
    public bool ShowDamageNumber = true;

}
