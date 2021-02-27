using System.Collections.Generic;
using System.Linq;
public class CaveEnclosure {
    public List<MapNode> Edges {get; private set;} = new List<MapNode>();
    public List<MapNode> Nodes {get; private set;} = new List<MapNode>();

    public List<MapNodeConnection> Connections { get; private set; } = new List<MapNodeConnection>();

    public CaveEnclosure(MapNode node) {
        Nodes.Add(node);
    }

    public void AddNode(MapNode node) {
        Nodes.Add(node);
    }
    public void AddEdge(MapNode node) {
        Edges.Add(node);
    }

    public bool HasNode(MapNode node) {
        return Nodes.Contains(node);
    }

    public bool HasConnection(CaveEnclosure targetEnclosure) {
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