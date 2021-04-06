using System.Linq;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

public class RoomSealer {

    public static List<MapNode> FindNodesToSeal(MapGenData data) {
        List<MapNode> sealNodes = new List<MapNode>();
        foreach(CaveEnclosure room in data.Rooms) {
            foreach(MapNode edgeNode in room.Edges) {
                foreach (MapNode neighbor in edgeNode.Neighbors) {
                    if (!neighbor.IsWall && !room.Nodes.Contains(neighbor) && !sealNodes.Contains(neighbor)) {
                        sealNodes.Add(neighbor);
                    }
                }
            }
        }
        foreach(MapNode edgeNode in data.Tower.Edges) {
            foreach (MapNode neighbor in edgeNode.Neighbors) {
                if (!neighbor.IsWall && !data.Tower.Nodes.Contains(neighbor) && !sealNodes.Contains(neighbor)) {
                    sealNodes.Add(neighbor);
                }
            }
        }
        return sealNodes;
    }

    public static async UniTask<List<MapNode>> SealTower(MapGenData data, NodeContainer nodeContainer, bool destroyable = false) {
        List<MapNode> sealNodes = new List<MapNode>();
        foreach(MapNode edgeNode in data.Tower.Edges) {
            if (!sealNodes.Contains(edgeNode) && nodeContainer.FindAllNeighbors(edgeNode).Any(neighbor => !neighbor.IsWall && !data.Tower.Nodes.Contains(neighbor))) {
                sealNodes.Add(edgeNode);
            }
        }
        foreach(MapNode sealNode in sealNodes) {
            sealNode.Seal(destroyable);
        }
        await MapGenerator.main.RunBlobGrid();
        return sealNodes;
    }

    public static async UniTask SealAllRooms(MapGenData data, List<MapNode> sealNodes, bool destroyable = false) {
        foreach(MapNode sealNode in sealNodes) {
            sealNode.Seal(destroyable);
        }
        await MapGenerator.main.RunBlobGrid();
    }

    public static async UniTask UnsealAllRooms(List<MapNode> sealNodes) {
        if (sealNodes != null && sealNodes.Count > 0) {
            foreach(MapNode sealNode in sealNodes) {
                sealNode.Unseal();
            }
            await MapGenerator.main.RunBlobGrid();
        }
    }
}