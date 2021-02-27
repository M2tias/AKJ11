using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    [SerializeField]
    private float lifetime;
    private float created;

    [SerializeField]
    private float endFadeAmount;
    private SpriteRenderer renderer;

    public void Initialize(Vector2 position, Vector2 direction)
    {
        transform.position = new Vector3(position.x, position.y, 0);

        //Vector3 targetDir = aimingReticule.transform.position - transform.position;
        float angleDiff = Vector2.SignedAngle(transform.right, direction);
        transform.transform.Rotate(Vector3.forward, angleDiff);;
    }

    // Start is called before the first frame update
    void Start()
    {
        created = Time.time;
        renderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time - created > lifetime)
        {
            Destroy(gameObject);
        }

        Color color = renderer.color;
        color.a = Mathf.Lerp(1f, endFadeAmount, (Time.time - created) / lifetime);
        renderer.color = color;
    }
}
