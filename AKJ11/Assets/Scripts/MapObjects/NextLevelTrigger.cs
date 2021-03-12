using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextLevelTrigger : MonoBehaviour
{
    private bool triggerEnabled = false;
    [SerializeField]
    private SpriteRenderer enabledSprite;
    [SerializeField]
    private SpriteRenderer disabledSprite;

    public static NextLevelTrigger main;

    public void Initialize(Vector2Int midPoint) {
        main = this;
        enabledSprite.enabled = false;
        disabledSprite.enabled = true;
        transform.position = new Vector2(midPoint.x, midPoint.y - 1);
    }
    public void Enable() {
        triggerEnabled = true;
        enabledSprite.enabled = true;
        disabledSprite.enabled = false;
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (triggerEnabled) {
            if (other.gameObject.tag == "Player") {
                main = null;
                MapGenerator.main.LevelEnd();
            }
        }
    }
}
