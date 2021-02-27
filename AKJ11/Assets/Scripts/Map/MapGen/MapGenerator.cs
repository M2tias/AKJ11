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
        TowerRoomGenerator towerRoomGenerator = new TowerRoomGenerator(config.Tower, nodeContainer);
        await towerRoomGenerator.Generate();
        foreach(RectInt area in cavernAreas) {
            CellularAutomataCarver carver = new CellularAutomataCarver(area, nodeContainer, config.Cave);
            await carver.Generate();
        }
        nodeContainer.Render();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
