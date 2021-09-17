using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITooltip : MonoBehaviour
{
    [SerializeField]
    private Text txtValue;

    [SerializeField]
    private Image imgBg;

    private bool followMouse = false;

    public static UITooltip main;
    [SerializeField]
    private Vector2 offset = new Vector2(5f, 5f);

    RectTransform rectTransform = null;

    private bool show = false;

    void Awake()
    {
        main = this;
    }


    private Vector2 pivotRight = new Vector2(1, 0);

    public void Show(string text)
    {
        if (rectTransform == null)
        {
            rectTransform = GetComponent<RectTransform>();
        }
        SetText(text);
        show = true;
        followMouse = true;
        bool mouseIsOnTheLeft = Input.mousePosition.x < halfOfScreenWidth;
        if (mouseIsOnTheLeft) {
            rectTransform.pivot = Vector2.zero;
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.zero;
        } else {
            rectTransform.pivot = pivotRight;
            rectTransform.anchorMin = pivotRight;
            rectTransform.anchorMax = pivotRight;
        }
        txtValue.enabled = true;
        imgBg.enabled = true;
        //contentWidth = new Vector2(transform.lossyScale.x, 0f);
    }

    public void Hide()
    {
        show = false;
        txtValue.enabled = false;
        imgBg.enabled = false;
        followMouse = false;
    }

    public void SetText(string newText)
    {
        txtValue.text = newText;
    }

    private float halfOfScreenWidth = Screen.width / 2;

    private void Update()
    {

        {
            if (followMouse)
            {
                /*if (mouseIsOnTheLeft)
                {
                    transform.position =  contentWidth + (Vector2)Input.mousePosition + offset;
                }
                else
                {
                }*/
                transform.position = (Vector2)Input.mousePosition - offset;
                if (show)
                {
                    txtValue.enabled = true;
                    imgBg.enabled = true;
                }
                //Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                //transform.position = new Vector2(mouseWorld.x, mouseWorld.y);
            }
        }

    }
}
