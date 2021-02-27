using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class EnclosureEdgeFinder
{

    public static async UniTask FindEdges(NodeContainer nodeContainer, List<CaveEnclosure> enclosures)
    {
        DelayCounter counter = new DelayCounter(50);
        foreach(CaveEnclosure enclosure in enclosures){
            List<MapNode> nodes = enclosure.Nodes;
            foreach (MapNode node in nodes)
            {
                node.SetColor(Color.yellow);
                await Configs.main.Debug.DelayIfCounterFinished(counter);
                bool hasNeighborsNotInRoom = nodeContainer.FindAllNeighbors(node).Any(neighbor => !enclosure.HasNode(neighbor));
                if (hasNeighborsNotInRoom)
                {
                    enclosure.AddEdge(node);
                    node.SetColor(Color.black);
                }
            }
        }
    }
}
