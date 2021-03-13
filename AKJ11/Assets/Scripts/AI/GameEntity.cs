
using UnityEngine;

public class GameEntity: MonoBehaviour {
    public MapNode Node {get; private set;}
    public virtual void Initialize (GameEntityConfig entityConfig, MapNode node) {
        Node = node;
    }
    public virtual void WakeUp () {
        GameStateManager.main.EntityWokeUp(this);
    }

    public virtual void Die() {
        GameStateManager.main.EntityDied(this);
    }
}