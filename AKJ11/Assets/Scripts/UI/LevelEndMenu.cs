using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

public class LevelEndMenu : MonoBehaviour
{
    public static LevelEndMenu main;

    void Awake()
    {
        main = this;
    }

    [SerializeField]
    private GameObject container;
    [SerializeField]
    private Text txtTitle;

    [SerializeField]
    private UIOpenSpellbook spellbook;

    private bool isOpen = false;
    public void Open()
    {
        if (!isOpen)
        {
            SoundManager.main.PlaySound(GameSoundType.OpenSpellBook);
            isOpen = true;
            GameStateManager.main.StopTime();
            txtTitle.text = $"Level {MapGenerator.main.CurrentLevel} complete!";
            container.SetActive(true);
        }
    }

    public void GoBack() {
        Close();
        GameStateManager.main.StartTime();
    }

    public void Close()
    {
        if (isOpen)
        {
            SoundManager.main.PlaySound(GameSoundType.CloseSpellBook);
            isOpen = false;
            container.SetActive(false);
        }
    }

    public void ClickNextLevel()
    {
        NextLevel();
    }

    public async void NextLevel()
    {
        spellbook.ApplyPoints();
        Close();
        await MapGenerator.main.LevelEnd();
    }

}
