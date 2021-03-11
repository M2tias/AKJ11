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
        animator.Play(config.flickerAnimationState, -1, Random.Range(0f, 1f));
        spriteRenderer.transform.localPosition = config.spritePosition;
        spriteRenderer.sprite = config.sprite;
        spriteRenderer.flipX = config.spriteXFlip;
    }

    void Start() {
        if (config != null) {
            Initialize(config);
        }
    }

}
