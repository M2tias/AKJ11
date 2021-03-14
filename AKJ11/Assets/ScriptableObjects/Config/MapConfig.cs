
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[CreateAssetMenu(fileName = "MapConfig", menuName = "Configs/MapConfig")]
public class MapConfig : ScriptableObject
{

    [field: SerializeField]
    [field: Range(0, 4)]
    public int NumberOfAreas { get; private set; } = 4;


    [SerializeField]
    private SpawnPosition areaType =  SpawnPosition.Cave;
    public SpawnPosition AreaType { get {return areaType;} }

    [field: SerializeField]
    public ConnectionStrategy ConnectionStrategy {get; private set;} = ConnectionStrategy.First;

    [SerializeField]
    [ConditionalHide("areaType", SpawnPosition.Tower)]
    private int circularAreaRadius = 5;
    public int CircularAreaRadius { get { return circularAreaRadius; } }


    [field: SerializeField]
    public int Size { get; private set; } = 20;

    public int PassageRadius { get; private set; } = 1;

    [field: SerializeField]
    [field: Range(1, 10)]
    public int Padding { get; private set; } = 1;

    [field: SerializeField]
    [field: Range(5, 20)]
    public int TowerRadius { get; private set; } = 5;

    [field: SerializeField]
    public List<GameEntitySpawn> EntitySpawns { get; private set; }

    [field: SerializeField]
    public List<ItemSpawn> Items { get; private set; }

    [field: SerializeField]
    public TileStyle CaveTileStyle { get; private set; }

    [field: SerializeField]
    public KeySpawn KeySpawn {get; private set;}

    public Sprite GetSprite(int configuration, TileStyle style)
    {
        int lookedUp = configuration;
        return style.Cases[lookedUp];
    }
}

[System.Serializable]
public class GameEntitySpawn
{
    [field: SerializeField]
    [field: Range(1, 30)]
    public int Amount { get; private set; } = 1;

    [SerializeField]
    private SpawnPosition area = SpawnPosition.Cave;
    public SpawnPosition Area { get {return area;}}

    [SerializeField]
    [ConditionalHide("area", SpawnPosition.Cave)]
    private SpawnStrategy location = SpawnStrategy.Random;
    public SpawnStrategy Location { get {return location;} }

    [SerializeField]
    [ConditionalHide("area", SpawnPosition.Tower)]
    [Range(2, 5)]
    private float minPlayerDistance = 2f;
    public float MinimumDistanceFromPlayer {get {return minPlayerDistance;}}

    [field: SerializeField]
    public List<GameEntityConfig> Entities { get; private set; }
}

public enum SpawnPosition
{
    Cave,
    Tower
}

public enum SpawnStrategy {
    Random,
    MaxDistanceFromPlayer,
    CenterOfTheEnclosure
}


public enum KeySpawn {
    MaxDistanceFromPlayer,
    LastEntityDrops,
    Manual
}

[System.Serializable]
public class ItemSpawn
{
    [field: SerializeField]
    [field: Range(1, 30)]
    public int SpawnThisManyTimes { get; private set; } = 1;

    [SerializeField]
    [ConditionalHide("spawnPosition", SpawnPosition.Tower)]
    private int minPlayerDistance = 2;
    public int MinPlayerDistance { get {return minPlayerDistance;}}

    [SerializeField]
    private SpawnPosition spawnPosition = SpawnPosition.Cave;
    public SpawnPosition SpawnPosition { get {return spawnPosition;}}

    [field: SerializeField]
    public PickupableItemScriptableObject Item { get; private set; }
}

