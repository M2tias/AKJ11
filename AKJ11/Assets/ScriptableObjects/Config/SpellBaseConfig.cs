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
    public int Damage { get; private set; }

    [field: SerializeField]
    public int Aoe { get; private set; }

    [field: SerializeField]
    public int Bounces { get; private set; }

    [field: SerializeField]
    public int Dot { get; private set; }

    [field: SerializeField]
    public int DotTickRate { get; private set; }

    [field: SerializeField]
    public int Piercing { get; private set; }

    [field: SerializeField]
    public float Cooldown { get; private set; }

    [field: SerializeField]
    public bool IsSpellWall { get; private set; }

    [field: SerializeField]
    public bool IsUnlocked { get; private set; }

    [field: SerializeField]
    public float Speed { get; private set; }

    [field: SerializeField]
    public Sprite ProjectileSprite { get; private set; }
}

public enum SpellStats
{
    Damage,
    Aoe,
    Bounces,
    Dot,
    DotTickRate,
    Piercing,
    Cooldown
}