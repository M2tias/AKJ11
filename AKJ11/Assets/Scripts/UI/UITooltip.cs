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

    void Awake() {
        main = this;
    }

    public void Show(string text) {
        txtValue.enabled = true;
        imgBg.enabled = true;
        followMouse = true;
        SetText(text);
    }

    public void Hide() {
        txtValue.enabled = false;
        imgBg.enabled = false;
        followMouse = false;
    }

    public void SetText(string newText) {
        txtValue.text = newText;
    }

    private void Update() {
        if (followMouse) {
            transform.position = (Vector2)Input.mousePosition - offset;
            //Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            //transform.position = new Vector2(mouseWorld.x, mouseWorld.y);
        }
    }

}
