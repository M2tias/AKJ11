using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BillboardText : MonoBehaviour
{

    private Transform camTransform;
    private Quaternion originalRotation;
    private RectTransform rt;

    [SerializeField]
    private Text txtMessage;

    [SerializeField]
    private Animator animator;

    public void Initialize(Vector2 pos, string text, Color color, Transform parent)
    {
        rt = GetComponent<RectTransform>();
        originalRotation = transform.rotation;
        camTransform = Camera.main.transform;
        txtMessage.text = text;
        txtMessage.color = color;
        transform.SetParent(parent);
        transform.position = pos;
        animator.SetTrigger("Show");
    }

    public void Die()
    {
        Destroy(gameObject);
    }

    void Update()
    {
        transform.rotation = originalRotation * camTransform.rotation;
    }

}
