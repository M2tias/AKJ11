using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Aiming : MonoBehaviour
{
    [SerializeField]
    private GameObject aimingReticule;

    [SerializeField]
    private GameObject hand;
    private SpriteRenderer handRenderer;

    // Start is called before the first frame update
    void Start()
    {
        handRenderer = hand.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        aimingReticule.transform.position = new Vector3(worldPosition.x, worldPosition.y, 0);

        Vector3 targetDir = GetDirection(); //aimingReticule.transform.position - transform.position;
        float angleDiff = Vector2.SignedAngle(hand.transform.right, targetDir);
        hand.transform.Rotate(Vector3.forward, angleDiff);
        
        if (worldPosition.x < transform.position.x)
        {
            handRenderer.flipY = true;
        }
        else
        {
            handRenderer.flipY = false;
        }

        if (Input.GetKey(KeyCode.Z))
        {
            handRenderer.flipY = true;
        }
    }

    public Vector3 GetDirection()
    {
        return aimingReticule.transform.position - transform.position;
    }
}
