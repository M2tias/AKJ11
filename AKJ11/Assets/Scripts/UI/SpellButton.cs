using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpellButton : MonoBehaviour
{
    [SerializeField]
    public SpellBaseConfig Spell;
    [SerializeField]
    private Image icon;
    [SerializeField]
    private Image background;
    [SerializeField]
    private Color cooldownColor;
    private Color originalColor;
    [SerializeField]
    private Image coolDownImage;
    [SerializeField]
    private Text txtName;
    [SerializeField]
    private UISpellLevel levelDamage;
    [SerializeField]
    private UISpellLevel levelCooldown;
    [SerializeField]
    private UISpellLevel levelDot;
    [SerializeField]
    private UISpellLevel levelBounces;

    private float cooldownTimer = 0;
    private bool onCooldown = false;

    private float coolDown = -1;

    private SpellLevelRuntime spellLevelRuntime;

    void Start() {
        Init();
    }
    public void Init() {
        txtName.text = Spell.Name;
        icon.sprite = Spell.ProjectileSprite;
        originalColor = background.color;
        spellLevelRuntime = Experience.main.GetSpellLevelRuntime(Spell.SpellType);
        levelDamage.SetType(SpellStatType.Damage);
        levelCooldown.SetType(SpellStatType.CoolDown);
        levelDot.SetType(SpellStatType.Dot);
        levelBounces.SetType(SpellStatType.Aoe);
    }

    void Update() {
        if (onCooldown) {
            cooldownTimer += Time.deltaTime;
            float amount = Mathf.Lerp(1, 0, cooldownTimer / coolDown);
            coolDownImage.fillAmount = amount;
            if (cooldownTimer >= coolDown) {
                coolDownImage.fillAmount = 0;
                onCooldown = false;
                background.color = originalColor;
            }
        }
    }

    public void UpdateLevel() {
        levelCooldown.SetLevel(spellLevelRuntime.CooldownLevel);
        if (spellLevelRuntime.SpellType == SpellType.Wall) {
            return;
        }
        levelDamage.SetLevel(spellLevelRuntime.DamageLevel);
        levelDot.SetLevel(spellLevelRuntime.DotLevel);
        if (spellLevelRuntime.SpellType == SpellType.FireBall) {
            levelBounces.SetLevel(spellLevelRuntime.AoeLevel);
        } else if (spellLevelRuntime.SpellType == SpellType.MagicMissile) {
            levelBounces.SetLevel(spellLevelRuntime.BouncesLevel);
        }
    }

    public void Cooldown() {
        onCooldown = true;
        background.color = cooldownColor;
        coolDownImage.fillAmount = 1;
        cooldownTimer = 0f;
        coolDown = Experience.main.GetCooldown(Spell);
    }

}
