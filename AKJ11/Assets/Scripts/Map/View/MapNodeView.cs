using System;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class MapNodeView: MonoBehaviour {

    private MapNode mapNode;
    private SpriteRenderer spriteRenderer;
    [SerializeField]
    private Sprite wallSprite;

    public void Initialize(MapNode mapNode, Transform container) {
        this.mapNode = mapNode;
        transform.SetParent(container);
        transform.position = mapNode.Position;
        name = $"X: {mapNode.WorldX} Y: {mapNode.WorldY}";
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Render() {
        spriteRenderer.sprite = mapNode.IsWall ? wallSprite : null;
    }
}