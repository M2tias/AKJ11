using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHistorySingleRun : MonoBehaviour
{
    [SerializeField]
    private Transform spellContainer;

    [SerializeField]
    private Transform longStatContainer;

    [SerializeField]
    private Transform singleStatContainer;

    [SerializeField]
    private Text txtTitle;

    [SerializeField]
    private Image imgSuccessIndicator;
    [SerializeField]
    private Sprite victorySprite;
    [SerializeField]
    private Sprite lossSprite;

    public void Initialize(RunHistory runHistory)
    {
        imgSuccessIndicator.sprite = runHistory.RunSuccesful ? victorySprite : lossSprite;
        imgSuccessIndicator.GetComponent<UITooltipTrigger>().Initialize(runHistory.RunSuccesful ? "Victory" : "Defeat");
        txtTitle.text = runHistory.FormattedStartDate;
        ShowSpellStats(runHistory);
        ShowLongStats(runHistory);
        ShowSingleStats(runHistory);
    }

    [SerializeField]
    private Sprite runtimeIcon;
    [SerializeField]
    private Sprite seedIcon;

    private void ShowLongStats(RunHistory runHistory) {
        UIHistoryLongStat runtimeStat = Prefabs.Get<UIHistoryLongStat>();
        runtimeStat.transform.SetParent(longStatContainer);
        runtimeStat.Initialize(runtimeIcon, runHistory.RunTime, "Run time");
        UIHistoryLongStat seedStat = Prefabs.Get<UIHistoryLongStat>();
        seedStat.transform.SetParent(longStatContainer);
        seedStat.Initialize(seedIcon, runHistory.Seed, "Generation Seed (click to set as seed)", delegate{
            MainMenu.main.SetSeedText(runHistory.Seed);
        });
    }

    private void ShowSpellStats(RunHistory runHistory)
    {
        UIHistorySpellStatView magicMissile = Prefabs.Get<UIHistorySpellStatView>();
        UIHistorySpellStatView fireball = Prefabs.Get<UIHistorySpellStatView>();
        UIHistorySpellStatView wall = Prefabs.Get<UIHistorySpellStatView>();
        magicMissile.Initialize(runHistory.MagicMissileHistory);
        magicMissile.transform.SetParent(spellContainer);
        fireball.Initialize(runHistory.FireBallHistory);
        fireball.transform.SetParent(spellContainer);
        wall.Initialize(runHistory.WallHistory);
        wall.transform.SetParent(spellContainer);
        wall.transform.SetParent(spellContainer);
    }

    private void ShowSingleStats(RunHistory runHistory)
    {
        AddSingleStat(healthIcon, runHistory.Health, "Health");
        AddSingleStat(levelIcon, runHistory.LevelReached, "Level reached");
        AddSingleStat(skillIcon, runHistory.SkillPointsDistributed, "Skill points distributed");
        AddSingleStat(xpIcon, runHistory.ExperiencePoints, "Total experience points");
        AddSingleStat(xpBookIcon, runHistory.XPBooksPickedUp, "Experience books picked up");
        AddSingleStat(potionIcon, runHistory.PotionsPickedUp, "Potions picked up");
        AddSingleStat(hpGainedIcon, runHistory.HealthGained, "Health healed");
        AddSingleStat(dmgTakenIcon, runHistory.DamageTaken, "Damage taken");
    }

    [SerializeField]
    private Sprite healthIcon;
    [SerializeField]
    private Sprite levelIcon;
    [SerializeField]
    private Sprite skillIcon;
    [SerializeField]
    private Sprite xpIcon;
    [SerializeField]
    private Sprite xpBookIcon;
    [SerializeField]
    private Sprite potionIcon;
    [SerializeField]
    private Sprite hpGainedIcon;
    [SerializeField]
    private Sprite dmgTakenIcon;

    private void AddSingleStat(Sprite icon, int value, string tooltip)
    {
        AddSingleStat(icon, value.ToString(), tooltip);
    }
    private void AddSingleStat(Sprite icon, string value, string tooltip)
    {
        UIHistorySingleStat health = Prefabs.Get<UIHistorySingleStat>();
        health.Initialize(icon, value, tooltip);
        health.transform.SetParent(singleStatContainer);
    }
}
