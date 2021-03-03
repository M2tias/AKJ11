using UnityEngine;

[CreateAssetMenu(fileName = "PickupableItemScriptableObject", menuName = "Configs/New PickupableItemScriptableObject")]
public class PickupableItemScriptableObject : ScriptableObject
{
    [field: SerializeField]
    public ResourceTypeConfig Config { get; private set; }

    [field: SerializeField]
    public int amount { get; private set; } = 1;

    public void Gain()
    {
        if (Config.Type == ResourceType.XP)
        {
            Experience.main.AddExperience(amount);
            if (SoundManager.main != null)
            {
                SoundManager.main.PlaySound(GameSoundType.PickupXP);
            }
        }
        if (Config.Type == ResourceType.HP)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                Hurtable playerHurtable = player.GetComponent<Hurtable>();
                if (playerHurtable != null)
                {
                    playerHurtable.GainHealth(amount);
                }
            }
            if (SoundManager.main != null)
            {
                SoundManager.main.PlaySound(GameSoundType.Heal);
            }
        }
        if (UIWorldCanvas.main != null)
        {
            UIWorldCanvas.main.ShowNumberAtPlayer(amount, Config.Type);
        }
    }

}