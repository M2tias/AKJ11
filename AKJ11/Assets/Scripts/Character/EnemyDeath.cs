using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDeath : MonoBehaviour
{
    public void Death()
    {
        Debug.Log("�rp! kuolin :( t. vihuliini");
        Destroy(gameObject);
    }
}
