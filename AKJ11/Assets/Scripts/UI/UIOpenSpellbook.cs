using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIOpenSpellbook : MonoBehaviour
{
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
    }

    public void Toggle() {
        if (spellBook.activeSelf) {
            Close();
        } else {
            Open();
        }
    }
}
