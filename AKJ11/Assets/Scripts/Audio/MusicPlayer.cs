using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Cysharp.Threading.Tasks;


public class MusicPlayer
{
    public static MusicPlayer main;

    private AudioSource currentMain;
    private AudioSource currentBoss;
    private MusicConfig config;

    public bool HasConfig {get {return config != null;}}

    private List<AudioFade> fades = new List<AudioFade>();

    public static MusicPlayer GetInstance() {
        if (main == null) {
            main = new MusicPlayer();
        }
        return main;
    }

    public async UniTask SetConfig(MusicConfig config) {
        if (config == null) {
            Debug.Log("Your music config is null!");
            return;
        }
        InitializeAudioSources();
        this.config = config;
        AudioSource mainSource = await HandleConfig();
        if (mainSource != null) {
            currentMain = mainSource;
        }
    }

    public async UniTask PlayBossMusic() {
        if (config.Boss == null) {
            Debug.Log("Your music config has no boss music!");
            return;
        }
        currentBoss.time = currentMain.time;
        await CrossFade(currentMain, currentBoss, 1f, 1f);
        await UniTask.Delay(5000);
    }

    public async UniTask PlayAfterBossMusic() {
        if (config.AfterBoss == null) {
            Debug.Log("Your music config has no afterboss music!");
            return;
        }

        currentMain.clip = config.AfterBoss;
        currentMain.volume = 0f;
        currentMain.Play();
        await CrossFade(currentBoss, currentMain, 5f, 10f);
    }

    private async UniTask<AudioSource> HandleConfig() {
        AudioSource source = currentMain;
        if (source.clip == null) {
            source.clip = config.Main;
            source.volume = config.Volume;
            source.Play();
        }
        if (config.Boss != null) {
            if (currentBoss == null) {
                currentBoss = InitializeAudioSource("Boss");
            }
            currentBoss.clip = config.Boss;
            currentBoss.Play();
        }
        if (source.clip != config.Main) {
            if (source.isPlaying) {
                AudioSource newSource = InitializeAudioSource(source.name + " (new)");
                newSource.clip = config.Main;
                newSource.Play();
                await CrossFade(source, newSource, config.FadeOutDuration, config.FadeInDuration);
                source.name += " (disabled)";
                return newSource;
            } else {
                source.clip = config.Main;
            }
        }
        return null;
    }

    private void InitializeAudioSources() {
        if (currentBoss == null) {
            currentBoss = InitializeAudioSource("Boss music");
        }
        if (currentMain == null) {
            currentMain = InitializeAudioSource("Main music");
        }
    }

    private AudioSource InitializeAudioSource(string name) {
        AudioSource source = Prefabs.Get<AudioSource>();
        source.volume = 0;
        source.transform.position = Vector2.zero;
        source.loop = true;
        source.name = name;
        return source;
    }

    public async UniTask Fade(AudioSource fadeSource, float targetVolume, float duration = 0.5f) {
        AudioFade fade = new AudioFade(duration, targetVolume, fadeSource);
        fades.Add(fade);
        await UniTask.WaitUntil(() => !fade.IsFading);
    }

    public async UniTask CrossFade(AudioSource fadeOutSource, AudioSource fadeInSource, float durationOut, float durationIn) {
        AudioFade fadeOut = new AudioFade(durationOut, 0f, fadeOutSource);
        AudioFade fadeIn = new AudioFade(durationIn, config.Volume, fadeInSource);
        fades.Add(fadeOut);
        fades.Add(fadeIn);
        await UniTask.WaitUntil(() => !fadeOut.IsFading && !fadeIn.IsFading);
        fadeOutSource.Stop();
    }

    public void Update() {
        for(int index = 0; index < fades.Count; index += 1) {
            AudioFade fade = fades[index];
            if (fade != null && fade.IsFading) {
                fade.Update();
            }
            if (!fade.IsFading) {
                fades.Remove(fade);
            }
        }
    }
}

public class AudioFade {
    public AudioFade(float duration, float target, AudioSource track) {
        this.duration = duration;
        IsFading = true;
        timer = 0f;
        originalVolume = track.volume;
        targetVolume = target;
        audioSource = track;
    }
    public bool IsFading {get; private set;}
    private float duration;
    private float timer;
    private float targetVolume;
    private AudioSource audioSource;
    private float originalVolume;

    public void Update() {
        timer += Time.unscaledDeltaTime / duration;
        audioSource.volume = Mathf.Lerp(originalVolume, targetVolume, timer);
        if (timer >= 1) {
            audioSource.volume = targetVolume;
            IsFading = false;
        }
    }
}