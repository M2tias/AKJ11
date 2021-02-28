using System;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class MapNodeView : MonoBehaviour
{

    private MapNode mapNode;
    private SpriteRenderer spriteRenderer;

    private int spriteConfig = -1;
    [SerializeField]
    private SpriteRenderer groundSprite;
    private Color originalColor;
    private Sprite originalSprite;

    MapConfig config;

    private TileStyle style;

    public void Initialize(MapNode mapNode, Transform container, MapConfig config)
    {
        this.mapNode = mapNode;
        transform.SetParent(container);
        transform.position = (Vector2)mapNode.Position;
        name = $"X: {mapNode.WorldX} Y: {mapNode.WorldY}";
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
        originalSprite = spriteRenderer.sprite;
        //wallSprite = config.WallSprite;
        groundSprite.enabled = false;
        this.config = config;
    }
    public void Render()
    {
        spriteRenderer.sprite = GetSprite();
        spriteRenderer.color = GetColor();
        spriteRenderer.sortingOrder = GetOrder();
    }

    private int GetOrder() {
        return GetStyle().LayerOrder;
    }

    private TileStyle GetStyle () {
        //return mapNode.IsCave ? config.CaveTileStyle : (mapNode.IsTower ? config.TowerTileStyle : config.DefaultTileStyle);
        return style;
    }

    private Color GetColor() {
        TileStyle style = GetStyle();
        return mapNode.IsWall ? style.ColorTint : style.GroundTint;
    }

    private Sprite GetSprite() {
        
        TileStyle style = GetStyle();
        if (!mapNode.IsWall) {
            return style.GroundSprite;
        }
        if (spriteConfig >= 0) {
            return config.GetSprite(spriteConfig, style);
        }
        return style.GroundSprite;
    }

    public void ShowFloorSprite() {
        groundSprite.enabled = true;
        TileStyle style = GetStyle();
        groundSprite.sprite = style.GroundSprite;
        groundSprite.color = style.GroundTint;

    }
    public void SetStyle(TileStyle style) {
        this.style = style;
    }

    public void SetSpriteConfig(int spriteConfig){
        this.spriteConfig = spriteConfig;
    }

}