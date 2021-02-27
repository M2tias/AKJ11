
using UnityEngine;
using System.Collections.Generic;


[CreateAssetMenu(fileName = "MapConfig", menuName = "Configs/MapConfig")]
public class MapConfig : ScriptableObject
{
    [field: SerializeField]
    public int Size { get; private set; } = 20;

    [field: SerializeField]
    [field: Range(1, 5)]
    public int PassageRadius { get; private set; } = 1;

    [field: SerializeField]
    [field: Range(0, 10)]
    public int Padding { get; private set; } = 1;

    [field: SerializeField]
    public CaveConfig Cave { get; private set; }

    [field: SerializeField]
    public TowerRoomConfig Tower { get; private set; }

    [field: SerializeField]
    public Sprite GroundSprite { get; private set; }

    [field: SerializeField]
    public Sprite WallSprite { get; private set; }

    [field: SerializeField]
    public List<EnemySpawn> Spawns { get; private set; }
}


[System.Serializable]
public class EnemySpawn
{
    [field: SerializeField]
    public int SpawnThisManyTimes { get; private set; } = 1;

    [field: SerializeField]
    public List<EnemyConfig> Enemies { get; private set; }
}
