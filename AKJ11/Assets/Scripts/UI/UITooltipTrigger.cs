using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UITooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    private string tooltip;
    [SerializeField]
    private bool showToolTip = false;

    public void Initialize(string tooltip) {
        this.tooltip = tooltip;
        showToolTip = true;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (showToolTip)
        {
            UITooltip.main.Show(tooltip);
        }
    }

    public void Hide() {
        showToolTip = false;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (showToolTip)
        {
            UITooltip.main.Hide();
        }
    }
}
