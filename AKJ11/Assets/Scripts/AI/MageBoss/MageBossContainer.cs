using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MageBossContainer : GameEntity
{
    override public void Initialize(GameEntityConfig entityConfig, MapNode node)
    {
        if (node != null)
        {
            transform.position = (Vector2)node.Position;
        }
    }

    override public void WakeUp()
    {
    }
}
