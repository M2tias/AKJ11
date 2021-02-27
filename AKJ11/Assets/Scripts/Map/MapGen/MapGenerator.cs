using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class MapGenerator : MonoBehaviour
{
    List<RectInt> cavernAreas;
    NodeContainer nodeContainer;
    MapConfig config;
    
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
        await towerRoomGenerator.Generate();

        List<CaveEnclosure> enclosures = await EnclosureFinder.Find(nodeContainer);
        EnclosureConnector connector = new EnclosureConnector(nodeContainer);
        MonoBehaviour.print($"Enclosures found: {enclosures.Count}");
        while (enclosures.Count > 1) {
            await EnclosureEdgeFinder.FindEdges(nodeContainer, enclosures);
            await connector.Connect(enclosures);
            enclosures = await EnclosureFinder.Find(nodeContainer);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
