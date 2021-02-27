
using UnityEngine;
using System.Collections.Generic;


[CreateAssetMenu(fileName = "CaveConfig", menuName = "Configs/CaveConfig")]
public class CaveConfig : ScriptableObject
{
    [field: SerializeField]
    public int Size { get; private set; } = 20;
    [field: SerializeField]
    public int PercentWalls { get; private set; } = 40;
    [field: SerializeField]
    public int CavernRuns { get; private set; } = 3;

}
