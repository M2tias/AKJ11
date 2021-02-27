using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Experience : MonoBehaviour
{
    private int level;
    private int currentExp;
    private int statPointsToUse;
    [SerializeField]
    private ExperienceConfig expConfig;

    void Start()
    {
        level = 0;
        currentExp = 0;
        statPointsToUse = 0;
        statPointsToUse += expConfig.StatPointsPerLevel[level]; // initial stat points
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
    }

    public int GetUnusedStatPoints()
    {
        return statPointsToUse;
    }
}
