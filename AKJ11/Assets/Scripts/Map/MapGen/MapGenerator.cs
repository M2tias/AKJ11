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
    [SerializeField]
    private TorchConfig torchLeftConfig;
    [SerializeField]
    private TorchConfig torchRightConfig;
    [SerializeField]
    private TorchConfig torchTopConfig;
    [SerializeField]
    private TorchConfig torchBottomConfig;
    [SerializeField]
    private GameObject torchPrefab;

    private FadeOptions fadeToBlack = new FadeOptions(Color.black, 0.2f, true);
    private FadeOptions fadeToTransparent = new FadeOptions(Color.clear, 0.5f, true);

    private int currentLevel = 0;

    [SerializeField]
    private int DebugCurrentLevel;

    private FullscreenFade fader;

    public static MapGenerator main;

    private NextLevelTrigger nextLevelTrigger;

    GameObject player;

    private RandomNumberGenerator rng;

    void Awake()
    {
        main = this;
    }

    void Start()
    {
        fader = FullscreenFade.main;
        #if UNITY_EDITOR 
            currentLevel = DebugCurrentLevel;
        #endif
        Time.timeScale = 1f;
        rng = RandomNumberGenerator.GetInstance();
        SeedView.main.SetText(rng.Seed);
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
        MapConfig nextMapConfig = Configs.main.Campaign.Get(currentLevel);
        if (nextMapConfig != null) {
            config = nextMapConfig;
            currentLevel += 1;
        }
        else
        {
            TheEnd();
            return;
        }
        if (nodeContainer != null)
        {
            await nodeContainer.Kill();
        }
        nodeContainer = new NodeContainer(0, 0, config.Size, config.Size, config, config.CaveTileStyle);
        if (Configs.main.Debug.DelayGeneration)
        {
            nodeContainer.Render();
        }
        cavernAreas = MapAreaSplitter.GetSplitAreas(config);
        await Generate();
    }

    void CreateBackgroundSprites() {
        Vector2 tiledPosition = new Vector2(config.Size / 2 - 0.5f, config.Size / 2 - 0.5f);
        TiledBackground floor = Prefabs.Get<TiledBackground>();
        floor.Initialize(
            config.CaveTileStyle.GroundSprite,
            config.CaveTileStyle.GroundTint,
            nodeContainer.ViewContainer,
            -100,
            config.Size,
            tiledPosition
        );
        floor.name = "Floor";
        TiledBackground outsideWall = Prefabs.Get<TiledBackground>();
        outsideWall.Initialize(
            config.CaveTileStyle.GroundSprite,
            config.CaveTileStyle.ColorTint,
            nodeContainer.ViewContainer,
            -101,
            config.Size * 2,
            tiledPosition
        );
        outsideWall.name = "Outsidewall";
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
        CreateBackgroundSprites();
        await GenerateTowerRoom();
        await FindAndConnectEnclosures();
        await BlobGrid.Run(nodeContainer);
        CreateNavMesh();
        MapNode playerNode = Populate();
        PlaceItems(playerNode);
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
        meshContainer.SetParent(nodeContainer.ViewContainer);
        meshContainer.eulerAngles = new Vector3(90, 0, 0);
        Transform meshRotator = Prefabs.Get<Transform>();
        meshRotator.name = "NavMeshMeshRotator";
        meshRotator.parent = meshContainer;
        meshRotator.localEulerAngles = new Vector3(-90, 0, 0);
        nodeContainer.CreateMesh(meshRotator);
        NavMeshUtil.GenerateNavMesh(meshContainer.gameObject);
    }


    public Transform GetContainer() {
        return nodeContainer.ViewContainer;
    }

    private MapNode Populate()
    {
        if (player == null) {
            player = Instantiate(playerPrefab);
        }

        List<MapNode> nonTowerNodes = nodeContainer.Nodes.Where(node => !node.IsWall && !towerNodes.Contains(node)).ToList();
        MapNode playerNode = nodeContainer.GetNode(nodeContainer.MidPoint);

        if (Configs.main.Campaign.IsLastLevel(config)) {
            Debug.Log("LastLevel");
            TheEndView.main.Show();
        } else {
            nextLevelTrigger = Prefabs.Get<NextLevelTrigger>();
            nextLevelTrigger.transform.SetParent(nodeContainer.ViewContainer);
            MapNode keyNode;
            if (nonTowerNodes.Count > 0) {
                keyNode = nonTowerNodes.OrderByDescending(node => node.Distance(playerNode)).First();
                nextLevelTrigger.Initialize(nodeContainer.MidPoint);
            } else {
                keyNode = towerNodes.OrderByDescending(node => node.Distance(playerNode)).First();
                MapNode enterNode = towerNodes.OrderByDescending(node => node.Distance(keyNode)).First();
                nextLevelTrigger.Initialize(enterNode.Position);
            }
            NextLevelKey nextLevelKey = Prefabs.Get<NextLevelKey>();
            nextLevelKey.transform.SetParent(nodeContainer.ViewContainer);
            nextLevelKey.Initialize(keyNode.Position);
        }

        player.transform.position = (Vector2)playerNode.Position;
        Movement movement = player.GetComponentInChildren<Movement>();
        if (movement != null) {
            movement.transform.localPosition = Vector2.zero;
        }


        SpawnEnemies(playerNode, nonTowerNodes);
        SpawnTorches(playerNode, nonTowerNodes);

        FollowTarget cameraFollow = Camera.main.GetComponent<FollowTarget>();
        if (cameraFollow != null)
        {
            cameraFollow.Initialize(Configs.main.Camera, true);
        }
        else
        {
            Debug.Log("<color=red>Camera doesn't have FollowTarget component!</color>");
        }
        return playerNode;
    }

    private void PlaceItems(MapNode playerNode) {
        try{
        if (config.Items.Count > 0) {
            List<MapNode> nonTowerNodes = nodeContainer.Nodes.Where(node => !node.IsWall && !towerNodes.Contains(node)).ToList();
            List<MapNode> itemTowerNodes = nodeContainer.Nodes.Where(node => node.IsTower && node.Distance(playerNode) > 2).ToList();
            foreach(ItemSpawn spawn in config.Items) {
                for (int timesSpawned = 0; timesSpawned < spawn.SpawnThisManyTimes; timesSpawned += 1) {
                    MapNode spawnNode = null;
                    if (spawn.SpawnPosition == SpawnPosition.Tower) {
                        if (itemTowerNodes.Count < 1) {
                            continue;
                        }
                        spawnNode = itemTowerNodes[rng.Range(0, itemTowerNodes.Count)];
                        itemTowerNodes.Remove(spawnNode);
                    } else if (spawn.SpawnPosition == SpawnPosition.Cave) {
                        if (nonTowerNodes.Count < 1) {
                            continue;
                        }
                        spawnNode = nonTowerNodes[rng.Range(0, nonTowerNodes.Count)];
                        nonTowerNodes.Remove(spawnNode);
                    }
                    if (spawnNode != null) {
                        PickupableItem item = Prefabs.Get<PickupableItem>();
                        item.Initialize(spawn.Item, nodeContainer.ViewContainer, spawnNode.Position);
                    }
                }
            }
        }
        }
        catch(Exception e) {
            MonoBehaviour.print(e);
        }

    }

    private void SpawnEnemies(MapNode playerNode, List<MapNode> nonTowerNodes)
    {
        float minDistanceFromPlayer = 2f;

        List<MapNode> nonEdgeNodes = nonTowerNodes.Where(node => !node.IsEdge).ToList();
        List<MapNode> enemyTowerNodes = towerNodes.Where(node => !node.IsEdge && node.Distance(playerNode) > minDistanceFromPlayer).ToList();

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
                            randomNode = nonEdgeNodes[rng.Range(0, nonEdgeNodes.Count)];
                            nonEdgeNodes.Remove(randomNode);
                        }
                        else
                        {
                            randomNode = enemyTowerNodes[rng.Range(0, enemyTowerNodes.Count)];
                            enemyTowerNodes.Remove(randomNode);
                        }
                        Enemy enemy = Prefabs.Get<Enemy>();

                        enemy.transform.SetParent(nodeContainer.ViewContainer);
                        enemy.Initialize(enemyConfig, randomNode);
                        enemy.name = enemyConfig.name;
                        enemy.WakeUp();
                    }
                    catch (Exception e)
                    {
                        MonoBehaviour.print(e);
                    }
                }
            }
        }
    }

    private void SpawnTorches(MapNode playerNode, List<MapNode> nonTowerNodes)
    {
        IEnumerable<MapNode> edgeNodes = nonTowerNodes.Where(node => node.IsEdge);

        List <MapNode> leftEdgeNodes = edgeNodes.Where(node =>
            node.Neighbors.Any(n => n.IsWall && n.X < node.X && n.Y == node.Y) &&
            node.Neighbors.Where(n => n.IsWall && n.X < node.X).Count() > 1
        ).ToList();

        List<MapNode> rightEdgeNodes = edgeNodes.Where(node =>
            node.Neighbors.Any(n => n.IsWall && n.X > node.X && n.Y == node.Y) &&
            node.Neighbors.Where(n => n.IsWall && n.X > node.X).Count() > 1
        ).ToList();

        List<MapNode> topEdgeNodes = edgeNodes.Where(node =>
            node.Neighbors.Any(n => n.IsWall && n.X == node.X && n.Y > node.Y) &&
            node.Neighbors.Where(n => n.IsWall && n.Y > node.Y).Count() > 1
        ).ToList();

        List<MapNode> bottomEdgeNodes = edgeNodes.Where(node =>
            node.Neighbors.Any(n => n.IsWall && n.X == node.X && n.Y < node.Y) &&
            node.Neighbors.Where(n => n.IsWall && n.Y < node.Y).Count() > 1
        ).ToList();

        float spawnPitch = 4f;

        SpawnTorchesSide(leftEdgeNodes, spawnPitch, torchLeftConfig, "left");
        SpawnTorchesSide(rightEdgeNodes, spawnPitch, torchRightConfig, "right");
        SpawnTorchesSide(topEdgeNodes, spawnPitch, torchTopConfig, "top");
        SpawnTorchesSide(bottomEdgeNodes, spawnPitch, torchBottomConfig, "bottom");
    }

    private void SpawnTorchesSide(List<MapNode> edgeNodes, float spawnPitch, TorchConfig config, string direction)
    {
        for (int index = 0; index < Mathf.RoundToInt(edgeNodes.Count / spawnPitch); index += 1)
        {
            try
            {
                SpawnTorch(edgeNodes, spawnPitch, index, config, direction);
            }
            catch (Exception e)
            {
                MonoBehaviour.print(e);
            }
        }
    }

    private void SpawnTorch(List<MapNode> edgeNodes, float spawnPitch, int index, TorchConfig config, string direction)
    {
        MapNode randomNode = edgeNodes[Math.Min(Mathf.RoundToInt(index * spawnPitch), edgeNodes.Count - 1)];//UnityEngine.Random.Range(0, rightEdgeNodes.Count)];

        GameObject torchInstance2 = Instantiate(torchPrefab);
        torchInstance2.GetComponent<Torch>().Initialize(config);
        torchInstance2.transform.SetParent(nodeContainer.ViewContainer);
        Vector2 nodePos2 = (Vector2)randomNode.Position;
        torchInstance2.transform.position = new Vector2(nodePos2.x, nodePos2.y);
        torchInstance2.name = "torch_"+direction+"_X:" + randomNode.X + "|Y:" + randomNode.Y;
    }
}
