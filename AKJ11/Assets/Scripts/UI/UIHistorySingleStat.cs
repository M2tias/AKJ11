using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class UIHistorySingleStat : MonoBehaviour
{
    [SerializeField]
    private Text txtValue;
    [SerializeField]
    private Image imgIcon;

    public void Initialize(Sprite sprite, string value, string tooltip="") {
        txtValue.text = value;
        imgIcon.sprite = sprite;
        GetComponent<UITooltipTrigger>().Initialize(tooltip);
    }

    [SerializeField]
    private bool showToolTip = false;

}
