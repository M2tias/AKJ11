using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpellStatsUI : MonoBehaviour
{
    // keep values to undo
    private int DamagePoints = 0;
    private int AoePoints = 0;
    private int BouncesPoints = 0;
    private int DotPoints = 0;
    private int DotTickDamagePoints = 0;
    private int PiercingPoints = 0;
    private int CooldownPoints = 0;
    private int SpeedPoints = 0;

    private float DamageCurrentValue = 0;
    private float AoeCurrentValue = 0;
    private float BouncesCurrentValue = 0;
    private float DotCurrentValue = 0;
    private float DotTickDamageCurrentValue = 0;
    private float PiercingCurrentValue = 0;
    private float CooldownCurrentValue = 0;
    private float SpeedCurrentValue = 0;

    [SerializeField]
    private Text DamageText;
    [SerializeField]
    private Text AoeText;
    [SerializeField]
    private Text BouncesText;
    [SerializeField]
    private Text DotText;
    [SerializeField]
    private Text DotTickDamageText;
    [SerializeField]
    private Text PiercingText;
    [SerializeField]
    private Text CooldownText;
    [SerializeField]
    private Text SpeedText;

    [SerializeField]
    private GameObject DamageButton;
    [SerializeField]
    private GameObject AoeButton;
    [SerializeField]
    private GameObject BouncesButton;
    [SerializeField]
    private GameObject DotButton;
    [SerializeField]
    private GameObject DotTickDamageButton;
    [SerializeField]
    private GameObject PiercingButton;
    [SerializeField]
    private GameObject CooldownButton;
    [SerializeField]
    private GameObject SpeedButton;

    [SerializeField]
    SpellLevelRuntime spellRuntime;
    [SerializeField]
    SpellBaseConfig spellConfig;

    // Start is called before the first frame update
    void Start()
    {
        ResetTexts();
    }

    private void ResetTexts()
    {
        SetTextValue(DamageText, spellConfig.Damage[spellRuntime.DamageLevel].ToString());
        SetTextValue(AoeText, spellConfig.Aoe[spellRuntime.AoeLevel].ToString());
        SetTextValue(BouncesText, spellConfig.Bounces[spellRuntime.BouncesLevel].ToString());
        SetTextValue(DotText, spellConfig.Dot[spellRuntime.DotLevel].ToString());
        SetTextValue(DotTickDamageText, spellConfig.DotTickDamage[spellRuntime.DotTickDamageLevel].ToString());
        SetTextValue(PiercingText, spellConfig.Piercing[spellRuntime.PiercingLevel].ToString());
        SetTextValue(CooldownText, spellConfig.Cooldown[spellRuntime.CooldownLevel].ToString());
        SetTextValue(SpeedText, spellConfig.Speed[spellRuntime.SpeedLevel].ToString());
    }

    private void SetTextValue(Text t, string value)
    {
        if (t != null)
        {
            t.text = value;
        }
    }

    // Update is called once per frame
    void Update()
    {
        DisableStatButtons();
    }

    public void DamageButtonAction()
    {
        DamagePoints++;
        Experience.main.AddPendingStatPoint(1);
        SetTextValue(DamageText, spellConfig.Damage[spellRuntime.DamageLevel + DamagePoints].ToString());
        DisableStatButtons();
    }
    public void AoeButtonAction()
    {
        AoePoints++;
        Experience.main.AddPendingStatPoint(1);
        SetTextValue(AoeText, spellConfig.Aoe[spellRuntime.AoeLevel + AoePoints].ToString());
        DisableStatButtons();
    }
    public void BouncesButtonAction()
    {
        BouncesPoints++;
        Experience.main.AddPendingStatPoint(1);
        SetTextValue(BouncesText, spellConfig.Bounces[spellRuntime.BouncesLevel + BouncesPoints].ToString());
        DisableStatButtons();
    }
    public void DotButtonAction()
    {
        DotPoints++;
        Experience.main.AddPendingStatPoint(1);
        SetTextValue(DotText, spellConfig.Dot[spellRuntime.DotLevel + DotPoints].ToString());
        DisableStatButtons();
    }
    public void DotTickDamaButtonAction()
    {
        DotTickDamagePoints++;
        Experience.main.AddPendingStatPoint(1);
        SetTextValue(DotTickDamageText, spellConfig.DotTickDamage[spellRuntime.DotTickDamageLevel + DotTickDamagePoints].ToString());
        DisableStatButtons();
    }
    public void PiercingButtonAction()
    {
        PiercingPoints++;
        Experience.main.AddPendingStatPoint(1);
        SetTextValue(PiercingText, spellConfig.Piercing[spellRuntime.PiercingLevel + PiercingPoints].ToString());
        DisableStatButtons();
    }
    public void CooldownButtonAction()
    {
        CooldownPoints++;
        Experience.main.AddPendingStatPoint(1);
        SetTextValue(CooldownText, spellConfig.Cooldown[spellRuntime.CooldownLevel + CooldownPoints].ToString());
        DisableStatButtons();
    }
    public void SpeedButtonAction()
    {
        SpeedPoints++;
        Experience.main.AddPendingStatPoint(1);
        SetTextValue(SpeedText, spellConfig.Speed[spellRuntime.SpeedLevel + SpeedPoints].ToString());
        DisableStatButtons();
    }

    public void UndoButtonAction()
    {
        ResetValues();
        ResetTexts();
        Experience.main.ResetPendingStatPoints();
    }

    public void DoneButtonAction()
    {
        Experience.main.CommitPendingStatPoints();
        spellRuntime.DamageLevel += DamagePoints;
        spellRuntime.AoeLevel += AoePoints;
        spellRuntime.BouncesLevel += BouncesPoints;
        spellRuntime.DotLevel += DotPoints;
        spellRuntime.DotTickDamageLevel += DotTickDamagePoints;
        spellRuntime.PiercingLevel += PiercingPoints;
        spellRuntime.CooldownLevel += CooldownPoints;
        spellRuntime.SpeedLevel += SpeedPoints;
        ResetValues();
        ResetTexts();
    }


    private void DisableStatButtons()
    {
        int unusedPoints = Experience.main.GetUnusedStatPoints();
        int usedPoints = Experience.main.GetPendingStatPoints();

        if (unusedPoints <= usedPoints)
        {
            if (DamageButton != null) { DamageButton.SetActive(false); }
            if (AoeButton != null) { AoeButton.SetActive(false); }
            if (BouncesButton != null) { BouncesButton.SetActive(false); }
            if (DotButton != null) { DotButton.SetActive(false); }
            if (DotTickDamageButton != null) { DotTickDamageButton.SetActive(false); }
            if (PiercingButton != null) { PiercingButton.SetActive(false); }
            if (CooldownButton != null) { CooldownButton.SetActive(false); }
            if (SpeedButton != null) { SpeedButton.SetActive(false); }
        }
        else
        {
            if (DamageButton != null) { DamageButton.SetActive(true); }
            if (AoeButton != null) { AoeButton.SetActive(true); }
            if (BouncesButton != null) { BouncesButton.SetActive(true); }
            if (DotButton != null) { DotButton.SetActive(true); }
            if (DotTickDamageButton != null) { DotTickDamageButton.SetActive(true); }
            if (PiercingButton != null) { PiercingButton.SetActive(true); }
            if (CooldownButton != null) { CooldownButton.SetActive(true); }
            if (SpeedButton != null) { SpeedButton.SetActive(true); }
        }
    }

    private void ResetValues()
    {
        DamagePoints = 0;
        AoePoints = 0;
        BouncesPoints = 0;
        DotPoints = 0;
        DotTickDamagePoints = 0;
        PiercingPoints = 0;
        CooldownPoints = 0;
        SpeedPoints = 0;
    }

    private int GetPointsUsed()
    {
        int pointsUsed =
            DamagePoints + AoePoints + BouncesPoints
            + DotPoints + DotTickDamagePoints + PiercingPoints
            + CooldownPoints + SpeedPoints;
        return pointsUsed;
    }
}