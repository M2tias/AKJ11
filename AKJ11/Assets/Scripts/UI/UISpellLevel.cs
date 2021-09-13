using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISpellLevel : MonoBehaviour
{
    [SerializeField]
    private Text txtLevel;

    [SerializeField]
    private Image imgLevelBg;


    public void SetLevel(int newLevel) {
        txtLevel.text = $"{newLevel + 1}";
        imgLevelBg.color = Configs.main.GradientConfig.GetColor(newLevel);
    }

}
