using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHistoryView : MonoBehaviour
{
    [SerializeField]
    private GameObject historyViewContainer;

    [SerializeField]
    private Transform singleRunContainer;
    [SerializeField]
    private Text generalInfo;

    void Start()
    {
        Close();
    }

    public void Close()
    {
        historyViewContainer.SetActive(false);
    }

    public void Open()
    {
        List<RunHistory> runHistories = RunHistoryDb.LoadAll();
        if (runHistories.Count == 0)
        {
            Debug.Log("No run histories found, don't show history view!");
            generalInfo.text = "No attempts found.";
        }
        else
        {
            historyViewContainer.SetActive(true);
        }
        runHistories.Reverse();
        int victories = 0;
        int defeats = 0;
        RunHistory fastestRun = null;
        RunHistory closest = null;
        foreach (RunHistory runHistory in runHistories)
        {
            UIHistorySingleRun runUI = Prefabs.Get<UIHistorySingleRun>();
            if (runHistory.RunSuccesful)
            {
                victories++;
            }
            else
            {
                defeats++;
            }
            if (fastestRun == null || runHistory.RunTimeAsNumber < fastestRun.RunTimeAsNumber)
            {
                fastestRun = runHistory;
            }
            if (closest == null || runHistory.LevelReached > fastestRun.LevelReached)
            {
                closest = runHistory;
            }
            runUI.transform.SetParent(singleRunContainer);
            runUI.Initialize(runHistory);
        }
        if (victories > 0)
        {
            generalInfo.text = $"{runHistories.Count} runs, {victories} victories, {defeats} defeats. Fastest run: {fastestRun.RunTime} on {fastestRun.FormattedStartDate}.";
        }
        else
        {
            if (runHistories.Count == 1) {
                generalInfo.text = $"A single attempt has been made: level {closest.LevelReached} on {closest.FormattedStartDate}.";
            }
            else if (closest != null)
            {
                generalInfo.text = $"{runHistories.Count} attempts made. Furthest attempt: level {closest.LevelReached} on {closest.FormattedStartDate}.";
            }
        }
    }
}
