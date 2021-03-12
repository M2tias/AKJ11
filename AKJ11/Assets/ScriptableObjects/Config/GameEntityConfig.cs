using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "GameEntityConfig", menuName = "Configs/GameEntityConfig", order = 0)]
public class GameEntityConfig : ScriptableObject
{

    [field: SerializeField]
    public GameEntity EntityPrefab { get; private set; }

    [SerializeField]
    private EntityType entityType = EntityType.Enemy;
    public EntityType EntityType {get {return entityType;}}

    [SerializeField]
    [ConditionalHide("entityType", EntityType.Enemy)]
    private EnemyConfig enemyConfig;
    public EnemyConfig EnemyConfig {get {return enemyConfig;}}
}

public enum EntityType {
    Enemy,
    Boss
}