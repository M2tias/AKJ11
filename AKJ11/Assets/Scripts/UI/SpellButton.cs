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
    private Text txtDamage;

    [SerializeField]
    private Text txtCooldown;

    [SerializeField]
    private Text txtDot;
    [SerializeField]
    private Text txtBounces;

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
        txtCooldown.text = $"{spellLevelRuntime.CooldownLevel + 1}";
        if (spellLevelRuntime.SpellType == SpellType.Wall) {
            return;
        }
        txtDamage.text = $"{spellLevelRuntime.DamageLevel + 1}";
        txtDot.text = $"{spellLevelRuntime.DotLevel + 1}";
        if (spellLevelRuntime.SpellType == SpellType.FireBall) {
            txtBounces.text = $"{spellLevelRuntime.AoeLevel + 1}";
        } else if (spellLevelRuntime.SpellType == SpellType.MagicMissile) {
            txtBounces.text = $"{spellLevelRuntime.BouncesLevel + 1}";
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
