using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class CaveEnclosure
{
    public List<MapNode> Edges { get; private set; } = new List<MapNode>();
    public List<MapNode> Nodes { get; private set; } = new List<MapNode>();

    public List<MapNodeConnection> Connections { get; private set; } = new List<MapNodeConnection>();

    private bool midPointCalculated;
    private Vector2 midPoint;
    private Vector2 MidPoint {
        get {
            if (!midPointCalculated) {
                midPoint = new Vector2(
                    Edges.Select(node => (float)node.X).Average(),
                    Edges.Select(node => (float)node.Y).Average()
                );
                midPointCalculated = true;
            }
            return midPoint;
        }
    }

    public float DistanceFromMidPoint(MapNode node) {
        return Vector2.Distance(node.Position, MidPoint);
    }

    public bool IsTower {get; private set;} = false;
    public CaveEnclosure(MapNode node, bool isTower = false)
    {
        IsTower = isTower;
        AddNode(node);
    }

    public CaveEnclosure(List<MapNode> nodes, bool isTower)
    {
        Nodes = new List<MapNode>(nodes);
        IsTower = isTower;
    }

    public void AddNode(MapNode node)
    {
        Nodes.Add(node);
    }
    public void AddEdge(MapNode node)
    {
        Edges.Add(node);
    }

    public bool HasNode(MapNode node)
    {
        return Nodes.Contains(node);
    }

    public bool HasConnection(CaveEnclosure targetEnclosure)
    {
        return Connections.Any(connection => connection.CaveEnclosureB == targetEnclosure);
    }

    public void AddConnection(MapNodeConnection connection)
    {
        if (!HasConnection(connection.CaveEnclosureB))
        {
            Connections.Add(connection);
        }
    }


}