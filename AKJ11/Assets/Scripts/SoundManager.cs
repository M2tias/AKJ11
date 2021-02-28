using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SoundManager : MonoBehaviour
{
    public static SoundManager main;

    private void Awake() {
        main = this;
    }

    [SerializeField]
    private List<GameSound> sounds;
    public void PlaySound(GameSoundType soundType) {
        if (soundType == GameSoundType.None) {
            return;
        }
        GameSound gameSound = sounds.Where(sound => sound.Type == soundType).FirstOrDefault();
        if (gameSound != null) {
            AudioSource audio = gameSound.Get();
            if (audio != null) {
                audio.Play();
            }
        }
    }
}


public enum GameSoundType {
    None,
    FireBall,
    SkeletonDie,
    SkeletonIsHit,
    DoorOpen,
    FindKey,
    Zing,
    SkeletonShoot,
    SwordSwing
}

[System.Serializable]
public class GameSound {
    [field: SerializeField]
    public GameSoundType Type {get ; private set;}

    [field: SerializeField]
    private List<AudioSource> sounds;

    public AudioSource Get () 
    {
        if (sounds == null || sounds.Count == 0) {
            return null;
        } 
        return sounds[Random.Range(0, sounds.Count)];
    }
}