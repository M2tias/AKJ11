using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SeedView : MonoBehaviour
{
    public static SeedView main;
    private void Awake() {
        main = this;
    }

    [SerializeField]
    private Text txtSeed;
    public void SetText(string seed) {
        txtSeed.text = $"seed: {seed}";
    }
}
