using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class AmbientEffects : MonoBehaviour
{
    public ParticleSystem HappyEffect;
    public ParticleSystem SadEffect;
    public Light2D HappyLight;

    private bool lightEnabled = false;
    private float lightTimer;
    private float lightDuration = 5.0f;

    private AudioSource music;
    private AudioSource bossMusic;

    private float bossMusicStopTimer = 0.0f;
    private float bossMusicStopDuration = 2.0f;
    private bool stopBossMusic = false;

    // Start is called before the first frame update
    void Start()
    {
        music = GameObject.FindGameObjectWithTag("Music").GetComponent<AudioSource>();
        bossMusic = GameObject.FindGameObjectWithTag("BossMusic").GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (lightEnabled)
        {
            var t = (Time.time - lightTimer) / lightDuration;
            HappyLight.intensity = Mathf.Lerp(0.0f, 1.0f, t);
        }
        else
        {
            HappyLight.intensity = 0.0f;
        }

        if (stopBossMusic)
        {
            if (bossMusicStopTimer > Time.time - bossMusicStopDuration)
            {
                var t = (Time.time - bossMusicStopTimer) / bossMusicStopDuration;
                bossMusic.pitch = Mathf.Lerp(1.0f, 0.0f, t);
            }
            else
            {
                bossMusic.Stop();
            }
        }
    }

    public void StopSad()
    {
        SadEffect.Stop();
    }

    public void SetHappy()
    {
        HappyEffect.Play();
        lightEnabled = true;
        lightTimer = Time.time;
        music.Play();
    }

    public void PlayBossMusic()
    {
        music.Stop();
        bossMusic.Play();
    }

    public void StopBossMusic()
    {
        if (stopBossMusic)
        {
            return;
        }

        stopBossMusic = true;
        bossMusicStopTimer = Time.time;
    }
}
