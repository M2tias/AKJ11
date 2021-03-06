using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Torch : MonoBehaviour
{
    private TorchConfig config;
    [SerializeField]
    private Animator animator;
    [SerializeField]
    private SpriteRenderer spriteRenderer;

    public void Initialize(TorchConfig config)
    {
        this.config = config;
    }

    // Start is called before the first frame update
    void Start()
    {
        animator.Play(config.flickerAnimationState, -1, Random.Range(0f, 1f));
        spriteRenderer.transform.localPosition = config.spritePosition;
        spriteRenderer.sprite = config.sprite;
        spriteRenderer.flipX = config.spriteXFlip;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
