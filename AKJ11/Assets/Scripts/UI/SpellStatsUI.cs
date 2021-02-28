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
        //DisableStatButtons();
    }

    public void DamageButtonAction()
    {
        if (spellConfig.Damage.Count == spellRuntime.DamageLevel + DamagePoints + 1)
        {
            DisableStatButtons();
            DamageButton.SetActive(false);
            return;
        }
        DamagePoints++;
        Experience.main.AddPendingStatPoint(1);
        SetTextValue(DamageText, spellConfig.Damage[spellRuntime.DamageLevel + DamagePoints].ToString());
        DisableStatButtons();
    }
    public void AoeButtonAction()
    {
        if (spellConfig.Aoe.Count == spellRuntime.AoeLevel + AoePoints + 1)
        {
            DisableStatButtons();
            AoeButton.SetActive(false);
            return;
        }
        AoePoints++;
        Experience.main.AddPendingStatPoint(1);
        SetTextValue(AoeText, spellConfig.Aoe[spellRuntime.AoeLevel + AoePoints].ToString());
        DisableStatButtons();
    }
    public void BouncesButtonAction()
    {
        if (spellConfig.Bounces.Count == spellRuntime.BouncesLevel + BouncesPoints + 1)
        {
            DisableStatButtons();
            BouncesButton.SetActive(false);
            return;
        }
        BouncesPoints++;
        Experience.main.AddPendingStatPoint(1);
        SetTextValue(BouncesText, spellConfig.Bounces[spellRuntime.BouncesLevel + BouncesPoints].ToString());
        DisableStatButtons();
    }
    public void DotButtonAction()
    {
        if (spellConfig.Dot.Count == spellRuntime.DotLevel + DotPoints + 1)
        {
            DisableStatButtons();
            DotButton.SetActive(false);
            return;
        }
        DotPoints++;
        DotTickDamagePoints++;
        Experience.main.AddPendingStatPoint(1);
        SetTextValue(DotText, spellConfig.Dot[spellRuntime.DotLevel + DotPoints].ToString());
        DisableStatButtons();
    }
    //public void DotTickDamaButtonAction()
    //{
    //    DotTickDamagePoints++;
    //    Experience.main.AddPendingStatPoint(1);
    //    SetTextValue(DotTickDamageText, spellConfig.DotTickDamage[spellRuntime.DotTickDamageLevel + DotTickDamagePoints].ToString());
    //    DisableStatButtons();
    //}
    public void PiercingButtonAction()
    {
        // NOT USED!
        return;
        PiercingPoints++;
        Experience.main.AddPendingStatPoint(1);
        SetTextValue(PiercingText, spellConfig.Piercing[spellRuntime.PiercingLevel + PiercingPoints].ToString());
        DisableStatButtons();
    }
    public void CooldownButtonAction()
    {
        if (spellConfig.Cooldown.Count == spellRuntime.CooldownLevel + CooldownPoints + 1)
        {
            DisableStatButtons();
            CooldownButton.SetActive(false);
            return;
        }
        CooldownPoints++;
        Experience.main.AddPendingStatPoint(1);
        SetTextValue(CooldownText, spellConfig.Cooldown[spellRuntime.CooldownLevel + CooldownPoints].ToString());
        DisableStatButtons();
    }
    public void SpeedButtonAction()
    {
        if (spellConfig.Speed.Count == spellRuntime.SpeedLevel + SpeedPoints + 1)
        {
            DisableStatButtons();
            SpeedButton.SetActive(false);
            return;
        }
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
        if (DamageButton != null)
        {
            if (spellConfig.Damage.Count == spellRuntime.DamageLevel + 1)
            {
                DamageButton.SetActive(false);
            }
            else if (unusedPoints <= usedPoints)
            {
                DamageButton.SetActive(false);
            }
            else
            {
                DamageButton.SetActive(true);
            }
        }
        if (AoeButton != null)
        {
            if (spellConfig.Aoe.Count == spellRuntime.AoeLevel + 1)
            {
                AoeButton.SetActive(false);
            }
            else if (unusedPoints <= usedPoints)
            {
                AoeButton.SetActive(false);
            }
            else
            {
                AoeButton.SetActive(true);
            }
        }
        if (BouncesButton != null)
        {
            if (spellConfig.Bounces.Count == spellRuntime.BouncesLevel + 1)
            {
                BouncesButton.SetActive(false);
            }
            else if (unusedPoints <= usedPoints)
            {
                BouncesButton.SetActive(false);
            }
            else
            {
                BouncesButton.SetActive(true);
            }
        }
        if (DotButton != null)
        {
            if (spellConfig.Dot.Count == spellRuntime.DotLevel + 1)
            {
                DotButton.SetActive(false);
            }
            else if (unusedPoints <= usedPoints)
            {
                DotButton.SetActive(false);
            }
            else
            {
                DotButton.SetActive(true);
            }
        }
        if (DotTickDamageButton != null)
        {
            if (spellConfig.DotTickDamage.Count == spellRuntime.DotTickDamageLevel + 1)
            {
                DotTickDamageButton.SetActive(false);
            }
            else if (unusedPoints <= usedPoints)
            {
                DotTickDamageButton.SetActive(false);
            }
            else
            {
                DotTickDamageButton.SetActive(true);
            }
        }
        if (PiercingButton != null)
        {
            if (spellConfig.Piercing.Count == spellRuntime.PiercingLevel + 1)
            {
                PiercingButton.SetActive(false);
            }
            else if (unusedPoints <= usedPoints)
            {
                PiercingButton.SetActive(false);
            }
            else
            {
                PiercingButton.SetActive(true);
            }
        }
        if (CooldownButton != null)
        {
            if (spellConfig.Cooldown.Count == spellRuntime.CooldownLevel + 1)
            {
                CooldownButton.SetActive(false);
            }
            else if (unusedPoints <= usedPoints)
            {
                CooldownButton.SetActive(false);
            }
            else
            {
                CooldownButton.SetActive(true);
            }
        }
        if (SpeedButton != null)
        {
            if (spellConfig.Speed.Count == spellRuntime.SpeedLevel + 1)
            {
                SpeedButton.SetActive(false);
            }
            else if (unusedPoints <= usedPoints)
            {
                SpeedButton.SetActive(false);
            }
            else
            {
                SpeedButton.SetActive(true);
            }
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