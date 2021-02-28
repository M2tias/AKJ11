using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Experience : MonoBehaviour
{
    public static Experience main;

    private int level;
    private int currentExp;
    private int statPointsToUse;
    private int pendingStatsPoints;
    private int totalPointsUsed;
    [SerializeField]
    private ExperienceConfig expConfig;
    [SerializeField]
    private SpellLevelRuntime spell1Runtime;
    [SerializeField]
    private SpellLevelRuntime spell2Runtime;
    [SerializeField]
    private SpellLevelRuntime spellWallRuntime;
    [SerializeField]
    private SpellBaseConfig spell1Config;
    [SerializeField]
    private SpellBaseConfig spell2Config;
    [SerializeField]
    private SpellBaseConfig spellWallConfig;

    private void Awake()
    {
        main = this;
    }

    void Start()
    {
        level = 0;
        currentExp = 1000;
        statPointsToUse = 0;
        totalPointsUsed = 0;
        statPointsToUse += expConfig.StatPointsPerLevel[level]; // initial stat points
        spell1Runtime.Initialize();
        spell2Runtime.Initialize();
        spellWallRuntime.Initialize();
        spell1Runtime.IsUnlocked = spell1Config.IsUnlocked;
        spell2Runtime.IsUnlocked = spell2Config.IsUnlocked;
        spellWallRuntime.IsUnlocked = spellWallConfig.IsUnlocked;
        if (UIXP.main != null) {
            UIXP.main.SetXP(0, 0, 0);
        }
    }

    void Update()
    {

    }

    public void AddExperience(int exp)
    {
        currentExp += exp;
  
        while (currentExp >= expConfig.ExpPerLevel[level])
        {
            int remainder = currentExp - expConfig.ExpPerLevel[level];
            currentExp = remainder;
            level++;
            statPointsToUse += expConfig.StatPointsPerLevel[level];
        }
        Debug.Log("Gained " + exp + " points of experience. Current level: " + level + " and remaining exp: " + currentExp);
        if (UIXP.main != null) {
            UIXP.main.SetXP(currentExp, expConfig.ExpPerLevel[level], level);
        }
    }

    public float GetCooldown(SpellBaseConfig spell) {
        if (spell == spell1Config) {
            return spell.Cooldown[spell1Runtime.CooldownLevel];
        }
        else if (spell == spell2Config)
        {
            return spell.Cooldown[spell2Runtime.CooldownLevel];
        }
        else if (spell == spellWallConfig)
        {
            return spell.Cooldown[spellWallRuntime.CooldownLevel];
        }
        return -1;
    }

    public int GetUnusedStatPoints()
    {
        return statPointsToUse;
    }

    public void UseStatPoints(int amount)
    {
        statPointsToUse -= amount;
        totalPointsUsed += amount;
    }

    public void AddPendingStatPoint(int amount)
    {
        pendingStatsPoints += amount;
    }

    public void ResetPendingStatPoints()
    {
        pendingStatsPoints = 0;
    }

    public void CommitPendingStatPoints()
    {
        statPointsToUse -= pendingStatsPoints;
        totalPointsUsed += pendingStatsPoints;
        pendingStatsPoints = 0;
    }

    public int GetPendingStatPoints()
    {
        return pendingStatsPoints;
    }

    public int GetTotalPointsUsed()
    {
        return totalPointsUsed;
    }

    public int GetLevel()
    {
        return level;
    }

    public SpellLevelRuntime GetSpell1Runtime()
    {
        return spell1Runtime;
    }

    public SpellLevelRuntime GetSpell2Runtime()
    {
        return spell2Runtime;
    }

    public SpellLevelRuntime GetSpellWallRuntime()
    {
        return spellWallRuntime;
    }
}
