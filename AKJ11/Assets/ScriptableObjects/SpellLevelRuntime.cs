using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "SpellLevelRuntime", menuName = "Runtimes/SpellLevelRuntime", order = 0)]
public class SpellLevelRuntime : ScriptableObject
{
    [field: SerializeField]
    public int DamageLevel { get; set; }

    [field: SerializeField]
    public int AoeLevel { get; set; }

    [field: SerializeField]
    public int BouncesLevel { get; set; }

    [field: SerializeField]
    public int DotLevel { get; set; }

    [field: SerializeField]
    public int DotTickDamageLevel { get; set; }

    [field: SerializeField]
    public int PiercingLevel { get; set; }

    [field: SerializeField]
    public int CooldownLevel { get; set; }

    [field: SerializeField]
    public int SpeedLevel { get; set; }

    [field: SerializeField]
    public bool IsUnlocked { get; set; }

    public void Initialize()
    {
        DamageLevel = 0;
        AoeLevel = 0;
        BouncesLevel = 0;
        DotLevel = 0;
        DotTickDamageLevel = 0;
        PiercingLevel = 0;
        CooldownLevel = 0;
        SpeedLevel = 0;
        IsUnlocked = false;
    }
}