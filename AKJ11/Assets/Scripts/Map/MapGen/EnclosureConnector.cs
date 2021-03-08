using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Cysharp.Threading.Tasks;

public class EnclosureConnector
{

    private NodeContainer nodeContainer;
    private List<CaveEnclosure> enclosures;
    private Color connectionColor = new Color(0.7f, 0, 0, 0.2f);
    private Color lineColor = new Color(0, 0.25f, 0.5f, 0.2f);
    private DelayCounter counter;
    private MapConfig config;

    public EnclosureConnector(NodeContainer nodeContainer, MapConfig config) {
        this.nodeContainer = nodeContainer; 
        this.config = config;
    }

    public async UniTask Connect(List<CaveEnclosure> enclosures)
    {
        counter = new DelayCounter(1);
        MapNodeConnection chosenConnection = FindConnectionsForRooms(enclosures);
        if (chosenConnection != null) {
            await CarvePassage(chosenConnection);
        } else {
            UnityEngine.MonoBehaviour.print("No connection found!");
        }
    }

    private MapNodeConnection FindConnectionsForRooms(List<CaveEnclosure> enclosures)
    {
        List<MapNodeConnection> connections = new List<MapNodeConnection>();
        foreach(CaveEnclosure caveEnclosureA in enclosures) {
            MapNodeConnection caveEnclosureConnection = new MapNodeConnection(caveEnclosureA, config.ConnectionStrategy);
            caveEnclosureA.AddConnection(caveEnclosureConnection);
            foreach(CaveEnclosure caveEnclosureB in enclosures) {
                if (caveEnclosureA == caveEnclosureB) {
                    continue;
                }
                FindConnections(caveEnclosureA, caveEnclosureB, caveEnclosureConnection);
            }
            if (caveEnclosureConnection.NodeA != null && caveEnclosureConnection.NodeB != null) {
                connections.Add(caveEnclosureConnection);
            }
        }
        return connections.OrderBy(connection => connection.distance).FirstOrDefault();
    }

    private void FindConnections(CaveEnclosure caveEnclosureA, CaveEnclosure caveEnclosureB, MapNodeConnection CaveEnclosureConnection) {
        if (caveEnclosureA.HasConnection(caveEnclosureB) || caveEnclosureB.HasConnection(caveEnclosureA)) {
            return;
        }
        foreach(MapNode nodeA in caveEnclosureA.Edges) {
            foreach(MapNode nodeB in caveEnclosureB.Edges) {
                CaveEnclosureConnection.SetAsNewConnectionIfSmaller(nodeA, nodeB, caveEnclosureB);
            }
        }
    }

    private async UniTask CarvePassage(MapNodeConnection connection) {
        if (connection != null) {
            List<MapNode> line = GridUtility.GetLine(connection.NodeA, connection.NodeB, nodeContainer);
            foreach(MapNode node in line) {
                List<MapNode> nodes = await GridUtility.DrawSquare(node, config.PassageRadius, nodeContainer);
                await Configs.main.Debug.DelayIfCounterFinished(counter);
            }
        }
    }

}

public class MapNodeConnection {
    private ConnectionStrategy strategy;
    public MapNodeConnection(CaveEnclosure caveEnclosureA, ConnectionStrategy connectionStrategy) {
        strategy = connectionStrategy;
        CaveEnclosureA = caveEnclosureA;
    }


    public void SetAsNewConnectionIfSmaller(MapNode nodeA, MapNode nodeB, CaveEnclosure caveEnclosureB) {
        if (nodeA == nodeB) {
            return;
        }
        float newDistance = Mathf.Abs(nodeA.Distance(nodeB));
        if (distance < 0 || newDistance < distance || ConsiderCenter(newDistance, nodeB)) {
            distance = newDistance;
            NodeA = nodeA;
            NodeB = nodeB;
            CaveEnclosureB = caveEnclosureB;
        }
    }

    private bool ConsiderCenter(float newDistance, MapNode node) {
        if (strategy == ConnectionStrategy.ClosestToCenter) {
            return Mathf.Approximately(distance, newDistance) && NodeB.DistanceToCenter > node.DistanceToCenter;
        }
        return false;
    }
    public float distance = -1;
    public CaveEnclosure CaveEnclosureA;
    public CaveEnclosure CaveEnclosureB;
    public MapNode NodeA;
    public MapNode NodeB;
}

public enum ConnectionStrategy {
    None,
    First,
    ClosestToCenter
}
