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

    void Awake() {
        main = this;
    }

    public void LevelStarted(MapConfig config, int levelIndex) {
        currentLevel = levelIndex;
        currentConfig = config;
        deadEntities = new List<GameEntity>();
        wokeEntities = new List<GameEntity>();
        Debug.Log($"Level {config.name} (number {levelIndex + 1}) started.");
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
}
