using UnityEngine;

[CreateAssetMenu(fileName = "MusicConfig", menuName = "Configs/New MusicConfig")]
public class MusicConfig: ScriptableObject {
    [field: SerializeField]
    [field: Range(0, 1f)]
    public float Volume {get; private set;}
    [field: SerializeField]
    [field: Range(0, 5f)]
    public float FadeInDuration {get; private set;}
    [field: SerializeField]
    [field: Range(0, 5f)]
    public float FadeOutDuration {get; private set;}
    [field: SerializeField]
    public AudioClip Main {get; private set;}
    [field: SerializeField]
    public AudioClip Boss {get; private set;}
    [field: SerializeField]
    public AudioClip AfterBoss {get; private set;}
}