using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TheEndView : MonoBehaviour
{
    public static TheEndView main;
    [SerializeField]
    private GameObject container;
    private Text txtEndMessage;
    private void Awake() {
        container.SetActive(false);
        main = this;
    }

    public void Show(string message) {
        container.SetActive(true);
        txtEndMessage = GetComponentInChildren<Text>();
        string seed = RandomNumberGenerator.GetInstance().Seed;
        SeedView.main.gameObject.SetActive(false);
        UITimer.main.gameObject.SetActive(false);
        string time = GameStateManager.main.GetFormattedTime();
        string xp = Experience.main.TotalExpGained.ToString();
        message += $"\n\nSeed: {seed}\nTime: {time}\nTotal XP: {xp}";
        txtEndMessage.text = message;
    }
}
