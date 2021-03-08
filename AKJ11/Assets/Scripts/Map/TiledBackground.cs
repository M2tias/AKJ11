using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TiledBackground : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer spriteRenderer;
    public void Initialize(Sprite sprite, Color color, Transform parent, int order, Vector2Int size, Vector2 position) {
        transform.SetParent(parent, true);
        transform.localPosition = position;
        spriteRenderer.sprite = sprite;
        spriteRenderer.sortingOrder = order;
        spriteRenderer.size = size;
        spriteRenderer.color = color;
    }
}
