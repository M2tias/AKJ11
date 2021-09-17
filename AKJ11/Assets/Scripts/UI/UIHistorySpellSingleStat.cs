using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHistorySpellSingleStat : MonoBehaviour
{
    [SerializeField]
    private Text txtValue;

    [SerializeField]
    private Image bgImg;
    [SerializeField]
    private Image iconImg;
    [SerializeField]
    private Image iconBgImg;

    public void Initialize(string tooltip) {
        GetComponent<UITooltipTrigger>().Initialize(tooltip);
    }

    public void GrayOut(Color bg, Color fg) {
        txtValue.color = fg;
        bgImg.color = bg;
        iconImg.color = fg;
        iconBgImg.color = bg;
        txtValue.text = "-";
        GetComponent<UITooltipTrigger>().Hide();
    }

}
