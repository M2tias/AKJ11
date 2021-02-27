using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using Cysharp.Threading.Tasks;

public class MapGenerator : MonoBehaviour
{
    List<RectInt> cavernAreas;
    NodeContainer nodeContainer;
    MapConfig config;
    List<MapNode> towerNodes;

    [SerializeField]
    private GameObject playerPrefab;

    void Start()
    {
        config = Configs.main.Map;
        nodeContainer = new NodeContainer(0, 0, config.Size, config.Size);
        if (Configs.main.Debug.DelayGeneration) {
            nodeContainer.Render();
        }
        cavernAreas = MapAreaSplitter.GetSplitAreas(config);
        Generate();
    }

    async UniTask Generate() {
        await GenerateCaves();
        if (Configs.main.Debug.DelayGeneration) {
            nodeContainer.Render();
        }
        await GenerateTowerRoom();
        await FindAndConnectEnclosures();
        CreateNavMesh();
        Populate();
        nodeContainer.Render();
    }

    async UniTask GenerateCaves () {
        foreach(RectInt area in cavernAreas) {
            CellularAutomataCarver carver = new CellularAutomataCarver(area, nodeContainer, config.Cave);
            await carver.Generate();
        }
    }

    async UniTask GenerateTowerRoom() {
        TowerRoomGenerator towerRoomGenerator = new TowerRoomGenerator(config.Tower, nodeContainer);
        towerNodes = await towerRoomGenerator.Generate();
    }

    async UniTask FindAndConnectEnclosures() {
        List<CaveEnclosure> enclosures = await EnclosureFinder.Find(nodeContainer);
        EnclosureConnector connector = new EnclosureConnector(nodeContainer);
        MonoBehaviour.print($"Enclosures found: {enclosures.Count}");
        while (enclosures.Count > 1) {
            await EnclosureEdgeFinder.FindEdges(nodeContainer, enclosures);
            await connector.Connect(enclosures);
            enclosures = await EnclosureFinder.Find(nodeContainer);
        }
    }

    void CreateNavMesh() {
        Transform meshContainer = Prefabs.Get<Transform>();
        meshContainer.name = "NavMeshMesh";
        meshContainer.eulerAngles = new Vector3(90, 0, 0);
        Transform meshRotator = Prefabs.Get<Transform>();
        meshRotator.name = "NavMeshMeshRotator";
        meshRotator.parent = meshContainer;
        meshRotator.localEulerAngles = new Vector3(-90, 0, 0);
        nodeContainer.CreateMesh(meshRotator);
        //nodeContainer.CreateMesh(meshContainer);
        NavMeshUtil.GenerateNavMesh(meshContainer.gameObject);
    }

    private void Populate() {
        GameObject player = Instantiate(playerPrefab);
        player.transform.position = (Vector2) nodeContainer.MidPoint;
        FollowTarget cameraFollow = Camera.main.GetComponent<FollowTarget>();
        if (cameraFollow != null) {
            cameraFollow.Initialize(Configs.main.Camera, player.GetComponentInChildren<Movement>().transform, true);
        } else {
            Debug.Log("<color=red>Camera doesn't have FollowTarget component!</color>");
        }
        List<MapNode> nonTowerNodes = nodeContainer.Nodes.Where(node => !node.IsWall && !towerNodes.Contains(node)).ToList();
        foreach(EnemySpawn enemySpawn in config.Spawns) {
            for (int spawnCount = 0; spawnCount < enemySpawn.SpawnThisManyTimes; spawnCount += 1) {
                foreach(EnemyConfig enemyConfig in enemySpawn.Enemies) {
                    try {
                    MapNode randomNode = nonTowerNodes[UnityEngine.Random.Range(0, nonTowerNodes.Count)];
                    nonTowerNodes.Remove(randomNode);
                    Enemy enemy = Prefabs.Get<Enemy>();
                    enemy.Initialize(enemyConfig, randomNode);
                    enemy.name = enemyConfig.name;
                    enemy.WakeUp();
                    } catch (Exception e) {
                        MonoBehaviour.print(e);
                    }
                }
            }
        }
    }

}
