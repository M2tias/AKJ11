using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITimer : MonoBehaviour
{

    public static UITimer main;
    [SerializeField]
    private Text txtTimer;
    public Timer timer {private get; set;}

    void Awake()
    {
        main = this;
    }

    void Update()
    {
        if (timer != null) {
            txtTimer.text = timer.GetString();
        }
    }
}
