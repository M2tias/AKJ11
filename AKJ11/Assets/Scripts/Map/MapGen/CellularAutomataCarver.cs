using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using Cysharp.Threading.Tasks;

public class CellularAutomataCarver
{
    private NodeContainer nodeContainer;
    private List<MapNode> nodes = new List<MapNode>();
    private DelayCounter counter = new DelayCounter(25);
    private CaveConfig config;
    private RectInt area;
    

    private Dictionary<MapNode, List<MapNode>> memoizedNeighbors = new Dictionary<MapNode, List<MapNode>>();
    public CellularAutomataCarver(RectInt area, NodeContainer nodeContainer, CaveConfig config)
    {
        this.nodeContainer = nodeContainer;
        this.config = config;
        this.area = area;
    }

    public async UniTask Generate()
    {
        await RandomFillMap();
        int cavernRunCounter = 0;
        while (cavernRunCounter < config.CavernRuns) {
            await MakeCaverns();
            cavernRunCounter += 1;
        }
    }

    public static bool RandomPercent(int percent)
    {
        return percent >= UnityEngine.Random.Range(1, 101);
    }

    private async UniTask RandomFillMap()
    {
        foreach (Vector2Int position in area.allPositionsWithin)
        {
            await Configs.main.Debug.DelayIfCounterFinished(counter);
            MapNode node = nodeContainer.GetNode(position);
            if (node == null)
            {
                continue;
            }
            if (!RandomPercent(config.PercentWalls))
            {
                node.MapGen.Carve();
            }
            if (Configs.main.Debug.DelayGeneration)
            {
                node.Render();
            }
        }
    }

    private List<MapNode> GetNeighbors(MapNode node)
    {
        if (!memoizedNeighbors.ContainsKey(node))
        {
            memoizedNeighbors[node] = nodeContainer.FindAllNeighbors(node);
        }
        return memoizedNeighbors[node];
    }

    private int GetNeighborWalls(MapNode node)
    {
        return GetNeighbors(node).Where(n => n != null && n.IsWall).Count();
    }

    private async UniTask MakeCaverns()
    {
        try {

        
        foreach (Vector2Int position in area.allPositionsWithin)
        {
            {
                await Configs.main.Debug.DelayIfCounterFinished(counter);
                MapNode node = nodeContainer.GetNode(position);
                MapGenerationData mapGenData = node.MapGen;
                if (node == null || (node.IsWall && mapGenData.AllNeighborsAreWalls))
                {
                    continue;
                }
                if (node != null && node.IsInside(nodeContainer.Rect))
                {
                    int numberOfWalls = GetNeighborWalls(node);
                    if (node.IsWall)
                    {
                        if (numberOfWalls < 4)
                        {
                            mapGenData.Carve();
                            if (Configs.main.Debug.DelayGeneration) {
                                node.Render();
                            }
                        }
                    }
                    else if (numberOfWalls > 4)
                    {
                        mapGenData.Uncarve();
                        if (Configs.main.Debug.DelayGeneration) {
                            node.Render();
                        }
                    }
                    if (numberOfWalls == 8)
                    {
                        mapGenData.AllNeighborsAreWalls = true;
                    }
                    else
                    {
                        mapGenData.AllNeighborsAreWalls = false;
                    }
                }
            }
        }
        } catch (Exception e) {
            UnityEngine.MonoBehaviour.print(e);
        }
    }
}
