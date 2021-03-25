using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class MapPopulator
{
    public static void Populate(MapGenData data)
    {
        PlayerCharacter player = data.Player;
        player.SetPosition(data.MidPointNode);

        if (Configs.main.Campaign.IsLastLevel(data.Config))
        {
            TheEndView.main.Show(Configs.main.Campaign.EndMessage);
        }
        else
        {
            PlaceKeyAndEntrance(data);
        }

        SpawnGameEntities(data);

        SetUpCamera();
    }

    public static void SetUpCamera()
    {
        FollowTarget cameraFollow = Camera.main.GetComponent<FollowTarget>();
        if (cameraFollow != null)
        {
            cameraFollow.Initialize(Configs.main.Camera, true);
        }
        else
        {
            Debug.Log("<color=red>Camera doesn't have FollowTarget component!</color>");
        }
    }

    private static void PlaceKeyAndEntrance(MapGenData data)
    {
        List<MapNode> caveNodes = data.CaveNodes;
        List<MapNode> towerNodes = data.Tower.Nodes;
        MapNode keyNode = null;
        if (data.Config.KeySpawn == KeySpawn.MaxDistanceFromPlayer || data.Config.KeySpawn == KeySpawn.Manual) {
            if (caveNodes.Count > 0)
            {
                keyNode = caveNodes.OrderByDescending(node => node.Distance(data.Player.Node)).First();
                PlaceNextLevelTrigger(data.MidPointNode);
            }
            else
            {
                keyNode = towerNodes.OrderByDescending(node => node.Distance(data.Player.Node)).First();
                MapNode enterNode = towerNodes.OrderByDescending(node => node.Distance(keyNode)).First();
                PlaceNextLevelTrigger(enterNode);
            }
        }

        if (data.Config.KeySpawn == KeySpawn.MaxDistanceFromPlayer)
        {
            PlaceKey(keyNode);
        }
    }

    public static void PlaceNextLevelTrigger(MapNode node) {
        NextLevelTrigger nextLevelTrigger = Prefabs.Get<NextLevelTrigger>();
        nextLevelTrigger.transform.SetParent(MapGenerator.main.GetContainer());
        nextLevelTrigger.Initialize(node.Position);
    }

    public static void PlaceKey(MapNode node) {
        PlaceKey(node.Position);
    }

    public static void PlaceKey(Vector2 position)
    {
        NextLevelKey nextLevelKey = Prefabs.Get<NextLevelKey>();
        nextLevelKey.transform.SetParent(NextLevelTrigger.main.transform.parent);
        nextLevelKey.Initialize(position, NextLevelTrigger.main);
    }

    private static void SpawnGameEntities(MapGenData data)
    {
        int entitiesToSpawn = data.Config.EntitySpawns.Sum(entitySpawn => entitySpawn.Amount * entitySpawn.Entities.Count);
        int entitiesSpawned = 0;
        RandomNumberGenerator rng = RandomNumberGenerator.GetInstance();
        foreach (GameEntitySpawn entitySpawn in data.Config.EntitySpawns)
        {
            for (int spawnCount = 0; spawnCount < entitySpawn.Amount; spawnCount += 1)
            {
                foreach (GameEntityConfig entityConfig in entitySpawn.Entities)
                {
                    try
                    {
                        MapNode spawnNode = DetermineEntitySpawnNode(data, entitySpawn, rng);
                        if (spawnNode == null)
                        {
                            return;
                        }
                        SpawnEntity(entityConfig, spawnNode, data.NodeContainer.ViewContainer);
                        entitiesSpawned += 1;
                    }
                    catch (Exception e)
                    {
                        MonoBehaviour.print(e);
                    }
                }
            }
        }
        if (entitiesToSpawn > 0)
        {
            if (entitiesSpawned == entitiesToSpawn)
            {
                MonoBehaviour.print($"Succesfully spawned all {entitiesToSpawn} entities!");
            }
            else
            {
                MonoBehaviour.print($"Spawned {entitiesSpawned}/{entitiesToSpawn} entities.");
            }
        }
    }

    private static MapNode DetermineEntitySpawnNode(MapGenData data, GameEntitySpawn entitySpawn, RandomNumberGenerator rng)
    {
        MapNode node = null;
        if (entitySpawn.Location == SpawnStrategy.Random)
        {
            if (entitySpawn.CanSpawnInHallways) {
                List<MapNode> possibleNodes =  data.NonTowerNodes.Where(node => !node.MapGen.EntitySpawnsHere).ToList();
                node = possibleNodes[rng.Range(0, data.NonTowerNodes.Count)];
            } else {
                List<MapNode> possibleNodes =  data.CaveNodes.Where(node => !node.MapGen.EntitySpawnsHere).ToList();
                node = possibleNodes[rng.Range(0, data.CaveNodes.Count)];
            }
        }
        else if (entitySpawn.Location == SpawnStrategy.CenterOfTheEnclosure)
        {
            CaveEnclosure enclosure = DetermineEnclosure(data, entitySpawn, rng);
            node = enclosure.Nodes.OrderBy(node => enclosure.DistanceFromMidPoint(node)).FirstOrDefault();
        }
        else if (entitySpawn.Location == SpawnStrategy.MaxDistanceFromPlayer)
        {
            CaveEnclosure enclosure = DetermineEnclosure(data, entitySpawn, rng);
            node = enclosure.Nodes.OrderByDescending(node => node.Distance(data.Player.Node)).FirstOrDefault();
        }
        if (node == null) {
            MonoBehaviour.print($"Something went wrong, Entity {entitySpawn} doesn't have a spawnNode.");
        }
        return node;
    }

    private static CaveEnclosure DetermineEnclosure(MapGenData data, GameEntitySpawn entitySpawn, RandomNumberGenerator rng) {
        CaveEnclosure enclosure = entitySpawn.Area == SpawnPosition.Cave ? data.Rooms[rng.Range(0, data.Rooms.Count)] : data.Tower;
        if (enclosure == null)
        {
            MonoBehaviour.print($"Could not find a room. Count: {data.Rooms.Count}. Also tower: {data.Tower}");
            return null;
        }
        return enclosure;
    }

    private static GameEntity SpawnEntity(GameEntityConfig entityConfig, MapNode spawnNode, Transform parent)
    {
        GameEntity gameEntity = MonoBehaviour.Instantiate(entityConfig.EntityPrefab).GetComponent<GameEntity>();
        spawnNode.MapGen.EntitySpawnsHere = true;
        gameEntity.Initialize(entityConfig, spawnNode);
        gameEntity.transform.SetParent(parent);
        gameEntity.name = entityConfig.name;
        gameEntity.WakeUp();
        return gameEntity;
    }

}
public struct MapGenData
{
    public NodeContainer NodeContainer;
    public MapConfig Config;
    public CaveEnclosure Tower;
    public List<CaveEnclosure> Rooms;

    public CaveEnclosure RoomsAndTower;
    private PlayerCharacter player;
    public PlayerCharacter Player
    {
        get
        {
            if (player == null)
            {
                player = PlayerCharacter.GetInstance();
            }
            return player;
        }
    }

    public MapNode MidPointNode { get { return NodeContainer.GetNode(NodeContainer.MidPoint); } }

    public List<MapNode> CaveNodes
    {
        get { return Rooms.SelectMany(x => x.Nodes).ToList(); }
    }

    private List<MapNode> nonTowerNodes;
    public List<MapNode> NonTowerNodes {
        get {
            if (nonTowerNodes == null) {
                CaveEnclosure tower = Tower;
                nonTowerNodes = RoomsAndTower.Nodes.Where(node => (!tower.Nodes.Contains(node))).ToList();
            }
            return nonTowerNodes;
        }
    }
}