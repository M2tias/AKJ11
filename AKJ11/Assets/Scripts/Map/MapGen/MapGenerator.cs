using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using Cysharp.Threading.Tasks;

public class MapGenerator : MonoBehaviour
{
    List<MapArea> cavernAreas;
    NodeContainer nodeContainer;
    MapConfig config;

    [SerializeField]
    private Transform worldContainer;

    private FadeOptions fadeToBlack = new FadeOptions(Color.black, 0.2f);
    private FadeOptions fadeToTransparent = new FadeOptions(Color.clear, 0.5f);

    private int currentLevel = 0;

    public int CurrentLevel { get { return currentLevel; } }

    [SerializeField]
    private int DebugCurrentLevel;
    [SerializeField]
    private string DebugSeed = "";
    [SerializeField]
    private int DebugIntSeed;

    [SerializeField]
    private bool NextLevelButton = false;

    private FullscreenFade fader;

    public static MapGenerator main;

    private RandomNumberGenerator rng;

    private MapGenData data;
    List<MapNode> sealNodes;
    List<MapNode> towerSealNodes;

    void Awake()
    {
        main = this;
    }

    void Start()
    {
        fader = FullscreenFade.main;
#if UNITY_EDITOR
        currentLevel = DebugCurrentLevel;
        if (DebugSeed.Trim() != "")
        {
            RandomNumberGenerator.SetInstance(new RandomNumberGenerator(DebugSeed));
        }
        else if (DebugIntSeed != default(int))
        {
            RandomNumberGenerator.SetInstance(new RandomNumberGenerator(DebugIntSeed));
        }
        if (NextLevelButton)
        {
            Prefabs.Get<NextLevelButton>();
        }
#endif
        rng = RandomNumberGenerator.GetInstance();
        GameStateManager.main.StartTime();
        SeedView.main.SetText(rng.Seed);
        NextLevel();

    }

    void TheEnd()
    {
        Debug.Log("Gratz!");
    }

    public async UniTask LevelEnd()
    {
        if (SoundManager.main != null)
        {
            SoundManager.main.PlaySound(GameSoundType.DoorOpen);
        }
        GameStateManager.main.LevelEnded();
        await fader.Fade(fadeToBlack);
        await NextLevel();
    }

    public Transform GetContainer()
    {
        return nodeContainer.ViewContainer;
    }

    public NodeContainer GetNodeContainer()
    {
        return nodeContainer;
    }

    public async UniTask NextLevel()
    {
        try
        {
            await NewMap();
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
        GameStateManager.main.LevelStarted(config, currentLevel);
        Camera.main.GetComponent<FollowTarget>().SetPositionToTarget();
        if (Shooting.main != null)
        {
            Shooting.main.KillActiveSpells();
        }
        await fader.Fade(fadeToTransparent);
        GameStateManager.main.StartTime();
    }

    public async void SealAllRooms(bool destroyable = false)
    {
        await RoomSealer.SealAllRooms(data, sealNodes, destroyable);
    }

    public async void UnsealAllRooms()
    {
        await RoomSealer.UnsealAllRooms(sealNodes);
        await RoomSealer.UnsealAllRooms(towerSealNodes);
    }

    public async UniTask RunBlobGrid()
    {
        await BlobGrid.Run(nodeContainer);
        nodeContainer.Render();
    }

    async UniTask NewMap()
    {
        MapConfig nextMapConfig = Configs.main.Campaign.Get(currentLevel);
        if (nextMapConfig.CaveTileStyle == null)
        {
            Debug.Log("<color=red>You need to select tileStyle for your map config!");
        }
        if (nextMapConfig != null)
        {
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
        nodeContainer.ViewContainer.transform.SetParent(worldContainer, false);
        if (Configs.main.Debug.DelayGeneration)
        {
            nodeContainer.Render();
        }
        cavernAreas = MapAreaSplitter.GetSplitAreas(config);
        await Generate();
    }

    async UniTask Generate()
    {
        List<CaveEnclosure> rooms = await GenerateAndFindEnclosures();
        await EnclosureEdgeFinder.FindEdges(nodeContainer, rooms);
        if (Configs.main.Debug.DelayGeneration)
        {
            nodeContainer.Render();
        }
        BackgroundCreator.Create(nodeContainer, config);
        CaveEnclosure tower = await TowerRoomGenerator.GenerateTower(config.TowerRadius, nodeContainer, nodeContainer.MidPoint);
        await EnclosureEdgeFinder.FindEdges(nodeContainer, tower);
        CaveEnclosure roomsAndTower = await FindAndConnectEnclosures();
        await BlobGrid.Run(nodeContainer);
        CreateNavMesh();
        data = new MapGenData
        {
            NodeContainer = nodeContainer,
            Config = config,
            Tower = tower,
            Rooms = rooms,
            RoomsAndTower = roomsAndTower
        };
        MapPopulator.Populate(data);
        TorchSpawner.Spawn(data);
        PlaceItems(data);
        nodeContainer.Render();
        sealNodes = RoomSealer.FindNodesToSeal(data);
        if (config.SealTower)
        {
            towerSealNodes = await RoomSealer.SealTower(data, nodeContainer, true);
        }
        BackgroundCreator.CreateFloor(data, config, nodeContainer);
    }

    async UniTask<List<CaveEnclosure>> GenerateAndFindEnclosures()
    {
        List<CaveEnclosure> areas;
        await GenerateEnclosures();
        areas = await EnclosureFinder.Find(nodeContainer);

        int attempts = 20;
        while (areas.Count < config.NumberOfAreas && attempts > 0)
        {
            nodeContainer.KillSync();
            nodeContainer = new NodeContainer(0, 0, config.Size, config.Size, config, config.CaveTileStyle);
            await GenerateEnclosures();
            areas = await EnclosureFinder.Find(nodeContainer);
            attempts -= 1;
            if (areas.Count < config.NumberOfAreas)
            {
                Debug.Log($"{areas.Count} < {config.NumberOfAreas}, trying {attempts} more times...");
            }
        }
        if (attempts < 1)
        {
            Debug.Log($"Config sucks! Just couldn't create enough areas of type: {config.AreaType}.");
        }
        return areas;
    }

    async UniTask GenerateEnclosures()
    {
        if (config.AreaType == SpawnPosition.Cave)
        {
            await GenerateCaves();
        }
        else
        {
            await GenerateCircularRooms();
        }
    }

    async UniTask GenerateCaves()
    {
        foreach (MapArea area in cavernAreas)
        {
            CellularAutomataCarver carver = new CellularAutomataCarver(area.Rect, nodeContainer);
            await carver.Generate();
        }
    }

    async UniTask GenerateCircularRooms()
    {
        foreach (MapArea area in cavernAreas)
        {
            await TowerRoomGenerator.Generate(
                config.CircularAreaRadius,
                nodeContainer,
                config,
                area
            );
        }
    }

    async UniTask<CaveEnclosure> FindAndConnectEnclosures()
    {
        List<CaveEnclosure> enclosures = await EnclosureFinder.Find(nodeContainer);
        EnclosureConnector connector = new EnclosureConnector(nodeContainer, config);
        while (enclosures.Count > 1)
        {
            await EnclosureEdgeFinder.FindEdges(nodeContainer, enclosures);
            await connector.Connect(enclosures);
            enclosures = await EnclosureFinder.Find(nodeContainer);
        }
        await EnclosureEdgeFinder.FindEdges(nodeContainer, enclosures);
        if (enclosures.Count == 1)
        {
            return enclosures[0];
        }
        return null;
    }

    private void CreateNavMesh()
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

    private void PlaceItems(MapGenData data)
    {
        List<MapNode> towerNodes = new List<MapNode>(data.Tower.Nodes);
        List<MapNode> nonTowerNodes = new List<MapNode>(data.NonTowerNodes);
        try
        {
            if (config.Items.Count > 0)
            {
                foreach (ItemSpawn spawn in config.Items)
                {
                    for (int timesSpawned = 0; timesSpawned < spawn.SpawnThisManyTimes; timesSpawned += 1)
                    {
                        MapNode spawnNode = null;
                        if (spawn.SpawnPosition == SpawnPosition.Tower)
                        {
                            if (towerNodes.Count < 1)
                            {
                                continue;
                            }
                            List<MapNode> possibleNodes = towerNodes.Where(node => node.Distance(data.Player.Node) > spawn.MinPlayerDistance).ToList();
                            spawnNode = possibleNodes[rng.Range(0, possibleNodes.Count)];
                            towerNodes.Remove(spawnNode);
                        }
                        else if (spawn.SpawnPosition == SpawnPosition.Cave)
                        {
                            if (nonTowerNodes.Count < 1)
                            {
                                continue;
                            }
                            spawnNode = nonTowerNodes[rng.Range(0, nonTowerNodes.Count)];
                            nonTowerNodes.Remove(spawnNode);
                        }
                        if (spawnNode != null)
                        {
                            PickupableItem item = Prefabs.Get<PickupableItem>();
                            item.Initialize(spawn.Item, nodeContainer.ViewContainer, spawnNode.Position);
                        }
                    }
                }
            }
        }
        catch (Exception e)
        {
            MonoBehaviour.print(e);
        }
    }

}
