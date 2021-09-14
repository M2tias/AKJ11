using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHistorySpellStatView : MonoBehaviour
{
    [SerializeField]
    private Text txtUseCount;
    [SerializeField]
    private Text txtKills;
    [SerializeField]
    private Text txtDamageDone;

    [SerializeField]
    private UISpellLevel spellLevel1;
    [SerializeField]
    private UISpellLevel spellLevel2;
    [SerializeField]
    private UISpellLevel spellLevel3;
    [SerializeField]
    private UISpellLevel spellLevel4;

    public void Initialize(SpellHistory spellHistory) {
        // int uses, int kills, float dmg, int spell1, int spell2, int spell3, int spell4
        txtUseCount.text = spellHistory.RunSpellStats.TimesUsed.ToString();
        txtKills.text = spellHistory.RunSpellStats.EnemiesKilled.ToString();
        txtDamageDone.text = Mathf.RoundToInt(spellHistory.RunSpellStats.DamageDone).ToString();
        spellLevel1.SetLevel(spellHistory.SpellLevelStats.DamageLevel);
        spellLevel2.SetLevel(spellHistory.SpellLevelStats.CooldownLevel);
        spellLevel3.SetLevel(spellHistory.SpellLevelStats.DotLevel);
        spellLevel4.SetLevel(spellHistory.SpellLevelStats.BounceLevel);
    }
}
