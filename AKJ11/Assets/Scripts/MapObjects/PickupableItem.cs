using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupableItem : MonoBehaviour
{
    private ResourceGainConfig resourceGain;
    [SerializeField]
    private SpriteRenderer spriteRenderer;

    public void Initialize(ResourceGainConfig gain, Transform parent, Vector2Int position) {
        this.resourceGain = gain;
        spriteRenderer.sprite = gain.Sprite;
        spriteRenderer.color = gain.ColorTint;
        this.transform.SetParent(parent);
        this.transform.position = (Vector2) position;
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.tag == "Player") {
            if (resourceGain != null) {
                resourceGain.Gain();
                Destroy(gameObject);
            }
        }
    }
}