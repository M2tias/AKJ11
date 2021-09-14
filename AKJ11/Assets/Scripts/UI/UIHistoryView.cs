using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIHistoryView : MonoBehaviour
{
    [SerializeField]
    private GameObject historyViewContainer;

    [SerializeField]
    private Transform singleRunContainer;

    void Start() {
        List<RunHistory> runHistories = RunHistoryDb.LoadAll();
        if (runHistories.Count == 0) {
            Debug.Log("No run histories found, don't show history view!");
        } else {
            historyViewContainer.SetActive(true);
        }
        runHistories.Reverse();
        foreach(RunHistory runHistory in runHistories) {
            UIHistorySingleRun runUI = Prefabs.Get<UIHistorySingleRun>();
            runUI.transform.SetParent(singleRunContainer);
            runUI.Initialize(runHistory);
        }
    }
}
