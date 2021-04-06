using System.Linq;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

public class RoomSealer {

    public static List<MapNode> FindHallwayNodes(MapGenData data) {
        List<MapNode> hallwayNodes = new List<MapNode>();
        foreach(CaveEnclosure room in data.Rooms) {
            foreach(MapNode edgeNode in room.Edges) {
                foreach (MapNode neighbor in edgeNode.Neighbors) {
                    if (!neighbor.IsWall && !room.Nodes.Contains(neighbor) && !hallwayNodes.Contains(neighbor)) {
                        hallwayNodes.Add(neighbor);
                    }
                }
            }
        }
        foreach(MapNode edgeNode in data.Tower.Edges) {
            foreach (MapNode neighbor in edgeNode.Neighbors) {
                if (!neighbor.IsWall && !data.Tower.Nodes.Contains(neighbor) && !hallwayNodes.Contains(neighbor)) {
                    hallwayNodes.Add(neighbor);
                }
            }
        }
        return hallwayNodes;
    }

    public static async UniTask SealTower(MapGenData data, bool destroyable = false) {
        List<MapNode> hallwayNodes = new List<MapNode>();
        foreach(MapNode edgeNode in data.Tower.Edges) {
            foreach (MapNode neighbor in edgeNode.Neighbors) {
                if (!neighbor.IsWall && !data.Tower.Nodes.Contains(neighbor)) {
                    hallwayNodes.Add(neighbor);
                }
            }
        }
        foreach(MapNode hallwayNode in hallwayNodes) {
            hallwayNode.Seal(destroyable);
        }
        await MapGenerator.main.RunBlobGrid();
    }

    public static async UniTask SealAllRooms(MapGenData data, List<MapNode> hallwayNodes, bool destroyable = false) {
        foreach(MapNode hallwayNode in hallwayNodes) {
            hallwayNode.Seal(destroyable);
        }
        await MapGenerator.main.RunBlobGrid();
    }

    public static async UniTask UnsealAllRooms(List<MapNode> hallwayNodes) {
        if (hallwayNodes.Count > 0) {
            foreach(MapNode hallwayNode in hallwayNodes) {
                hallwayNode.Unseal();
            }
            await MapGenerator.main.RunBlobGrid();
        }
    }
}