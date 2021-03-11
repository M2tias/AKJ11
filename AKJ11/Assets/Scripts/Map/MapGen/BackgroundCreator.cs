using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundCreator {

    public static void Create(NodeContainer nodeContainer, MapConfig config)
    {
        float offset = (config.Size % 2) * 0.5f;
        Vector2 tiledPosition = new Vector2(config.Size / 2 - 0.5f + offset, config.Size / 2 - 0.5f + offset);
        TiledBackground floor = Prefabs.Get<TiledBackground>();
        floor.Initialize(
            config.CaveTileStyle.GroundSprite,
            config.CaveTileStyle.GroundTint,
            nodeContainer.ViewContainer,
            -100,
            new Vector2Int(config.Size, config.Size),
            tiledPosition
        );
        floor.name = "Floor";

        int outsideSize = 10;
        CreateBGSprite(nodeContainer, config, "OutsideTop", new Vector2(tiledPosition.x, config.Size + outsideSize / 2 - 0.5f), new Vector2Int(config.Size + outsideSize * 2, outsideSize));
        CreateBGSprite(nodeContainer, config, "OutsideRight", new Vector2(outsideSize / 2 + config.Size - 0.5f, tiledPosition.y), new Vector2Int(outsideSize, config.Size));
        CreateBGSprite(nodeContainer, config, "OutsideLeft", new Vector2(-outsideSize / 2 - 0.5f, tiledPosition.y), new Vector2Int(outsideSize, config.Size));
        CreateBGSprite(nodeContainer, config, "OutsideBot", new Vector2(tiledPosition.x, -outsideSize / 2 - 0.5f), new Vector2Int(config.Size + outsideSize * 2, outsideSize));
    }

    private static void CreateBGSprite(NodeContainer nodeContainer, MapConfig config, string spriteName, Vector2 position, Vector2Int size) {
        TiledBackground bgSprite = Prefabs.Get<TiledBackground>();
        bgSprite.Initialize(
            config.CaveTileStyle.GroundSprite,
            config.CaveTileStyle.ColorTint,
            nodeContainer.ViewContainer,
            -101,
            size,
            position
        );
        bgSprite.name = spriteName;
    }
}
