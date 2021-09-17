using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHealth : MonoBehaviour
{
    [SerializeField]
    private Text txtCount;
    [SerializeField]
    private Image imgFill;

    public static UIHealth main;
    void Awake()
    {
        main = this;
    }
    public void SetHp(float current, float max) {
        float clamped = Mathf.Clamp(current, 0, max);
        txtCount.text = clamped.ToString();
        imgFill.fillAmount = clamped / max;
        imgFill.color = Configs.main.GradientConfig.GetHealthColorAt(imgFill.fillAmount);
    }

}
