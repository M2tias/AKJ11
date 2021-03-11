using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class MapPopulator
{
    public static PlayerCharacter Populate(MapGenData data)
    {
        PlayerCharacter player = data.Player;
        player.SetPosition(data.MidPointNode);

        if (Configs.main.Campaign.IsLastLevel(data.Config))
        {
            Debug.Log("LastLevel");
            TheEndView.main.Show();
        }
        else
        {
            PlaceKeyAndEntrance(data);
        }

        SpawnGameEntities(data);
        //SpawnEnemies(data);

        SetUpCamera();
        return player;
    }

    private static void SetUpCamera()
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
        List<MapNode> nonTowerNodes = data.NonTowerNodes;
        List<MapNode> towerNodes = data.Tower.Nodes;
        NextLevelTrigger nextLevelTrigger = Prefabs.Get<NextLevelTrigger>();
        nextLevelTrigger.transform.SetParent(data.NodeContainer.ViewContainer);
        MapNode keyNode;
        if (nonTowerNodes.Count > 0)
        {
            keyNode = nonTowerNodes.OrderByDescending(node => node.Distance(data.Player.Node)).First();
            nextLevelTrigger.Initialize(data.NodeContainer.MidPoint);
        }
        else
        {
            keyNode = towerNodes.OrderByDescending(node => node.Distance(data.Player.Node)).First();
            MapNode enterNode = towerNodes.OrderByDescending(node => node.Distance(keyNode)).First();
            nextLevelTrigger.Initialize(enterNode.Position);
        }
        NextLevelKey nextLevelKey = Prefabs.Get<NextLevelKey>();
        nextLevelKey.transform.SetParent(data.NodeContainer.ViewContainer);
        nextLevelKey.Initialize(keyNode.Position, nextLevelTrigger);
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
                        CaveEnclosure enclosure = entitySpawn.Area == SpawnPosition.Cave ? data.Rooms[rng.Range(0, data.Rooms.Count)] : data.Tower;
                        if (enclosure == null)
                        {
                            MonoBehaviour.print($"Could not find a room. Count: {data.Rooms.Count}. Also tower: {data.Tower}");
                            return;
                        }
                        MapNode spawnNode = DetermineEntitySpawnNode(entitySpawn, enclosure, rng, data.Player.Node);
                        if (spawnNode == null)
                        {
                            MonoBehaviour.print($"Something went wrong, Entity {entityConfig.name} doesn't have a spawnNode.");
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

    private static MapNode DetermineEntitySpawnNode(GameEntitySpawn entitySpawn, CaveEnclosure enclosure, RandomNumberGenerator rng, MapNode playerNode)
    {
        MapNode node = null;
        if (entitySpawn.Location == SpawnStrategy.Random)
        {
            node = enclosure.Nodes[rng.Range(0, enclosure.Nodes.Count)];
        }
        else if (entitySpawn.Location == SpawnStrategy.CenterOfTheEnclosure)
        {
            node = enclosure.Nodes.OrderBy(node => enclosure.DistanceFromMidPoint(node)).FirstOrDefault();
        }
        else if (entitySpawn.Location == SpawnStrategy.MaxDistanceFromPlayer)
        {
            node = enclosure.Nodes.OrderByDescending(node => node.Distance(playerNode)).FirstOrDefault();
        }
        return node;
    }

    private static GameEntity SpawnEntity(GameEntityConfig entityConfig, MapNode spawnNode, Transform parent)
    {
        GameEntity gameEntity = MonoBehaviour.Instantiate(entityConfig.EntityPrefab).GetComponent<GameEntity>();
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

    public List<MapNode> NonTowerNodes
    {
        get { return Rooms.SelectMany(x => x.Nodes).ToList(); }
    }
}