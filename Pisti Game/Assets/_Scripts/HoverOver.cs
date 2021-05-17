using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoverOver : MonoBehaviour
{
    public SpriteRenderer cardFaceSpriteRenderer;
    private Material mat;

    private void Start()
    {
        mat = cardFaceSpriteRenderer.material;
    }

    private void OnMouseEnter()
    {
        mat.SetFloat("GlowFactor", 1);
    }

    private void OnMouseExit()
    {
        mat.SetFloat("GlowFactor", 0);

    }
}
