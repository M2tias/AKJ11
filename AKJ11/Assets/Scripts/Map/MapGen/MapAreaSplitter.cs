using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapAreaSplitter
{
    public static List<RectInt> GetSplitAreas(MapConfig mapConfig) {
        int centerRadius = mapConfig.TowerRadius;
        int padding = mapConfig.Padding;
        int mapWidth = mapConfig.Size;
        int mapHeight = mapConfig.Size;
        int halfWidth = mapWidth / 2;
        int halfHeight = mapHeight / 2;

        List<RectInt> areas = new List<RectInt>();
        RectInt topLeft = new RectInt(
            new Vector2Int(padding, padding + halfHeight + centerRadius),
            new Vector2Int(mapWidth - (halfWidth - centerRadius) - (padding * 2), halfHeight - centerRadius - padding * 2)
        );
        RectInt topRight = new RectInt(
            new Vector2Int(topLeft.width + (padding * 2) + padding, padding + halfHeight - centerRadius),
            new Vector2Int(halfWidth - centerRadius - padding * 2, mapHeight - (halfHeight - centerRadius) - padding * 2)
        );
        RectInt botRight = new RectInt(
            new Vector2Int(
                topRight.width + padding * 2 + padding,
                padding
            ),
            new Vector2Int(topLeft.width, mapHeight - topRight.height - padding * 3)
        );
        RectInt botLeft = new RectInt(
            new Vector2Int(
                padding,
                padding
            ),
            new Vector2Int(mapWidth - botRight.width - padding * 3, mapHeight - topLeft.height - padding * 3)
        );
        areas.Add(topLeft);
        areas.Add(topRight);
        areas.Add(botRight);
        areas.Add(botLeft);
        while(areas.Count > mapConfig.NumberOfAreas) {
            areas.RemoveAt(Random.Range(0, areas.Count));
        }

        return areas;
    }
}
