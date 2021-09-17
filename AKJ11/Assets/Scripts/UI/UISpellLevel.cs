using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UISpellLevel : MonoBehaviour
{
    [SerializeField]
    private Text txtLevel;

    [SerializeField]
    private Image imgLevelBg;

    [SerializeField]
    private Image innerBg;

    [SerializeField]
    private Image imgIcon;
    [SerializeField]
    private Image imgIconBg;
    [SerializeField]
    private Sprite spellStatSpriteDamage;
    [SerializeField]
    private Sprite spellStatSpriteCD;
    [SerializeField]
    private Sprite spellStatSpriteDot;
    [SerializeField]
    private Sprite spellStatSpriteAoe;


    public void SetType(SpellStatType statType)
    {
        string tooltip = "";
        if (statType == SpellStatType.Damage)
        {
            imgIcon.sprite = spellStatSpriteDamage;
            tooltip = "Damage level";
        }
        else if (statType == SpellStatType.CoolDown)
        {
            imgIcon.sprite = spellStatSpriteCD;
            tooltip = "Cooldown level";
        }
        else if (statType == SpellStatType.Dot)
        {
            imgIcon.sprite = spellStatSpriteDot;
            tooltip = "Damage over time level";
        }
        else if (statType == SpellStatType.Aoe)
        {
            imgIcon.sprite = spellStatSpriteAoe;
            tooltip = "AoE level";
        }
        if (showToolTip) {
            GetComponent<UITooltipTrigger>().Initialize(tooltip);
        }
    }


    [SerializeField]
    private bool showToolTip = false;

    public void SetLevel(int newLevel)
    {
        txtLevel.text = $"{newLevel + 1}";
        imgLevelBg.color = Configs.main.GradientConfig.GetColor(newLevel);
    }

    public void GrayOut(Color bg, Color fg)
    {
        txtLevel.color = fg;
        imgLevelBg.color = bg;
        innerBg.color = bg;
        txtLevel.text = "-";
        /*imgIconBg.color = fg;
        imgIcon.color = fg;*/
        imgIcon.enabled = false;
        imgIconBg.enabled = false;
        GetComponent<UITooltipTrigger>().Hide();
    }

}

public enum SpellStatType
{
    None,
    Damage,
    CoolDown,
    Dot,
    Aoe
}