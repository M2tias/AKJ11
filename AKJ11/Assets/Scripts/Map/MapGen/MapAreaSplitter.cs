using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapAreaSplitter
{
    public static List<MapArea> GetSplitAreas(MapConfig mapConfig) {
        RandomNumberGenerator rng = RandomNumberGenerator.GetInstance();
        int centerRadius = mapConfig.TowerRadius;
        int padding = mapConfig.Padding;
        int mapWidth = mapConfig.Size;
        int mapHeight = mapConfig.Size;
        int halfWidth = mapWidth / 2;
        int halfHeight = mapHeight / 2;

        List<MapArea> areas = new List<MapArea>();
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
        areas.Add(new MapArea(topLeft, MapAreaType.TopLeft));
        areas.Add(new MapArea(topRight, MapAreaType.TopRight));
        areas.Add(new MapArea(botRight, MapAreaType.BotRight));
        areas.Add(new MapArea(botLeft, MapAreaType.BotLeft));
        if (Configs.main.Debug.ShowAreaBorders) {
            foreach(MapArea area in areas) {
                area.Show();
            }
        }
        while(areas.Count > mapConfig.NumberOfAreas) {
            areas.RemoveAt(rng.Range(0, areas.Count));
        }

        return areas;
    }

}
public class MapArea {
    public RectInt Rect;
    public MapAreaType Type;
    private SpriteRenderer spriteRenderer;
    public MapArea(RectInt rect, MapAreaType type ) {
        Rect = rect;
        Type = type;
    }
    public void Show() {
        if (spriteRenderer == null) {
            SpriteRenderer spriteRenderer = Prefabs.Get<SpriteRenderer>();
            spriteRenderer.transform.position = Rect.center;
            spriteRenderer.sprite = Configs.main.Debug.BorderSprite;
            spriteRenderer.drawMode = SpriteDrawMode.Sliced;
            spriteRenderer.sortingOrder = 500;
            spriteRenderer.size = Rect.size;
        }
    }
}

public enum MapAreaType {
    None,
    TopLeft,
    TopRight,
    BotRight,
    BotLeft
}
