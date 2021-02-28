using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using System;

public class TowerRoomGenerator
{

    private NodeContainer nodeContainer;
    private MapConfig config;
    private DelayCounter delayCounter = new DelayCounter(70);
    private int buffer = 2;
    public TowerRoomGenerator(MapConfig config, NodeContainer nodeContainer) {
        this.config = config;
        int size = config.TowerRadius * 2 + buffer;
        this.nodeContainer = nodeContainer;
        if (Configs.main.Debug.DelayGeneration) {
            nodeContainer.Render();
        }
    }

    public async UniTask<List<MapNode>> Generate() {
        MapNode midPoint = nodeContainer.GetNode(nodeContainer.MidPoint);
        List<MapNode> nodes = await GridUtility.DrawCircle(nodeContainer, config.TowerRadius, midPoint.X, midPoint.Y);
        return nodes;
    }

}