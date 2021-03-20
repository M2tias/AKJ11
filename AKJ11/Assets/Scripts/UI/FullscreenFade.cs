using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

public class FullscreenFade : MonoBehaviour
{

    private Image imgFade;
    public static FullscreenFade main;

    private float fadingTimer;

    private bool isFading = false;

    private FadeOptions options;

    private Color originalColor;

    private void Awake() {
        main = this;
        imgFade = GetComponentInChildren<Image>();
        if (Configs.main != null && Configs.main.Debug != null && Configs.main.Debug.DisableFader) {
            imgFade.color = Color.clear;
        }
    }

    public async UniTask Fade(FadeOptions options)
    {
        if (Configs.main != null && Configs.main.Debug != null && Configs.main.Debug.DisableFader) {
            imgFade.color = Color.clear;
            return;
        }
        if (options == null) {
            options = new FadeOptions();
        }
        this.options = options;
        StartFading();
        await UniTask.WaitUntil(() => !isFading);
    }

    private void StartFading()
    {
        originalColor = imgFade.color;
        fadingTimer = 0f;
        isFading = true;
    }

    void Update()
    {
        if (isFading)
        {
            fadingTimer += Time.unscaledDeltaTime / options.Duration;
            imgFade.color = Color.Lerp(originalColor, options.TargetColor, fadingTimer);
            if (fadingTimer >= 1)
            {
                imgFade.color = options.TargetColor;
                fadingTimer = 0f;
                isFading = false;
            }
        }
    }
}


public class FadeOptions
{
    public Color TargetColor;

    [Range(0f, 20f)]
    public float Duration;

    public FadeOptions(float duration = 0.2f)
    {
        TargetColor = new Color(0, 0, 0, 1);
        Duration = duration;
    }

    public FadeOptions(Color color, float duration = 0.2f)
    {
        TargetColor = color;
        Duration = duration;
    }

}