using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using System;

public class TowerRoomGenerator
{

    public static async UniTask<List<MapNode>> Generate(int radius, NodeContainer nodeContainer, Vector2Int startPoint) {
        if (Configs.main.Debug.DelayGeneration) {
            nodeContainer.Render();
        }
        MapNode midPoint = nodeContainer.GetNode(startPoint);
        List<MapNode> nodes = await GridUtility.DrawCircle(nodeContainer, radius, midPoint.X, midPoint.Y);
        return nodes;
    }

}