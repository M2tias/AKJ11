using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextLevelKey : MonoBehaviour
{

    public void Initialize(Vector2Int pos)
    {
        transform.position = (Vector2)pos;
    }

    void OnTriggerEnter2D(Collider2D other) {

        if (other.gameObject.tag == "Player") {
            MapGenerator.main.FoundKey();
            Destroy(gameObject);
        }
    }
}
