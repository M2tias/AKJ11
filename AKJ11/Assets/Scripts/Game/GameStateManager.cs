using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager main;

    private List<GameEntity> wokeEntities = new List<GameEntity>();
    private List<GameEntity> deadEntities = new List<GameEntity>();

    private MapConfig currentConfig;
    private int currentLevel;

    private Timer timer;

    private MusicPlayer musicPlayer;

    void Awake() {
        main = this;
        musicPlayer = MusicPlayer.GetInstance();
    }

    public void LevelStarted(MapConfig config, int levelIndex) {
        if (config.MusicConfig != null) {
            musicPlayer.SetConfig(config.MusicConfig);
        }
        if (!musicPlayer.HasConfig) {
            musicPlayer.SetConfig(Configs.main.DefaultMusic);
        }
        if (timer == null) {
            timer = new Timer();
            if (UITimer.main != null) {
                UITimer.main.timer = timer;
            }
        } else {
            if (Configs.main.Campaign.IsLastLevel(config)) {
               StopTime();
            }
        }
        currentLevel = levelIndex;
        currentConfig = config;
        deadEntities = new List<GameEntity>();
        wokeEntities = new List<GameEntity>();
        Debug.Log($"Started level '{config.name}' (#{levelIndex}).");
    }

    public string GetFormattedTime() {
        if (timer == null) {
            return "-";
        }
        return timer.GetString();
    }

    public void StopTime() {
        if (timer != null) {
            timer.Pause();
        }
        Time.timeScale = 0f;
    }

    public void StartTime() {
        if (timer != null && !Configs.main.Campaign.IsLastLevel(currentConfig)) {
            timer.Unpause();
        }
        Time.timeScale = 1f;
    }

    public void LevelEnded() {
        Debug.Log($"Level {currentConfig.name} (number {currentLevel + 1}) ended.");
    }

    public void EntityWokeUp(GameEntity entity) {
        wokeEntities.Add(entity);
        Debug.Log($"Entity {entity} woke up.");
    }

    public void EntityDied(GameEntity deadEntity) {
        Debug.Log($"Entity {deadEntity} died.");
        deadEntities.Add(deadEntity);
        wokeEntities.Remove(deadEntity);
        if (currentConfig.KeySpawn == KeySpawn.LastEntityDrops && wokeEntities.Count == 0) {
            RandomNumberGenerator rng = RandomNumberGenerator.GetInstance();
            List<MapNode> possibleEntranceNodes = MapGenerator.main.GetNodeContainer().Nodes
                .Where(node => !node.IsWall && !node.IsEdge && IsBetweenRange(node.Distance(deadEntity.Node), 3, 5))
                .ToList();
            MapPopulator.PlaceNextLevelTrigger(possibleEntranceNodes[rng.Range(0, possibleEntranceNodes.Count)]);
            MapPopulator.PlaceKey(deadEntity.Node);
        }
    }

    private static bool IsBetweenRange(float value, float min, float max)
    {
        return value >= Mathf.Min(min, max) && value <= Mathf.Max(min, max);
    }

    public void SpawnKey(Vector2 position)
    {
        MapPopulator.PlaceKey(position);
    }

    private void Update() {
        musicPlayer.Update();
    }
}
