using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class TorchSpawner
{
    public static void Spawn(MapGenData data) {
        List<MapNode> nonTowerNodes = data.NonTowerNodes;
        IEnumerable<MapNode> edgeNodes = nonTowerNodes.Where(node => node.IsEdge);

        List<MapNode> leftEdgeNodes = edgeNodes.Where(node =>
           node.Neighbors.Any(n => n.IsWall && n.X < node.X && n.Y == node.Y) &&
           node.Neighbors.Where(n => n.IsWall && n.X < node.X).Count() > 1
        ).ToList();

        List<MapNode> rightEdgeNodes = edgeNodes.Where(node =>
            node.Neighbors.Any(n => n.IsWall && n.X > node.X && n.Y == node.Y) &&
            node.Neighbors.Where(n => n.IsWall && n.X > node.X).Count() > 1
        ).ToList();

        List<MapNode> topEdgeNodes = edgeNodes.Where(node =>
            node.Neighbors.Any(n => n.IsWall && n.X == node.X && n.Y > node.Y) &&
            node.Neighbors.Where(n => n.IsWall && n.Y > node.Y).Count() > 1
        ).ToList();

        List<MapNode> bottomEdgeNodes = edgeNodes.Where(node =>
            node.Neighbors.Any(n => n.IsWall && n.X == node.X && n.Y < node.Y) &&
            node.Neighbors.Where(n => n.IsWall && n.Y < node.Y).Count() > 1
        ).ToList();

        float spawnPitch = 4f;

        TorchConfig top = Configs.main.TorchConfigs.Where(torch => torch.Type == TorchType.Top).FirstOrDefault();
        TorchConfig right = Configs.main.TorchConfigs.Where(torch => torch.Type == TorchType.Right).FirstOrDefault();
        TorchConfig bottom = Configs.main.TorchConfigs.Where(torch => torch.Type == TorchType.Bottom).FirstOrDefault();
        TorchConfig left = Configs.main.TorchConfigs.Where(torch => torch.Type == TorchType.Left).FirstOrDefault();
        SpawnTorchesSide(data.NodeContainer, topEdgeNodes, spawnPitch, top, "top");
        SpawnTorchesSide(data.NodeContainer, rightEdgeNodes, spawnPitch, right, "right");
        SpawnTorchesSide(data.NodeContainer, leftEdgeNodes, spawnPitch, left, "left");
        SpawnTorchesSide(data.NodeContainer, bottomEdgeNodes, spawnPitch, bottom, "bottom");
    }

    private static void SpawnTorchesSide(NodeContainer nodeContainer, List<MapNode> edgeNodes, float spawnPitch, TorchConfig config, string direction)
    {
        for (int index = 0; index < Mathf.RoundToInt(edgeNodes.Count / spawnPitch); index += 1)
        {
            try
            {
                SpawnTorch(nodeContainer, edgeNodes, spawnPitch, index, config, direction);
            }
            catch (Exception e)
            {
                MonoBehaviour.print(e);
            }
        }
    }

    private static void SpawnTorch(NodeContainer nodeContainer, List<MapNode> edgeNodes, float spawnPitch, int index, TorchConfig config, string direction)
    {
        MapNode randomNode = edgeNodes[Math.Min(Mathf.RoundToInt(index * spawnPitch), edgeNodes.Count - 1)];
        Torch torch = Prefabs.Get<Torch>();
        torch.GetComponent<Torch>().Initialize(config);
        torch.transform.SetParent(nodeContainer.ViewContainer);
        Vector2 nodePos2 = (Vector2)randomNode.Position;
        torch.transform.position = new Vector2(nodePos2.x, nodePos2.y);
        torch.name = "torch_" + direction + "_X:" + randomNode.X + "|Y:" + randomNode.Y;
    }
}
