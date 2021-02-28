using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIOpenSpellbook : MonoBehaviour
{

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
