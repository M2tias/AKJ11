using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIXP : MonoBehaviour
{
    [SerializeField]
    private Text txtCount;
    [SerializeField]
    private Text txtLevel;
    [SerializeField]
    private Image imgFill;

    public static UIXP main;
    void Awake()
    {
        main = this;
    }
    public void SetXP(int current, int max, int level) {
        int clamped = Mathf.Clamp(current, 0, max);
        if (current == max) {
            clamped = 0;
        }
        txtCount.text = $"{clamped}xp / {max}xp";
        txtLevel.text = $"LVL {(level + 1).ToString()}";
        imgFill.fillAmount = (float)clamped / (float)max;
    }

}
