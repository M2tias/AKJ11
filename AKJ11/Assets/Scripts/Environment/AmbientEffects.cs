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

    // Start is called before the first frame update
    void Start()
    {
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
    }
}
