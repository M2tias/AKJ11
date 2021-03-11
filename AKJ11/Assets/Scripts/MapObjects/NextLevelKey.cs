using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextLevelKey : MonoBehaviour
{

    private NextLevelTrigger nextLevelTrigger;
    public void Initialize(Vector2Int pos, NextLevelTrigger nextLevelTrigger)
    {
        this.nextLevelTrigger = nextLevelTrigger;
        transform.position = (Vector2)pos;
    }

    void OnTriggerEnter2D(Collider2D other) {

        if (other.gameObject.tag == "Player") {
            nextLevelTrigger.Enable();
            if (SoundManager.main != null) {
                SoundManager.main.PlaySound(GameSoundType.FindKey);
            }
            Destroy(gameObject);
        }
    }
}
