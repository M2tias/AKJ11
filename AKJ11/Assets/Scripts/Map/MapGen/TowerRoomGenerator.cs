using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using System;

public class TowerRoomGenerator
{

    public static async UniTask<CaveEnclosure> Generate(int radius, NodeContainer nodeContainer, MapConfig config, MapArea area) {
        return await Generate(radius, nodeContainer, CalculateCircularRoomPosition(nodeContainer, config, area), false);
    }

    public static async UniTask<CaveEnclosure> GenerateTower(int radius, NodeContainer nodeContainer, Vector2Int startPoint) {
        CaveEnclosure enclosure = await Generate(radius, nodeContainer, startPoint, true);
        return enclosure;
    }

    private static async UniTask<CaveEnclosure> Generate(int radius, NodeContainer nodeContainer, Vector2Int startPoint, bool isTower) {
        if (Configs.main.Debug.DelayGeneration) {
            nodeContainer.Render();
        }
        MapNode midPoint = nodeContainer.GetNode(startPoint);
        List<MapNode> nodes = await GridUtility.DrawCircle(nodeContainer, radius, midPoint.X, midPoint.Y);
        return new CaveEnclosure(nodes, isTower);
    }

    private static Vector2Int CalculateCircularRoomPosition(NodeContainer nodeContainer, MapConfig config, MapArea area)
    {
        int offset = config.Padding + config.CircularAreaRadius;
        if (area.Type == MapAreaType.BotLeft)
        {
            offset = area.Rect.xMin + offset;
        }
        else if (area.Type == MapAreaType.BotRight) {
            offset = area.Rect.yMin + offset;
        }
        else if (area.Type == MapAreaType.TopLeft)
        {
            offset = area.Rect.yMax - offset;
        }
        else if (area.Type == MapAreaType.TopRight) {
            offset = area.Rect.xMax - offset;
        }
        if (area.Type == MapAreaType.TopLeft || area.Type == MapAreaType.BotRight)
        {
            return new Vector2Int(nodeContainer.MidPoint.x, offset);
        }
        else
        {
            return new Vector2Int(offset, nodeContainer.MidPoint.y);
        }
    }

}