using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "EnemyConfig", menuName = "Configs/EnemyConfig", order = 0)]
public class EnemyConfig : ScriptableObject {

    [field: SerializeField]
    public float MoveSpeed {get; private set;}

    [field: SerializeField]
    public float AttackRange {get; private set;} = 1.0f;

    [field: SerializeField]
    public float AggroRange {get; private set;} = 3.0f;

    [field: SerializeField]
    public HealthScriptableObject HealthConfig { get; private set; }

    [field: SerializeField]
    public EnemyExperienceGainConfig ExpGainConfig { get; private set; }

    [field: SerializeField]
    public WeaponConfig WeaponConfig { get; private set; }

    [field: SerializeField]
    public RuntimeAnimatorController AnimatorController { get; private set; }

    [field: SerializeField]
    public float DashSpeed { get; private set; }

    [field: SerializeField]
    public float DashDistance { get; private set; }

    [field: SerializeField]
    public float DashMinDelay { get; private set; }

    [field: SerializeField]
    public float DashMaxDelay { get; private set; }

    [field: SerializeField]
    public Color ColorTint { get; private set; } = Color.white;
}