using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardDisplay : MonoBehaviour
{

    public ScriptableCard card;
    public SpriteRenderer suitRenderer;
    public SpriteRenderer numberRenderer;
    private FitCollider fitCollider;


    void Start()
    {
        fitCollider = GetComponent<FitCollider>();
        SetSprites();
        fitCollider.FitColliders(suitRenderer.sprite.bounds.size);

    }

    void Update()
    {
        
    }


    public void SwitchOrientation()
    {
        card.orientation *= -1;
        SetSprites();
    }

    public void SetSprites()
    {
        if (card.orientation == 1)
        {
            suitRenderer.sprite = card.suit;
            numberRenderer.sprite = card.numberSprite;
        }
        else if (card.orientation == -1)
        {
            suitRenderer.sprite = card.cardBack;
            numberRenderer.sprite = null;
        }
    }
}
