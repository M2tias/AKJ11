using UnityEngine;

[CreateAssetMenu(fileName = "ResourceTypeConfig", menuName = "Configs/New ResourceTypeConfig")]
public class ResourceTypeConfig : ScriptableObject
{

    [field: SerializeField]
    public Sprite Sprite { get; private set; }
    [field: SerializeField]
    public Color SpriteColorTint { get; private set; }

    [field: SerializeField]
    public string Abbreviation {get; private set;}
    [field: SerializeField]
    public Color TextColorWhenPositive { get; private set; } = Color.green;
    [field: SerializeField]
    public Color TextColorWhenNegative { get; private set; } = Color.red;

    [field: SerializeField]
    public ResourceType Type { get; private set; }


}

public enum ResourceType
{
    None,
    HP,
    XP
}