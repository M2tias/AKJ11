using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "SpellBaseConfig", menuName = "Configs/SpellBaseConfig", order = 0)]
public class SpellBaseConfig : ScriptableObject
{
    [field: SerializeField]
    public List<SpellStats> VisibleStats { get; private set; }

    [field: SerializeField]
    public List<int> Damage { get; private set; }

    [field: SerializeField]
    public List<float> Aoe { get; private set; }

    [field: SerializeField]
    public List<int> Bounces { get; private set; }

    [field: SerializeField]
    public List<int> Dot { get; private set; }

    [field: SerializeField]
    public List<int> DotTickDamage { get; private set; }

    [field: SerializeField]
    public List<int> Piercing { get; private set; }

    [field: SerializeField]
    public List<float> Cooldown { get; private set; }

    [field: SerializeField]
    public bool IsSpellWall { get; private set; }

    [field: SerializeField]
    public bool IsUnlocked { get; private set; }

    [field: SerializeField]
    public List<float> Speed { get; private set; }

    [field: SerializeField]
    public Sprite ProjectileSprite { get; private set; }
}

public enum SpellStats
{
    Damage,
    Aoe,
    Bounces,
    Dot,
    DotTickDamage,
    Piercing,
    Cooldown
}