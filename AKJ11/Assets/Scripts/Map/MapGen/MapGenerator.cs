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
    NodeContainer baseLayer;
    MapConfig config;
    List<MapNode> towerNodes;

    [SerializeField]
    private GameObject playerPrefab;

    private FadeOptions fadeToBlack = new FadeOptions(Color.black, 0.2f, true);
    private FadeOptions fadeToTransparent = new FadeOptions(Color.clear, 0.5f, true);

    private int currentLevel = 0;

    private FullscreenFade fader;

    public static MapGenerator main;

    public int EnemyCount = 0;

    private NextLevelTrigger nextLevelTrigger;

    GameObject player;

    void Awake()
    {
        main = this;
    }

    void Start()
    {
        fader = FullscreenFade.main;
        fader.Initialize();
        NextLevel();
    }

    void TheEnd()
    {
        Debug.Log("Gratz!");
    }

    public async UniTask LevelEnd() {
        if (SoundManager.main != null) {
            SoundManager.main.PlaySound(GameSoundType.DoorOpen);
        }
        await fader.Fade(fadeToBlack);
        await NextLevel();
    }

    public void FoundKey() {
        if (nextLevelTrigger != null) {
            nextLevelTrigger.Enable();
        }
    }
    public async UniTask NextLevel()
    {
        await NewMap();
        await fader.Fade(fadeToTransparent);
    }

    async UniTask NewMap()
    {
        if (Configs.main.Maps.Count > currentLevel)
        {
            config = Configs.main.Maps[currentLevel];
            currentLevel += 1;
        }
        else
        {
            TheEnd();
            return;
        }
        if (nodeContainer != null)
        {
            nodeContainer.Kill();
        }
        nodeContainer = new NodeContainer(0, 0, config.Size, config.Size, config, config.CaveTileStyle);
        baseLayer = new NodeContainer(0, 0, config.Size, config.Size, config, config.DefaultTileStyle, false);
        baseLayer.Render();
        baseLayer.viewContainer.SetParent(nodeContainer.viewContainer);
        if (Configs.main.Debug.DelayGeneration)
        {
            nodeContainer.Render();
        }
        cavernAreas = MapAreaSplitter.GetSplitAreas(config);
        await Generate();
    }

    async UniTask Generate()
    {
        List<CaveEnclosure> caves;
        await GenerateCaves();
        caves = await EnclosureFinder.Find(nodeContainer);

        int attempts = 20;
        while (caves.Count < config.NumberOfAreas && attempts > 0)
        {
            nodeContainer.Kill();
            nodeContainer = new NodeContainer(0, 0, config.Size, config.Size, config, config.CaveTileStyle);
            baseLayer.viewContainer.SetParent(nodeContainer.viewContainer);
            await GenerateCaves();
            caves = await EnclosureFinder.Find(nodeContainer);
            attempts -= 1;
            if (caves.Count < config.NumberOfAreas) {
                Debug.Log($"{caves.Count} < {config.NumberOfAreas}, trying {attempts} more times...");
            }
        }
        if (attempts < 1)
        {
            Debug.Log("Config sucks! Just couldn't create enough caves.");
        }
        if (Configs.main.Debug.DelayGeneration)
        {
            nodeContainer.Render();
        }
        await GenerateTowerRoom();
        await FindAndConnectEnclosures();
        await BlobGrid.Run(nodeContainer);
        CreateNavMesh();
        Populate();
        nodeContainer.Render();
    }

    async UniTask GenerateCaves()
    {
        foreach (RectInt area in cavernAreas)
        {
            CellularAutomataCarver carver = new CellularAutomataCarver(area, nodeContainer);
            await carver.Generate();
        }
    }

    async UniTask GenerateTowerRoom()
    {
        TowerRoomGenerator towerRoomGenerator = new TowerRoomGenerator(config, nodeContainer);
        towerNodes = await towerRoomGenerator.Generate();
        foreach (MapNode node in towerNodes)
        {
            node.IsTower = true;
        }
    }

    async UniTask FindAndConnectEnclosures()
    {
        List<CaveEnclosure> enclosures = await EnclosureFinder.Find(nodeContainer);
        EnclosureConnector connector = new EnclosureConnector(nodeContainer, config);
        while (enclosures.Count > 1)
        {
            await EnclosureEdgeFinder.FindEdges(nodeContainer, enclosures, config);
            await connector.Connect(enclosures);
            enclosures = await EnclosureFinder.Find(nodeContainer);
        }
        await EnclosureEdgeFinder.FindEdges(nodeContainer, enclosures, config);
    }

    void CreateNavMesh()
    {
        Transform meshContainer = Prefabs.Get<Transform>();
        meshContainer.name = "NavMeshMesh";
        meshContainer.SetParent(nodeContainer.viewContainer);
        meshContainer.eulerAngles = new Vector3(90, 0, 0);
        Transform meshRotator = Prefabs.Get<Transform>();
        meshRotator.name = "NavMeshMeshRotator";
        meshRotator.parent = meshContainer;
        meshRotator.localEulerAngles = new Vector3(-90, 0, 0);
        nodeContainer.CreateMesh(meshRotator);
        NavMeshUtil.GenerateNavMesh(meshContainer.gameObject);
    }


    public Transform GetContainer() {
        return nodeContainer.viewContainer;
    }

    private void Populate()
    {
        if (player == null) {
            player = Instantiate(playerPrefab);
        }
        player.transform.position = (Vector2)nodeContainer.MidPoint;
        nextLevelTrigger = Prefabs.Get<NextLevelTrigger>();
        nextLevelTrigger.transform.SetParent(nodeContainer.viewContainer);
        nextLevelTrigger.Initialize(nodeContainer.MidPoint);

        List<MapNode> nonTowerNodes = nodeContainer.Nodes.Where(node => !node.IsWall && !towerNodes.Contains(node)).ToList();
        NextLevelKey nextLevelKey = Prefabs.Get<NextLevelKey>();
        nextLevelKey.transform.SetParent(nodeContainer.viewContainer);
        MapNode playerNode = nodeContainer.GetNode(nodeContainer.MidPoint);
        MapNode keyNode = nonTowerNodes.OrderByDescending(node => node.Distance(playerNode)).First();
        nextLevelKey.Initialize(keyNode.Position);

        SpawnEnemies(playerNode, nonTowerNodes);

        FollowTarget cameraFollow = Camera.main.GetComponent<FollowTarget>();
        if (cameraFollow != null)
        {
            cameraFollow.Initialize(Configs.main.Camera, player.GetComponentInChildren<Movement>().transform, true);
        }
        else
        {
            Debug.Log("<color=red>Camera doesn't have FollowTarget component!</color>");
        }
        

    }

    private void SpawnEnemies(MapNode playerNode, List<MapNode> nonTowerNodes) {
        float minDistanceFromPlayer = 2f;

        List<MapNode> enemyTowerNodes = towerNodes.Where(node => node.Distance(playerNode) > minDistanceFromPlayer).ToList();
        
        foreach (EnemySpawn enemySpawn in config.Spawns)
        {
            for (int spawnCount = 0; spawnCount < enemySpawn.SpawnThisManyTimes; spawnCount += 1)
            {
                foreach (EnemyConfig enemyConfig in enemySpawn.Enemies)
                {
                    try
                    {
                        MapNode randomNode;
                        if (enemySpawn.SpawnPosition == SpawnPosition.Cave)
                        {
                            randomNode = nonTowerNodes[UnityEngine.Random.Range(0, nonTowerNodes.Count)];
                            nonTowerNodes.Remove(randomNode);
                        }
                        else
                        {
                            randomNode = enemyTowerNodes[UnityEngine.Random.Range(0, enemyTowerNodes.Count)];
                            enemyTowerNodes.Remove(randomNode);
                        }
                        Enemy enemy = Prefabs.Get<Enemy>();
                        enemy.transform.SetParent(nodeContainer.viewContainer);
                        enemy.Initialize(enemyConfig, randomNode);
                        enemy.name = enemyConfig.name;
                        enemy.WakeUp();
                        EnemyCount += 1;
                    }
                    catch (Exception e)
                    {
                        MonoBehaviour.print(e);
                    }
                }
            }
        }
    }

}
