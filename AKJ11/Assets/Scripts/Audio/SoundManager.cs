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
    SwordSwing,
    Heal,
    PickupXP,
    OpenSpellBook,
    CloseSpellBook,
    WallBreak
}

[System.Serializable]
public class GameSound {
    [field: SerializeField]
    public GameSoundType Type {get ; private set;}

    [field: SerializeField]
    private List<AudioSource> sounds;

    private List<GameSoundPool> soundPools = new List<GameSoundPool>();
    private bool initialized = false;

    public AudioSource Get () 
    {
        if (!initialized)
        {
            initialize();
        }

        if (sounds == null || sounds.Count == 0) {
            return null;
        } 
        return soundPools[Random.Range(0, soundPools.Count)].getAvailable();
    }

    private void initialize()
    {
        soundPools = sounds.Select(it => new GameSoundPool(it)).ToList();
        initialized = true;
    }


    private class GameSoundPool
    {
        private AudioSource originalAudioSource;
        private List<AudioSource> audioSources = new List<AudioSource>();

        public GameSoundPool(AudioSource audioSource)
        {
            originalAudioSource = audioSource;
            addNewToPool();
        }

        public AudioSource getAvailable()
        {
            var src = audioSources.Where(it => it.isPlaying == false).FirstOrDefault();
            if (src == null)
            {
                src = addNewToPool();
            }
            return src;
        }

        private AudioSource addNewToPool()
        {
            AudioSource newSource = GameObject.Instantiate(originalAudioSource, originalAudioSource.transform.parent);
            audioSources.Add(newSource);
            return newSource;
        }
    }
}