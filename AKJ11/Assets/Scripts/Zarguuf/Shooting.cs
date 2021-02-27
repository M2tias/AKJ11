using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooting : MonoBehaviour
{
    [SerializeField]
    private float shootDelay = 1f;
    private float shootTime = 0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        bool canShootAgain = Time.time - shootTime > shootDelay;
        if (Input.GetKey(KeyCode.Mouse0) && canShootAgain)
        {
            Debug.Log("Piu!");
            shootTime = Time.time;
        }
    }
}
