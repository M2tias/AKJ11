using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverView : MonoBehaviour
{
    public static GameOverView main;
    [SerializeField]
    private GameObject container;
    private void Awake() {
        main = this;
        container.SetActive(false);
    }

    public void Show() {
        container.SetActive(true);
    }
}
