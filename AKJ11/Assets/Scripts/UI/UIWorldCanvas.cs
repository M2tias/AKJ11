using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIWorldCanvas : MonoBehaviour
{
    public static UIWorldCanvas main;
    private void Awake()
    {
        main = this;
    }

    [SerializeField]
    private Transform container;

    private Color defaultColor = Color.white;

    private Transform playerTransform;

    [field: SerializeField]
    public ResourceTypesConfig ResourceTypesConfig { get; private set; }


    public void ShowNumberAtPlayer(int number, ResourceType type, bool showAbbreviation = true)
    {
        if (playerTransform == null)
        {
            playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        }
        ShowNumber(playerTransform.position, number, type, showAbbreviation);
    }
    public void ShowNumber(Vector2 pos, float number, ResourceType type, bool showAbbreviation = true)
    {
        ShowNumberFormatted(pos, number < 0, number.ToString(), type, showAbbreviation);
    }

    public void ShowNumber(Vector2 pos, int number, ResourceType type, bool showAbbreviation = true)
    {
        ShowNumberFormatted(pos, number < 0, number.ToString(), type, showAbbreviation);
    }

    private void ShowNumberFormatted(Vector2 pos, bool isNegative, string numberAsString, ResourceType type, bool showAbbreviation)
    {
        ResourceTypeConfig config = ResourceTypesConfig.Get(type);
        Color color = config.TextColorWhenPositive;
        string text = numberAsString;
        if (isNegative)
        {
            color = config.TextColorWhenNegative;
        }
        else
        {
            text = "+" + text;
        }
        if (showAbbreviation) {
            text += config.Abbreviation;
        }
        ShowTextWithColor(pos, text, color);
    }

    private void ShowTextWithColor(Vector2 pos, string text, Color color)
    {
        BillboardText billboardText = Prefabs.Get<BillboardText>();
        billboardText.Initialize(pos, text, color, container);
    }
}
