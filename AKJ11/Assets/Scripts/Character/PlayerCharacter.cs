using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter : MonoBehaviour {
    private static PlayerCharacter main;
    public static PlayerCharacter GetInstance() {
        if (main == null) {
            main = Prefabs.Get<PlayerCharacter>();
        }
        return main;
    }

    public MapNode Node {get; private set;}

    public void SetPosition(MapNode mapNode) {
        Node = mapNode;
        transform.position = (Vector2)mapNode.Position;
        Movement movement = GetComponentInChildren<Movement>();
        if (movement != null)
        {
            movement.transform.localPosition = Vector2.zero;
        }
    }

}
