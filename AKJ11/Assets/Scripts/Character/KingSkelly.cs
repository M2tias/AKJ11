using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KingSkelly : MonoBehaviour
{
    [SerializeField]
    private GameObject sword;
    private Rigidbody2D swordBody;
    private Animator swordAnim;
    private Vector2 swordStartPos;
    [SerializeField]
    private GameObject upperSwordArm;
    private Vector2 upperSwordStartPos;
    [SerializeField]
    private GameObject lowerSwordArm;
    private Vector2 lowerSwordStartPos;

    [SerializeField]
    private GameObject shield;
    [SerializeField]
    private GameObject upperShieldArm;
    [SerializeField]
    private GameObject lowerShieldArm;

    [SerializeField]
    private Transform attackTarget;

    private bool attack = false;
    private bool attacked = false;
    private float attackStarted;
    private float attackTime = 1f;
    private bool returnSword = false;
    private float returnSwordStarted;
    private float returnTime = 2f;

    void Start()
    {
        swordBody = sword.GetComponent<Rigidbody2D>();
        swordAnim = sword.GetComponent<Animator>();
        swordStartPos = sword.transform.localPosition;
        upperSwordStartPos = upperSwordArm.transform.localPosition;
        lowerSwordStartPos = lowerSwordArm.transform.localPosition;
    }

    void Update()
    {
        Vector2 swordPos = new Vector2(sword.transform.localPosition.x, sword.transform.localPosition.y);

        if (Input.GetKey(KeyCode.K))
        {
            attack = true;
        }

        if (attack)
        {
            if (Vector2.Distance(sword.transform.position, attackTarget.position) > 0.2f)
            {
                swordBody.velocity = (attackTarget.position - sword.transform.position).normalized * 5f;
            }
            else
            {
                swordBody.velocity = Vector2.zero;
                swordAnim.Play("sword_attack");
                attack = false;
                attacked = true;
                attackStarted = Time.time;
            }
        }

        if (attacked && Time.time - attackStarted >= attackTime)
        {
            attacked = false;
            returnSword = true;
            returnSwordStarted = Time.time;
        }

        if (returnSword)
        {
            sword.transform.localPosition = Vector2.Lerp(sword.transform.localPosition, swordStartPos, (Time.time - returnSwordStarted) / returnTime);
            
            if ((Time.time - returnSwordStarted) > returnTime - 0.1f)
            {
                returnSword = false;
                sword.transform.localPosition = swordStartPos;
            }
        }

        if(Vector2.Distance(sword.transform.localPosition, swordStartPos) > 0.5f)
        {
            upperSwordArm.transform.localPosition = (swordPos - upperSwordStartPos) * 0.33f;
            lowerSwordArm.transform.localPosition = (swordPos - lowerSwordStartPos) * 0.66f;
        }
        else
        {
            upperSwordArm.transform.localPosition = upperSwordStartPos;
            lowerSwordArm.transform.localPosition = lowerSwordStartPos;
        }
    }
}
