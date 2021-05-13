using DG.Tweening;
using UnityEngine;

public class CardDisplay : MonoBehaviour
{

    public ScriptableCard card;
    public SpriteRenderer suitRenderer;
    public SpriteRenderer numberRenderer;
    private FitCollider fitCollider;
    public Vector3 positionInHand;


    void Start()
    {
        fitCollider = GetComponent<FitCollider>();
        SetSprites();
        fitCollider.FitColliders(suitRenderer.sprite.bounds.size);
        
    }


    public void SwitchOrientation()
    {
        card.orientation *= -1;
        SetSprites();
    }

    public void TweenToPosition(Vector3 toPosition)
    {
        transform.DOMove(toPosition, 1f)
            .SetEase(Ease.OutQuint);
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
