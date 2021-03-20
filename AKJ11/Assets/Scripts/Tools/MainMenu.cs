using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Cysharp.Threading.Tasks;

public class MainMenu : MonoBehaviour
{

    [SerializeField]
    private InputField seedInput;

    private FadeOptions fadeToBlack = new FadeOptions(Color.black, 0.2f);

    public void SetUpRng()
    {
        string seed = seedInput.text.Trim();
        if (seed.Length > 0)
        {
            try
            {
                int intSeed = Int32.Parse(seed);
                RandomNumberGenerator.SetInstance(new RandomNumberGenerator(intSeed));
            }
            catch (FormatException)
            {
                RandomNumberGenerator.SetInstance(new RandomNumberGenerator(seed));
            }
        }
        else
        {
            RandomNumberGenerator.SetInstance(new RandomNumberGenerator());
        }
    }



    public void LoadGameScene()
    {
        SetUpRng();
        FadeAndLoadGameScene();
    }

    private async UniTask FadeAndLoadGameScene()
    {
        await FullscreenFade.main.Fade(fadeToBlack);
        SceneLoader.LoadGameScene();
    }
}
