using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossShield : MonoBehaviour
{
    private SpriteRenderer rend;
    private Collider2D coll;

    float displayDuration = 0.5f;
    float displayTimer = 0.0f;

    bool initialized = false;

    // Start is called before the first frame update
    void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        if (!initialized)
        {
            initialized = true;
            rend = GetComponent<SpriteRenderer>();
            coll = GetComponent<Collider2D>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        var diff = displayTimer - Time.time;
        if (diff < 0)
        {
            rend.enabled = false;
        }
        else
        {
            rend.enabled = true;
            var color = rend.color;
            color.a = Mathf.Lerp(0.0f, 1.0f, diff / displayDuration);
            rend.color = color;
        }
    }

    public void Activate()
    {
        Initialize();
        coll.enabled = true;
    }

    public void Deactivate()
    {
        Initialize();
        coll.enabled = false;
    }

    public void Hit()
    {
        displayTimer = Time.time + displayDuration;
    }
}
