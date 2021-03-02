using UnityEngine;

[CreateAssetMenu(fileName = "ResourceGainConfig", menuName = "Configs/New ResourceGainConfig")]
public class ResourceGainConfig : ScriptableObject
{

    [field: SerializeField]
    public Sprite Sprite {get; private set;}
    [field: SerializeField]
    public Color ColorTint {get; private set;}

    [field: SerializeField]
    public ResourceType Type {get; private set;}

    [field: SerializeField]
    public int amount {get; private set;} = 1;

    public void Gain() {
        if (Type == ResourceType.XP) {
            Experience.main.AddExperience(amount);
            if (SoundManager.main != null) {
                SoundManager.main.PlaySound(GameSoundType.PickupXP);
            }
        }
        if (Type == ResourceType.HP) {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null) {
                Hurtable playerHurtable = player.GetComponent<Hurtable>();
                if (playerHurtable != null) {
                    playerHurtable.GainHealth(amount);
                }
            }
            if (SoundManager.main != null) {
                SoundManager.main.PlaySound(GameSoundType.Heal);
            }
        }
    }

}

public enum ResourceType {
    None,
    HP,
    XP
}