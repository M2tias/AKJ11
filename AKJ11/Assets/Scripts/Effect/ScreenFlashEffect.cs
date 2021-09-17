using UnityEngine;
using System;
using UnityEngine.UI;

public class ScreenFlashEffect : MonoBehaviour
{
    private bool flash = false;

    private ScreenFlashOptions options;

    private float flashTimer = 0f;

    private bool flashing = false;
    private bool flashingBack = false;

    public static ScreenFlashEffect main;

    [SerializeField]
    private ScreenFlashOptions defaultOptions;

    [SerializeField]
    private Image imgTarget;

    private Color initialColor;

    void Awake()
    {
        main = this;
    }

    public void Flash(ScreenFlashOptions newOptions)
    {
        if (flashing || flashingBack)
        {
            return;
        }
        options = newOptions;
        initialColor = imgTarget.color;
        flash = true;
        flashing = true;
        flashingBack = false;
        flashTimer = options.FlashDuration;
    }

    public void Flash()
    {
        Flash(defaultOptions);
    }

    void Update()
    {
        if (!flash)
        {
            return;
        }
        if (flashTimer <= 0 && options.FlashBack && !flashingBack)
        {
            flashTimer = 0f;
            flashingBack = true;
        }
        flashTimer -= Time.deltaTime;
        if (flashTimer > 0)
        {
            if (flashingBack)
            {
                imgTarget.color = Color.Lerp(
                    options.FlashColor,
                    initialColor,
                    (options.FlashDurationBack - flashTimer) / options.FlashDurationBack
                );
            }
            else
            {
                imgTarget.color = Color.Lerp(
                    initialColor,
                    options.FlashColor,
                    (options.FlashDuration - flashTimer) / options.FlashDuration
                );
            }
        }
        else
        {
            flashing = false;
            flash = false;
            flashTimer = 0f;
            imgTarget.color = initialColor;
        }
    }

}

[System.Serializable]
public class ScreenFlashOptions
{
    public Color FlashColor = Color.red;
    public float FlashDuration = 0.5f;
    public bool FlashBack = true;
    public float FlashDurationBack = 0.5f;
}