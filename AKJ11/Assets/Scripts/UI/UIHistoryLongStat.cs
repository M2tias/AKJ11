using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class UIHistoryLongStat : MonoBehaviour, IPointerClickHandler
{

    [SerializeField]
    private Text txtValue;
    [SerializeField]
    private Image imgIcon;

    private UnityAction onClickAction;

    public void Initialize(Sprite sprite, string value, string tooltip = "", UnityAction onClick=null)
    {
        GetComponent<UITooltipTrigger>().Initialize(tooltip);
        txtValue.text = value;
        imgIcon.sprite = sprite;
        onClickAction = onClick;
    }

    public void OnPointerClick(PointerEventData eventData) {
        if (onClickAction != null) {
            onClickAction.Invoke();
        }
    }
}
