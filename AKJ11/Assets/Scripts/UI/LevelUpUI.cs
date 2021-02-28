using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelUpUI : MonoBehaviour
{
    [SerializeField]
    private Text totalPointsUsed;
    [SerializeField]
    private Text pointsUsedNow;
    [SerializeField]
    private Text unassignedPoints;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        pointsUsedNow.text = Experience.main.GetPendingStatPoints().ToString();
        int total = Experience.main.GetPendingStatPoints() + Experience.main.GetTotalPointsUsed();
        totalPointsUsed.text = total.ToString();
        int unused = Experience.main.GetUnusedStatPoints() - Experience.main.GetPendingStatPoints();
        unassignedPoints.text = unused.ToString();
    }
}
