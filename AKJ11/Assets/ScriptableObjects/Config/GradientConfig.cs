using UnityEngine;
using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

[CreateAssetMenu(fileName = "GradientConfig", menuName = "Configs/GradientConfig", order = 0)]
public class GradientConfig : ScriptableObject
{
    [SerializeField]
    private List<Color> spellLevelColors;

    public Color GetColor(int index) {
        if (spellLevelColors != null && index > -1 && index < spellLevelColors.Count) {
            return spellLevelColors[index];
        }
        return Color.white;
    }

    [SerializeField]
    private Gradient HealthGradient;

    public Color GetHealthColorAt(float percentage) {
        return HealthGradient.Evaluate(percentage);
    }
}
