using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using System;

public class TowerRoomGenerator
{

    private NodeContainer nodeContainer;
    private TowerRoomConfig config;
    private DelayCounter delayCounter = new DelayCounter(70);
    public TowerRoomGenerator(TowerRoomConfig config, NodeContainer nodeContainer) {
        this.config = config;
        int size = config.Radius * 2 + config.Buffer;
        this.nodeContainer = nodeContainer;
        if (Configs.main.Debug.DelayGeneration) {
            nodeContainer.Render();
        }
    }

    public async UniTask Generate() {
        MapNode midPoint = nodeContainer.GetNode(nodeContainer.MidPoint);
        await GridUtility.DrawCircle(nodeContainer, config.Radius, midPoint.X, midPoint.Y);
    }

}