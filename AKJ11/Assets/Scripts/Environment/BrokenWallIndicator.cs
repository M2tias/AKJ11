using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrokenWallIndicator : MonoBehaviour
{
    [SerializeField]
    private SpriteMask spriteMask;
    public void SetSprite(Sprite sprite) {
        spriteMask.sprite = sprite;
    }
}
