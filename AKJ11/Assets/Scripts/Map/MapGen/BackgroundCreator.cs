using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BackgroundCreator {

    public static void Create(NodeContainer nodeContainer, MapConfig config)
    {
        float offset = (config.Size % 2) * 0.5f;
        Vector2 tiledPosition = new Vector2(config.Size / 2 - 0.5f + offset, config.Size / 2 - 0.5f + offset);

        int outsideSize = 10;
        CreateBGSprite(nodeContainer, config, "OutsideTop", new Vector2(tiledPosition.x, config.Size + outsideSize / 2 - 0.5f), new Vector2Int(config.Size + outsideSize * 2, outsideSize));
        CreateBGSprite(nodeContainer, config, "OutsideRight", new Vector2(outsideSize / 2 + config.Size - 0.5f, tiledPosition.y), new Vector2Int(outsideSize, config.Size));
        CreateBGSprite(nodeContainer, config, "OutsideLeft", new Vector2(-outsideSize / 2 - 0.5f, tiledPosition.y), new Vector2Int(outsideSize, config.Size));
        CreateBGSprite(nodeContainer, config, "OutsideBot", new Vector2(tiledPosition.x, -outsideSize / 2 - 0.5f), new Vector2Int(config.Size + outsideSize * 2, outsideSize));
    }

    public static void CreateFloor(MapGenData data, MapConfig config, NodeContainer nodeContainer) {
        List<MapNode> extraFloorNodes = new List<MapNode>();
        Transform container = Prefabs.Get<Transform>();
        container.name ="ExtraFloor";
        container.SetParent(nodeContainer.ViewContainer);
        data.RoomsAndTower.Nodes.Where(node => !node.IsWall).ToList().ForEach(node => {
                nodeContainer.FindAllNeighbors(node)
                .Where(node => node.IsWall && !extraFloorNodes.Contains(node))
                .ToList()
                .ForEach(node => extraFloorNodes.Add(node));
        });
        foreach(MapNode node in extraFloorNodes) {
            MapNode newNode = new MapNode(node.X, node.Y, nodeContainer, container, config);
            newNode.IsWall = false;
            newNode.SetStyle(node.GetStyle());
            newNode.SetSpriteConfig(BlobGrid.EmptyTileId);
            newNode.SetOrderOffset(-5);
            newNode.Render();
        }
    }

    private static void CreateBGSprite(NodeContainer nodeContainer, MapConfig config, string spriteName, Vector2 position, Vector2Int size) {
        TiledBackground bgSprite = Prefabs.Get<TiledBackground>();
        bgSprite.Initialize(
            config.CaveTileStyle.TilesheetConfig != null ? config.CaveTileStyle.DefaultGroundSprite : Configs.main.DefaultTileSheet.GetFirstSprite(BlobGrid.EmptyTileId),
            config.CaveTileStyle.ColorTint,
            nodeContainer.ViewContainer,
            -101,
            size,
            position
        );
        bgSprite.name = spriteName;
    }
}
