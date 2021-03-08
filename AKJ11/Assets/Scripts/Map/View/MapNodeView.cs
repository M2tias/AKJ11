using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class MapNodeView : MonoBehaviour
{

    private MapNode mapNode;
    private SpriteRenderer spriteRenderer;

    private int spriteConfig = -1;

    private Color originalColor;
    private Sprite originalSprite;

    MapConfig config;

    private TileStyle style;


    [SerializeField]
    PolygonCollider2D polygonCollider2D;

    private void UpdateCollider() {
        List<Vector2> points = new List<Vector2>();
        List<Vector2> simplifiedPoints = new List<Vector2>();
        float tolerance = 0.05f;
        polygonCollider2D.pathCount = spriteRenderer.sprite.GetPhysicsShapeCount();
        for(int i = 0; i < polygonCollider2D.pathCount; i++)
        {
            spriteRenderer.sprite.GetPhysicsShape(i, points);
            LineUtility.Simplify(points, tolerance, simplifiedPoints);
            polygonCollider2D.SetPath(i, simplifiedPoints);
        }
    }

    public void Initialize(MapNode mapNode, Transform container, MapConfig config)
    {
        this.mapNode = mapNode;
        transform.SetParent(container);
        transform.position = (Vector2)mapNode.Position;
        name = $"X: {mapNode.WorldX} Y: {mapNode.WorldY}";
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
        originalSprite = spriteRenderer.sprite;
        this.config = config;
    }
    public void Render()
    {
        Sprite sprite = GetSprite();
        Sprite oldSprite = spriteRenderer.sprite;
        spriteRenderer.sprite = sprite;
        if (sprite != oldSprite && spriteConfig != BlobGrid.EmptyTileId)
        {
            UpdateCollider();
        }

        if (spriteConfig != BlobGrid.EmptyTileId) {
            polygonCollider2D.enabled = mapNode.IsWall;
        }

        spriteRenderer.color = GetColor();
        spriteRenderer.sortingOrder = GetOrder();

        spriteRenderer.enabled = mapNode.IsWall;

    }

    private int GetOrder()
    {
        return GetStyle().LayerOrder;
    }

    private TileStyle GetStyle()
    {
        return style;
    }

    private Color GetColor()
    {
        TileStyle style = GetStyle();
        return mapNode.IsWall ? style.ColorTint : style.GroundTint;
    }

    private Sprite GetSprite()
    {

        TileStyle style = GetStyle();
        if (!mapNode.IsWall)
        {
            return style.GroundSprite;
        }
        if (spriteConfig >= 0)
        {
            return config.GetSprite(spriteConfig, style);
        }
        return style.GroundSprite;
    }
    public void SetStyle(TileStyle style)
    {
        this.style = style;
    }

    public void SetSpriteConfig(int spriteConfig)
    {
        this.spriteConfig = spriteConfig;
    }

}