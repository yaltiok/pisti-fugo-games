using DG.Tweening;
using UnityEngine;
using TMPro;
using System;

public class CardDisplay : MonoBehaviour
{

    public ScriptableCard card;
    public SpriteRenderer suitRenderer;
    public TMP_Text numberText;
    public GameObject shadow;
    private Vector3 shadowPos = new Vector3(0.2f,-0.2f);
    private string number = "";
    private FitCollider fitCollider;
    public Vector3 positionInHand;
    public int player = 0;
    public Color redColor;
    public Color blackColor;
    private TweenManager tweenManager;


    void Start()
    {
        SetCardNumberText();
        SetSprites();
        fitCollider = GetComponent<FitCollider>();
        fitCollider.FitColliders(suitRenderer.sprite.bounds.size);
        tweenManager = FindObjectOfType<TweenManager>();
        

    }

    private void SetCardNumberText()
    {
        if (card.number < 11)
        {
            number = card.number.ToString();
        }
    }

    public void SwitchOrientation()
    {
        card.orientation *= -1;
        SetSprites();
    }

    public void TweenToPosition(Vector3 toPosition, float length)
    {
        transform.DOMove(toPosition, length)
            .SetEase(Ease.OutQuint);
    }
    public void TweenWithEaseInBack(Transform obj, Vector3 toPosition, float length, int nextPhase, float delay, bool last)
    {
        tweenManager.TweenWithEaseInBack(obj, toPosition, length, nextPhase, delay, last);
    }

    public void CardDealTween(Vector3 toPos, float length)
    {
        Sequence sq = DOTween.Sequence();
        sq.Append(transform.DOMove(toPos, length))
          .Insert(0, transform.DORotate(new Vector3(0, 0, 0), sq.Duration()));
        sq.Play();
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
        if (card.isRed)
        {
            numberText.color = redColor;
        }
        else
        {
            numberText.color = blackColor;

        }
        if (card.orientation == 1)
        {
            suitRenderer.sprite = card.suit;
            numberText.text = number;
        }
        else if (card.orientation == -1)
        {
            suitRenderer.sprite = card.cardBack;
            numberText.text = "";
        }
    }

    public void ShadowTween(bool up)
    {
        if (up)
        {
            tweenManager.TweenToLocal(shadow, shadowPos, 1f);
        }
        else
        {
            tweenManager.TweenToLocal(shadow, Vector3.zero, 1f);
        }
    }
}
