using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MageBossContainer : GameEntity
{
    Boss boss;
    public override void Initialize(GameEntityConfig entityConfig, MapNode node)
    {
        base.Initialize(entityConfig, node);
        transform.position = (Vector2)node.Position;
        boss = GetComponentInChildren<Boss>();
        boss.Initialize(entityConfig, node, this);
    }

    public override void WakeUp()
    {
        base.WakeUp();
        boss.WakeUp();
    }

    public override void Die()
    {
        base.Die();
    }

}
