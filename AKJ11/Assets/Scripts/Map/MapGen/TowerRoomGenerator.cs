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
        await DrawCircle(midPoint.X, midPoint.Y);
    }

    private async UniTask DrawCircle(int centerX, int centerY) {

        int width = nodeContainer.Width;
        int height = nodeContainer.Height;
        int size = config.Radius * 2;
        int radiusSq = (size * size) / 4;
        for (int y = 0; y < height; y += 1) {
            int yDiff = y - centerY;
            int treshold = radiusSq - (yDiff * yDiff);
            for (int x = 0; x < width; x+= 1) {
                await Configs.main.Debug.DelayIfCounterFinished(delayCounter);
                int xDiff = x - centerX;
                MapNode node = nodeContainer.GetLocalNode(x, y);
                if ((xDiff * xDiff) < treshold) {
                    node.IsWall = false;
                    if (Configs.main.Debug.DelayGeneration) {
                        node.Render();
                    }
                }
            }
        }
    }

}