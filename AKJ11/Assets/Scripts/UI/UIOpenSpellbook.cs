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

    public void ApplyPoints() {
        spell1Done.Invoke();
        spell2Done.Invoke();
        spellWallDone.Invoke();
    }

}
