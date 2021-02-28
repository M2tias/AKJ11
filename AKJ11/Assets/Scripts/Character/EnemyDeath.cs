using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDeath : MonoBehaviour
{

    public ParticleSystem DieEffect;

    public void Death()
    {
        Instantiate(DieEffect, MapGenerator.main.GetContainer()).transform.position = transform.position;
        Destroy(gameObject);
    }
}
