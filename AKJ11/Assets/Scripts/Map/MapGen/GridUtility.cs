using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System;

public class GridUtility
{
    public static async UniTask<List<MapNode>> DrawCircle(NodeContainer nodeContainer, int radius, int centerX, int centerY) {

        DelayCounter delayCounter = new DelayCounter(70);
        int width = nodeContainer.Width;
        int height = nodeContainer.Height;
        int size = radius * 2;
        int radiusSq = (size * size) / 4;
        List<MapNode> nodes = new List<MapNode>();
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
                    nodes.Add(node);
                }
            }
        }
        return nodes;
    }

    public static async UniTask<List<MapNode>> DrawSquare(MapNode startNode, int radius, NodeContainer nodeContainer) {
        DelayCounter delayCounter = new DelayCounter(5);
        if (startNode == null) {
            return null;
        }
        List<MapNode> nodes = new List<MapNode>();
        nodes.Add(startNode);
        for (int x = -radius; x <= radius; x += 1) {
            for (int y = -radius; y <= radius; y+= 1) {
                await Configs.main.Debug.DelayIfCounterFinished(delayCounter);
                int xPos = startNode.WorldX + x;
                int yPos = startNode.WorldY + y;
                MapNode node = nodeContainer.GetNode(xPos, yPos);
                if (node != null) {
                    nodes.Add(node);
                    if (node.IsWall) {
                        node.MapGen.Carve();
                    }
                }
            }
        }
        return nodes;
    }

    public static List<MapNode> GetLine(MapNode start, MapNode target, NodeContainer nodeContainer)
    {
        List<MapNode> line = new List<MapNode>();
        int lineX = start.WorldX;
        int lineY = start.WorldY;
        line.Add(nodeContainer.GetNode(lineX, lineY));

        int deltaX = target.WorldX - lineX;
        int deltaY = target.WorldY - lineY;

        bool inverted = false;
        int step = Math.Sign(deltaX);
        int gradientStep = Math.Sign(deltaY);

        int longest = Math.Abs(deltaX);
        int shortest = Math.Abs(deltaY);

        if (longest < shortest)
        {
            inverted = true;
            longest = Math.Abs(deltaY);
            shortest = Math.Abs(deltaX);
            step = Math.Sign(deltaY);
            gradientStep = Math.Sign(deltaX);
        }

        int gradientAccumulation = longest / 2;
        for (int index = 1; index < longest; index += 1)
        {

            if (inverted)
            {
                lineY += step;
            }
            else
            {
                lineX += step;
            }

            MapNode node = nodeContainer.GetNode(lineX, lineY);
            if (node != null)
            {
                line.Add(node);
            }

            gradientAccumulation += shortest;
            if (gradientAccumulation >= longest)
            {
                if (inverted)
                {
                    lineX += gradientStep;
                }
                else
                {
                    lineY += gradientStep;
                }
                gradientAccumulation -= longest;
            }
        }

        return line;
    }
}
