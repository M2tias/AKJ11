using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZarguufDeath : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Death()
    {
        Debug.Log("�rp! kuolin :(");
        Destroy(gameObject);
    }
}
