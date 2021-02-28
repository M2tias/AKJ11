using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Aiming : MonoBehaviour
{
    [SerializeField]
    private GameObject aimingReticule;
    private SpriteRenderer aimingRenderer;

    [SerializeField]
    private GameObject hand;
    private SpriteRenderer handRenderer;

    [SerializeField]
    private Sprite fireBallReticule;
    [SerializeField]
    private Sprite wallReticule;

    // Start is called before the first frame update
    void Start()
    {
        handRenderer = hand.GetComponent<SpriteRenderer>();
        aimingRenderer = aimingReticule.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        aimingReticule.transform.position = new Vector3(worldPosition.x, worldPosition.y, 0);

        Vector3 targetDir = GetDirection();
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

    public void SetReticule(Spell spell)
    {
        switch (spell)
        {
            case Spell.fireball:
                aimingRenderer.sprite = fireBallReticule;
                break;
            case Spell.wall:
                aimingRenderer.sprite = wallReticule;
                break;
            default:
                aimingRenderer.sprite = fireBallReticule;
                break;
        }
    }
}

public enum Spell //TODO: Move this elsewhere
{
    fireball,
    wall
}
