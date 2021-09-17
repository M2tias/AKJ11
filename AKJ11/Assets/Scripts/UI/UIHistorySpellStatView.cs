using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHistorySpellStatView : MonoBehaviour
{
    [SerializeField]
    private Text txtUseCount;
    [SerializeField]
    private Text txtKills;
    [SerializeField]
    private Text txtDamageDone;

    [SerializeField]
    private Image imgSpellIcon;

    [SerializeField]
    private UISpellLevel spellLevel1;
    [SerializeField]
    private UISpellLevel spellLevel2;
    [SerializeField]
    private UISpellLevel spellLevel3;
    [SerializeField]
    private UISpellLevel spellLevel4;

    [SerializeField]
    private SpellBaseConfig magicMissileSpell;
    [SerializeField]
    private SpellBaseConfig fireballSpell;
    [SerializeField]
    private SpellBaseConfig wallSpell;

    [SerializeField]
    private UIHistorySpellSingleStat killsStat;
    [SerializeField]
    private UIHistorySpellSingleStat damageDoneStat;
    [SerializeField]
    private UIHistorySpellSingleStat usesStat;
    [SerializeField]
    private UISpellLevel damageStat;
    [SerializeField]
    private UISpellLevel dotStat;
    [SerializeField]
    private UISpellLevel aoeStat;

    [SerializeField]
    private Color grayOutColorBg;
    [SerializeField]
    private Color grayOutColorFg;

    public void Initialize(SpellHistory spellHistory)
    {
        ShowSpellStats(spellHistory);
        ShowSpellIcons(spellHistory);
        ShowSpellLevels(spellHistory);
        spellLevel1.SetType(SpellStatType.Damage);
        spellLevel2.SetType(SpellStatType.CoolDown);
        spellLevel3.SetType(SpellStatType.Dot);
        spellLevel4.SetType(SpellStatType.Aoe);
        killsStat.Initialize("Kills");
        damageDoneStat.Initialize("Damage done");
        usesStat.Initialize("Times used");
        HideUnneeded(spellHistory);
    }

    private void HideUnneeded(SpellHistory spellHistory) {
        if (spellHistory.SpellType == SpellType.Wall) {
            killsStat.GrayOut(grayOutColorBg, grayOutColorFg);
            damageDoneStat.GrayOut(grayOutColorBg, grayOutColorFg);
            damageStat.GrayOut(grayOutColorBg, grayOutColorFg);
            dotStat.GrayOut(grayOutColorBg, grayOutColorFg);
            aoeStat.GrayOut(grayOutColorBg, grayOutColorFg);
        }
    }

    private void ShowSpellStats(SpellHistory spellHistory)
    {
        txtUseCount.text = spellHistory.RunSpellStats.TimesUsed.ToString();
        txtKills.text = spellHistory.RunSpellStats.EnemiesKilled.ToString();
        txtDamageDone.text = Mathf.RoundToInt(spellHistory.RunSpellStats.DamageDone).ToString();
    }

    private void ShowSpellIcons(SpellHistory spellHistory)
    {
        if (spellHistory.SpellType == SpellType.MagicMissile)
        {
            imgSpellIcon.sprite = magicMissileSpell.ProjectileSprite;
            imgSpellIcon.transform.localScale = Vector2.one * 1.4f;
        }
        else if (spellHistory.SpellType == SpellType.FireBall)
        {
            imgSpellIcon.sprite = fireballSpell.ProjectileSprite;
            imgSpellIcon.transform.localScale = Vector2.one * 1.4f;
        }
        else if (spellHistory.SpellType == SpellType.Wall)
        {
            imgSpellIcon.sprite = wallSpell.ProjectileSprite;
            imgSpellIcon.transform.localScale = Vector2.one * 0.8f;
        }
    }
    private void ShowSpellLevels(SpellHistory spellHistory)
    {
        spellLevel1.SetLevel(spellHistory.SpellLevelStats.DamageLevel);
        spellLevel2.SetLevel(spellHistory.SpellLevelStats.CooldownLevel);
        spellLevel3.SetLevel(spellHistory.SpellLevelStats.DotLevel);
        spellLevel4.SetLevel(spellHistory.SpellLevelStats.BounceLevel);
    }
}
