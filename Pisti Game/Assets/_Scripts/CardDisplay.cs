using DG.Tweening;
using UnityEngine;

public class CardDisplay : MonoBehaviour
{

    public ScriptableCard card;
    public SpriteRenderer suitRenderer;
    public SpriteRenderer numberRenderer;
    private FitCollider fitCollider;
    public Vector3 positionInHand;
    public int player = 0;


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

    public void TweenToPosition(Vector3 toPosition, float length)
    {
        transform.DOMove(toPosition, length)
            .SetEase(Ease.OutQuint);
    }
    public void TweenWithEaseInBack(Vector3 toPosition, float length)
    {
        transform.DOMove(toPosition, length)
            .SetEase(Ease.InBack);
    }

    public void TweenX(float newX, float length)
    {
        transform.DOMoveX(newX, length)
            .SetEase(Ease.OutQuint)
            .OnComplete(() => {
                positionInHand.x = gameObject.transform.position.x;
            });
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
