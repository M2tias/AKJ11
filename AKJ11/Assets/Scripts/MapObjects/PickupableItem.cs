using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupableItem : MonoBehaviour
{
    private PickupableItemScriptableObject item;
    [SerializeField]
    private SpriteRenderer spriteRenderer;

    public void Initialize(PickupableItemScriptableObject item, Transform parent, Vector2Int position) {
        this.item = item;
        spriteRenderer.sprite = item.Config.Sprite;
        spriteRenderer.color = item.Config.SpriteColorTint;
        this.transform.SetParent(parent);
        this.transform.position = (Vector2) position;
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.tag == "Player") {
            if (item != null) {
                if (item.Config.Type == ResourceType.HP) {
                    Hurtable playerHurtable = other.gameObject.GetComponent<Hurtable>();
                    if (playerHurtable.HasMaxHealth()) {
                        return;
                    }
                    RunHistoryDb.AddPotion();
                }
                item.Gain();
                Destroy(gameObject);
            }
        }
    }
}