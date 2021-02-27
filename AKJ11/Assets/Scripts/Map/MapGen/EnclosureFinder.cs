using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Linq;

public class EnclosureFinder
{


    private static Vector2Int northCoord = new Vector2Int(0, 1);
    private static Vector2Int eastCoord = new Vector2Int(1, 0);
    private static Vector2Int southCoord = new Vector2Int(0, -1);
    private static Vector2Int westCoord = new Vector2Int(-1, 0);

    public static async UniTask<List<CaveEnclosure>> Find(NodeContainer nodeContainer) {
        List<CaveEnclosure> enclosures = new List<CaveEnclosure>();
        List<MapNode> nodes = nodeContainer.Nodes.Where(node => !node.IsWall).ToList();

        DelayCounter counter = new DelayCounter(25);
        while (nodes.Count > 0) {
            MapNode node = nodes.First();
            nodes.RemoveAt(0);
            node.SetColor(Color.magenta);

            CaveEnclosure enclosure = new CaveEnclosure(node);
            enclosures.Add(enclosure);

            ColorNode(node, enclosure, nodes);

            Queue<MapNode> nodeQueue = new Queue<MapNode>();
            nodeQueue.Enqueue(node);

            await FloodFill(nodeQueue, enclosure, counter, nodes, nodeContainer);
        }
        return enclosures;
    }
    private static async UniTask FloodFill(Queue<MapNode> nodeQueue, CaveEnclosure enclosure, DelayCounter counter, List<MapNode> nodes, NodeContainer nodeContainer)
    {
        while (nodeQueue.Count > 0)
        {
            MapNode innerNode = nodeQueue.Dequeue();
            MapNode west = GetNeighbor(nodeContainer, innerNode, westCoord);
            if (ColorNode(west, enclosure, nodes))
            {
                nodeQueue.Enqueue(west);
            }
            MapNode east = GetNeighbor(nodeContainer, innerNode, eastCoord);
            if (ColorNode(east, enclosure, nodes))
            {
                nodeQueue.Enqueue(east);
            }
            MapNode north = GetNeighbor(nodeContainer, innerNode, northCoord);
            if (ColorNode(north, enclosure, nodes))
            {
                nodeQueue.Enqueue(north);
            }
            MapNode south = GetNeighbor(nodeContainer, innerNode, southCoord);
            if (ColorNode(south, enclosure, nodes))
            {
                nodeQueue.Enqueue(south);
            }
            await Configs.main.Debug.DelayIfCounterFinished(counter);
        }
    }

    private static MapNode GetNeighbor(NodeContainer nodeContainer, MapNode node, Vector2Int coord)
    {
        return nodeContainer.GetNode(node.Position + coord);
    }


    private static bool ColorNode(MapNode node, CaveEnclosure enclosure, List<MapNode> nodes) {
        if (node == null)
        {
            return false;
        }
        if (!node.IsWall && !enclosure.HasNode(node))
        {
            enclosure.AddNode(node);

            node.SetColor(Color.green);
            if (nodes.Contains(node))
            {
                nodes.Remove(node);
            }
            return true;
        }
        return false;
    }
}
