using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrokenWallIndicator : MonoBehaviour
{
    [SerializeField]
    private SpriteMask spriteMask;
    [SerializeField]
    private SpriteRenderer spriteRenderer;
    [SerializeField]
    private GameObject pSystem;
    public void SetSprite(Sprite sprite) {
        spriteMask.sprite = sprite;
    }

    public void Hide() {
        spriteRenderer.enabled = false;
    }

    public void ShowParticleEffect() {
        pSystem.SetActive(true);
    }
}
