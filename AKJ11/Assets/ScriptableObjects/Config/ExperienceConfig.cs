using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "ExperienceConfig", menuName = "Configs/ExperienceConfig", order = 0)]
public class ExperienceConfig : ScriptableObject
{
    [field: SerializeField]
    public List<int> ExpPerLevel { get; private set; }

    [field: SerializeField]
    public List<int> StatPointsPerLevel { get; private set; }

}