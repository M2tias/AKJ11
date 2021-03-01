using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UIOpenSpellbook : MonoBehaviour
{
    [SerializeField]
    private UnityEvent spell1Done;
    [SerializeField]
    private UnityEvent spell2Done;
    [SerializeField]
    private UnityEvent spellWallDone;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I) || Input.GetKeyDown(KeyCode.Tab))
        {
            Toggle();
        }
    }

    [SerializeField]
    private GameObject spellBook;
    public void Open() {
        spellBook.SetActive(true);
    }

    public void Close() {
        spellBook.SetActive(false);
        spell1Done.Invoke();
        spell2Done.Invoke();
        spellWallDone.Invoke();
    }

    public void Toggle() {
        if (spellBook.activeSelf) {
            Close();
        } else {
            Open();
        }
    }
}
