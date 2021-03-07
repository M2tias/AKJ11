using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTarget : MonoBehaviour
{
    public Transform origin;
    public Transform pointer;
    public float maxRange = 3.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 diff = pointer.position - origin.position;
        var dist = Mathf.Min(maxRange, diff.magnitude);
        transform.position = (Vector2)origin.position + diff.normalized * dist;
    }
}
