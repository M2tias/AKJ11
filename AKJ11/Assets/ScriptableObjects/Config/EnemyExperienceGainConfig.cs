using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyExperienceGainConfig", menuName = "Configs/EnemyExperienceGainConfig", order = 0)]
public class EnemyExperienceGainConfig : ScriptableObject
{
    [SerializeField]
    private int experience;
    public int GainedExperience { get => experience; }
}
