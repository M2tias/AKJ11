using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIOpenSpellbook : MonoBehaviour
{
    [SerializeField]
    private UnityEvent spell1Done;
    [SerializeField]
    private UnityEvent spell2Done;
    [SerializeField]
    private UnityEvent spellWallDone;
    [SerializeField]
    private Sprite defaultBook;
    [SerializeField]
    private Sprite pointsLeftBook;
    [SerializeField]
    private Text pointsLeftText;

    [SerializeField]
    private Image bookImage;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I) || Input.GetKeyDown(KeyCode.Tab))
        {
            Toggle();
        }

        if(Experience.main.GetUnusedStatPoints() >  0)
        {
            bookImage.sprite = pointsLeftBook;
            pointsLeftText.text = Experience.main.GetUnusedStatPoints().ToString();
            pointsLeftText.gameObject.SetActive(true);
        }
        else
        {
            bookImage.sprite = defaultBook;
            pointsLeftText.text = "0";
            pointsLeftText.gameObject.SetActive(false);
        }
    }

    [SerializeField]
    private GameObject spellBook;
    public void Open() {
        spellBook.SetActive(true);
        if (SoundManager.main != null) {
            SoundManager.main.PlaySound(GameSoundType.OpenSpellBook);
        }
    }

    public void Close() {
        spellBook.SetActive(false);
        spell1Done.Invoke();
        spell2Done.Invoke();
        spellWallDone.Invoke();
        if (SoundManager.main != null) {
            SoundManager.main.PlaySound(GameSoundType.CloseSpellBook);
        }
    }

    public void Toggle() {
        if (spellBook.activeSelf) {
            Close();
        } else {
            Open();
        }
    }
}
