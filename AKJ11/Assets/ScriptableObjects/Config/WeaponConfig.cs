using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "WeaponConfig", menuName = "Configs/WeaponConfig", order = 0)]
public class WeaponConfig : ScriptableObject {

    [field: SerializeField]
    public WeaponType type { get; private set; }

    [field: SerializeField]
    public float Damage { get; private set; }

    [field: SerializeField]
    public Sprite WeaponSprite { get; private set; }

    [field: SerializeField]
    public Projectile Projectile { get; private set; }
    
    [field: SerializeField]
    public GameSoundType AnimationSound { get; private set; }
    [field: SerializeField]
    public GameSoundType ProjectileSound { get; private set; }
}
