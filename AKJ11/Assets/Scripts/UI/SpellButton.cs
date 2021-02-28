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

    private float cooldownTimer = 0;
    private bool onCooldown = false;

    private float coolDown = -1;

    void Start() {
        Init();
    }
    public void Init() {
        txtName.text = Spell.Name;
        icon.sprite = Spell.ProjectileSprite;
        originalColor = background.color;
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

    public void Cooldown() {
        onCooldown = true;
        background.color = cooldownColor;
        coolDownImage.fillAmount = 1;
        cooldownTimer = 0f;
        coolDown = Experience.main.GetCooldown(Spell);
    }
}
