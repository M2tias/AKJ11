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

}
