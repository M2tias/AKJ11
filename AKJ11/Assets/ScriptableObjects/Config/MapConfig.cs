
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[CreateAssetMenu(fileName = "MapConfig", menuName = "Configs/MapConfig")]
public class MapConfig : ScriptableObject
{

    [field:SerializeField]
    [field: Range(0, 4)]
    public int NumberOfAreas {get; private set;} = 4;

    [field: SerializeField]
    public int Size { get; private set; } = 20;

    [field: SerializeField]
    [field: Range(1, 5)]
    public int PassageRadius { get; private set; } = 1;

    [field: SerializeField]
    [field: Range(1, 10)]
    public int Padding { get; private set; } = 1;

    [field: SerializeField]
    [field: Range(5, 20)]
    public int TowerRadius { get; private set; } = 5;

    [field: SerializeField]
    public List<EnemySpawn> Spawns { get; private set; }

    [field: SerializeField]
    public List<ItemSpawn> Items { get; private set; }

    [field: SerializeField]
    public TileStyle DefaultTileStyle {get; private set;}

    [field: SerializeField]
    public TileStyle CaveTileStyle {get; private set;}
    /*[field: SerializeField]
    public TileStyle TowerTileStyle {get; private set;}*/

    public Sprite GetSprite(int configuration, TileStyle style) {
        //int lookedUp = marchingSquareLookup[configuration];
        int lookedUp = configuration;
        return style.Cases[lookedUp];
    } 
}


[System.Serializable]
public class EnemySpawn
{
    [field: SerializeField]
    public int SpawnThisManyTimes { get; private set; } = 1;

    [field: SerializeField]
    public SpawnPosition SpawnPosition { get; private set; } = SpawnPosition.Cave;

    [field: SerializeField]
    public List<EnemyConfig> Enemies { get; private set; }
}

public enum SpawnPosition {
    Cave,
    Tower
}


[System.Serializable]
public class ItemSpawn
{
    [field: SerializeField]
    public int SpawnThisManyTimes { get; private set; } = 1;

    [field: SerializeField]
    public SpawnPosition SpawnPosition { get; private set; } = SpawnPosition.Cave;

    [field: SerializeField]
    public ResourceGainConfig Item {get; private set;}
}

