using System;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class MapNodeView: MonoBehaviour {

    private MapNode mapNode;
    private SpriteRenderer spriteRenderer;

    private Color originalColor;

    public void Initialize(MapNode mapNode, Transform container) {
        this.mapNode = mapNode;
        transform.SetParent(container);
        transform.position = (Vector2)mapNode.Position;
        name = $"X: {mapNode.WorldX} Y: {mapNode.WorldY}";
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
    }

    public void Render() {
        spriteRenderer.sprite = mapNode.IsWall ? Configs.main.Map.WallSprite : Configs.main.Map.GroundSprite;
    }

    public void ReSetColor() {
        spriteRenderer.color = originalColor;
    }
    public void SetColor(Color color) {
        spriteRenderer.color = color;
    }
    public void SetColorAndRender(Color color) {
        SetColor(color);
        Render();
    }
}