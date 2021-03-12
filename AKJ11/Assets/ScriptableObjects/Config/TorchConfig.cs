using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "TorchConfig", menuName = "Configs/TorchConfig", order = 0)]
public class TorchConfig : ScriptableObject
{
    [field:SerializeField]
    public TorchType Type {get; private set;}

    [field: SerializeField]
    public Vector2 spritePosition { get; private set; }

    [field: SerializeField]
    public Sprite sprite { get; private set; }

    [field: SerializeField]
    public bool spriteXFlip { get; private set; }

    [field: SerializeField]
    public Vector2 flickerAnimationPosition { get; private set; }

    [field: SerializeField]
    public string flickerAnimationState { get; private set; }

}



public enum TorchType {
    Top,
    Right,
    Bottom,
    Left
}
