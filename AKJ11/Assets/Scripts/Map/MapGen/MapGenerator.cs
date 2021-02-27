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

        foreach(RectInt area in cavernAreas) {
            CellularAutomataCarver carver = new CellularAutomataCarver(area, nodeContainer, config.Cave);
            await carver.Generate();
        }
        nodeContainer.Render();
        TowerRoomGenerator towerRoomGenerator = new TowerRoomGenerator(config.Tower, nodeContainer);
        towerNodes = await towerRoomGenerator.Generate();

        List<CaveEnclosure> enclosures = await EnclosureFinder.Find(nodeContainer);
        EnclosureConnector connector = new EnclosureConnector(nodeContainer);
        MonoBehaviour.print($"Enclosures found: {enclosures.Count}");
        while (enclosures.Count > 1) {
            await EnclosureEdgeFinder.FindEdges(nodeContainer, enclosures);
            await connector.Connect(enclosures);
            enclosures = await EnclosureFinder.Find(nodeContainer);
        }
        Populate();
    }

    private void Populate() {
        GameObject player = Instantiate(playerPrefab);
        player.transform.position = (Vector2) nodeContainer.MidPoint;
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

    // Update is called once per frame
    void Update()
    {
        
    }
}
