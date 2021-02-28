using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TheEndView : MonoBehaviour
{
    public static TheEndView main;
    [SerializeField]
    private GameObject container;
    private void Awake() {
        container.SetActive(false);
        main = this;
    }

    public void Show() {
        container.SetActive(true);
    }
}
