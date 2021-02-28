using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class EnclosureEdgeFinder
{

    public static async UniTask FindEdges(NodeContainer nodeContainer, List<CaveEnclosure> enclosures, MapConfig config = null)
    {
        DelayCounter counter = new DelayCounter(50);
        foreach(CaveEnclosure enclosure in enclosures){
            List<MapNode> nodes = enclosure.Nodes;
            foreach (MapNode node in nodes)
            {
                await Configs.main.Debug.DelayIfCounterFinished(counter);
                List<MapNode> neighborsNotInRoom = nodeContainer.FindAllNeighbors(node).Where(neighbor => !enclosure.HasNode(neighbor)).ToList();
                bool hasNeighborsNotInRoom = neighborsNotInRoom.Count > 0;
                if (hasNeighborsNotInRoom)
                {
                    enclosure.AddEdge(node);
                    node.IsEdge = true;
                    if (config != null) {
                        foreach(MapNode nNode in neighborsNotInRoom) {
                            //nNode.ShowFloorSprite();
                        }
                    }
                }
            }
        }
    }
}
