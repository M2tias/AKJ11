using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class RunHistoryDb
{

    private static RunHistory currentRun;
    public static bool RunStarted = false;
    public static void SaveCurrent(bool playerWin, Hurtable playerHurtable)
    {
        MonoBehaviour.print($"Saving current {currentRun.Name}!");
        try
        {
            currentRun.RunSuccesful = playerWin;
            currentRun.FormattedEndDate = System.DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss");
            currentRun.RunTime = GameStateManager.main.GetFormattedTime();
            currentRun.RunTimeAsNumber = GameStateManager.main.GetTime();
            currentRun.Health = (int)playerHurtable.Health;
            currentRun.LevelReached = MapGenerator.main.CurrentLevel;
            currentRun.ExperiencePoints = Experience.main.TotalExpGained;
            currentRun.DamageTaken = (int)playerHurtable.DamageTaken;
            currentRun.MagicMissileHistory.CreateSpellLevelStats();
            currentRun.FireBallHistory.CreateSpellLevelStats();
            currentRun.WallHistory.CreateSpellLevelStats();
            string runJson = JsonUtility.ToJson(currentRun);
            RunHistoryKeys runHistoryKeys = LoadHistoryKeys();
            PlayerPrefs.SetString(currentRun.Name, runJson);
            runHistoryKeys.Keys.Add(currentRun.Name);
            PlayerPrefs.SetString("RunHistoryKeys", JsonUtility.ToJson(runHistoryKeys));
        }
        catch (System.Exception e)
        {
            UnityEngine.MonoBehaviour.print(e);
        }
        MonoBehaviour.print($"Saving succesful!");
    }

    private static RunHistoryKeys LoadHistoryKeys()
    {
        string runHistoryKeyString = PlayerPrefs.GetString("RunHistoryKeys", "");
        if (runHistoryKeyString != "")
        {
            return JsonUtility.FromJson<RunHistoryKeys>(runHistoryKeyString);
        }
        RunHistoryKeys runHistoryKeys = new RunHistoryKeys();
        return runHistoryKeys;
    }

    public static List<RunHistory> LoadAll()
    {
        RunHistoryKeys runHistoryKeys = LoadHistoryKeys();
        List<RunHistory> pastRuns = new List<RunHistory>();
        foreach (string key in runHistoryKeys.Keys)
        {
            string runName = PlayerPrefs.GetString(key, "");
            if (runName != "")
            {
                pastRuns.Add(JsonUtility.FromJson<RunHistory>(runName));
            }
        }
        return pastRuns;
    }

    public static void StartRun()
    {
        RunStarted = true;
        RandomNumberGenerator rng = RandomNumberGenerator.GetInstance();
        currentRun = new RunHistory(rng.Seed);
    }

    public static void AddPotion()
    {
        if (currentRun == null)
        {
            return;
        }
        currentRun.PotionsPickedUp++;
        MonoBehaviour.print($"Potion picked up (total: {currentRun.PotionsPickedUp}).");
    }


    public static void AddXpBookPickup()
    {
        if (currentRun == null)
        {
            return;
        }

        currentRun.XPBooksPickedUp++;
        MonoBehaviour.print($"Xp book picked up (total: {currentRun.XPBooksPickedUp}).");
    }

    public static void AddSkillPoints(int value)
    {
        if (currentRun == null)
        {
            return;
        }

        currentRun.SkillPointsDistributed += value;
        MonoBehaviour.print($"Skill points distributed: {value} (total {currentRun.SkillPointsDistributed}).");
    }

    public static void AddHealthGain(int value)
    {
        if (currentRun == null)
        {
            return;
        }

        currentRun.HealthGained += value;
        MonoBehaviour.print($"Health gained: {value} (total {currentRun.HealthGained}).");
    }


    public static void AddSpellUse(SpellType spellType)
    {
        if (spellType == SpellType.MagicMissile)
        {
            currentRun.MagicMissileHistory.AddUse();
        }
        if (spellType == SpellType.FireBall)
        {
            currentRun.FireBallHistory.AddUse();
        }
        if (spellType == SpellType.Wall)
        {
            currentRun.WallHistory.AddUse();
        }
        Debug.Log($"Spell [{spellType}] was used.");
    }
    public static void AddSpellKill(SpellType spellType)
    {
        if (spellType == SpellType.MagicMissile)
        {
            currentRun.MagicMissileHistory.AddKill();
        }
        if (spellType == SpellType.FireBall)
        {
            currentRun.FireBallHistory.AddKill();
        }
        Debug.Log($"An enemy was killed by spell [{spellType}].");
    }
    public static void AddSpellDamage(SpellType spellType, float value)
    {
        if (spellType == SpellType.MagicMissile)
        {
            currentRun.MagicMissileHistory.AddDamage(value);
        }
        if (spellType == SpellType.FireBall)
        {
            currentRun.FireBallHistory.AddDamage(value);
        }
        Debug.Log($"Spell [{spellType}] dealt {value} damage.");
    }
}

[System.Serializable]
public class RunHistoryKeys
{
    public RunHistoryKeys()
    {
        Keys = new List<string>();
    }
    public List<string> Keys;
}


[System.Serializable]
public class RunHistory
{
    public RunHistory(string seed)
    {
        Seed = seed;
        FormattedStartDate = System.DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss");
        Name = $"{seed}-{FormattedStartDate}";
        MagicMissileHistory = new SpellHistory(SpellType.MagicMissile, Experience.main.GetSpellLevelRuntime(SpellType.MagicMissile));
        FireBallHistory = new SpellHistory(SpellType.FireBall, Experience.main.GetSpellLevelRuntime(SpellType.FireBall));
        WallHistory = new SpellHistory(SpellType.Wall, Experience.main.GetSpellLevelRuntime(SpellType.Wall));
    }
    public string Name;
    public SpellHistory MagicMissileHistory;
    public SpellHistory FireBallHistory;
    public SpellHistory WallHistory;
    public string FormattedStartDate;
    public string FormattedEndDate;
    public string Seed;
    public string RunTime;
    public double RunTimeAsNumber;
    public bool RunSuccesful = false;
    public int Health;
    public int LevelReached;
    public int ExperiencePoints;
    public int SkillPointsDistributed;
    public int DamageTaken;
    public int PotionsPickedUp;
    public int XPBooksPickedUp;
    public int HealthGained;
}

[System.Serializable]
public class SpellHistory
{
    private SpellLevelRuntime runtime;

    public SpellHistory(SpellType spellType, SpellLevelRuntime spellLevelRunTime)
    {
        runtime = spellLevelRunTime;
        SpellType = spellType;
        RunSpellStats = new RunSpellStats();
    }

    public void CreateSpellLevelStats()
    {
        SpellLevelStats = new SpellLevelStats(runtime, SpellType);
    }

    public void AddUse()
    {
        RunSpellStats.TimesUsed++;
    }
    public void AddKill()
    {
        RunSpellStats.EnemiesKilled++;
    }
    public void AddDamage(float value)
    {
        RunSpellStats.DamageDone += value;
    }

    public SpellType SpellType;
    public SpellLevelStats SpellLevelStats;
    public RunSpellStats RunSpellStats;
}

[System.Serializable]
public class SpellLevelStats
{
    public SpellLevelStats(SpellLevelRuntime spellLevelRunTime, SpellType spellType)
    {
        CooldownLevel = spellLevelRunTime.CooldownLevel;
        if (spellType == SpellType.Wall)
        {
            return;
        }
        DamageLevel = spellLevelRunTime.DamageLevel;
        DotLevel = spellLevelRunTime.DotLevel;
        if (spellType == SpellType.FireBall)
        {
            BounceLevel = spellLevelRunTime.AoeLevel;
        }
        else if (spellType == SpellType.MagicMissile)
        {
            BounceLevel = spellLevelRunTime.BouncesLevel;
        }
    }
    public int DamageLevel;
    public int CooldownLevel;
    public int DotLevel;
    public int BounceLevel;
}

[System.Serializable]
public class RunSpellStats
{
    public RunSpellStats()
    {

    }
    public float DamageDone;
    public int TimesUsed;
    public int EnemiesKilled;
}