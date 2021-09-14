using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIHistorySingleRun : MonoBehaviour
{
    [SerializeField]
    private Transform spellContainer;
    public void Initialize(RunHistory runHistory) {
        UIHistorySpellStatView magicMissile = Prefabs.Get<UIHistorySpellStatView>();
        UIHistorySpellStatView fireball = Prefabs.Get<UIHistorySpellStatView>();
        UIHistorySpellStatView wall = Prefabs.Get<UIHistorySpellStatView>();
        magicMissile.Initialize(runHistory.MagicMissileHistory);
        magicMissile.transform.SetParent(spellContainer);
        fireball.Initialize(runHistory.FireBallHistory);
        fireball.transform.SetParent(spellContainer);
        wall.Initialize(runHistory.WallHistory);
        wall.transform.SetParent(spellContainer);
    }
}
