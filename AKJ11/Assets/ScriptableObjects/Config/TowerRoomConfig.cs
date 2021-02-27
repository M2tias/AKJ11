
using UnityEngine;
using System.Collections.Generic;


[CreateAssetMenu(fileName = "TowerRoomConfig", menuName = "Configs/TowerRoomConfig")]
public class TowerRoomConfig : ScriptableObject
{
    [field: SerializeField]
    [field: Range(5, 70)]
    public int Radius {get; private set;} = 5;

    [field: SerializeField]
    [field: Range(1, 10)]
    public int Buffer {get; private set;} = 1;

}
