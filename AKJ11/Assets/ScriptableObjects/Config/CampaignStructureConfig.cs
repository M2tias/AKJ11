
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

[CreateAssetMenu(fileName = "CampaignStructureConfig", menuName = "Configs/CampaignStructureConfig")]
public class CampaignStructureConfig : ScriptableObject
{

    [field: SerializeField]
    public MapConfig IntroLevel {get; private set;}
    [field: SerializeField]
    public MapConfig TheEndLevel {get; private set;}

    [field: SerializeField]
    public List<CampaignStage> Stages {get; private set;}

    [NonSerialized]
    private List<MapConfig> levels = null;

    private void GroupLevels() {
        levels = new List<MapConfig>();
        AddLevel(IntroLevel);
        foreach(CampaignStage stage in Stages) {
            foreach(MapConfig level in stage.Levels) {
                AddLevel(level);
            }
            AddLevel(stage.BossLevel);
            AddLevel(stage.AfterBossLevel);
        }
        AddLevel(TheEndLevel);
    }

    private void AddLevel(MapConfig level) {
        if (level == null) {
            return;
        }
        levels.Add(level);
    }

    public bool IsLastLevel(MapConfig mapConfig) {
        return mapConfig == TheEndLevel;
    }

    public MapConfig Get(int currentLevel) {
        if (levels == null) {
            GroupLevels();
        }
        if (currentLevel >= levels.Count) {
            return null;
        }
        return levels[currentLevel];
    }

}


[System.Serializable]
public class CampaignStage {

    [field: SerializeField]
    public string Name {get; private set;}

    [field: SerializeField]
    public MapConfig BossLevel {get; set;}

    [field: SerializeField]
    public MapConfig AfterBossLevel {get; set;}

    [field: SerializeField]
    public List<MapConfig> Levels {get; private set;}
}