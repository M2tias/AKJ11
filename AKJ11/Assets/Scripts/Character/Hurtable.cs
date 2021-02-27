using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Hurtable : MonoBehaviour
{
    [SerializeField]
    HealthScriptableObject healthScriptableObject;
    private float currentHealth;

    [SerializeField]
    private UnityEvent deadAction;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = healthScriptableObject.MaxHealth;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Hurt(float damage)
    {
        Debug.Log(currentHealth);
        Debug.Log(damage);
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Debug.Log("CALL!");
            deadAction.Invoke();
        }
        else
        {
            Debug.Log("OUCH!");
        }
    }
}
