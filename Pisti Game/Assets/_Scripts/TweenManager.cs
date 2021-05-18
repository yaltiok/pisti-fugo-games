using TMPro;
using UnityEngine;
using DG.Tweening;

public class TweenManager : MonoBehaviour
{
    public GameManager gameManager;
    public void CardPlayTween(GameObject obj, Vector3 toPosition,float length, int nextPhase)
    {
        obj.transform.DOMove(toPosition, length)
            .SetEase(Ease.OutQuint)
            .OnComplete(() => {
                gameManager.phase = nextPhase;
                if (nextPhase == 2)
                {
                    gameManager.BotMove();
                }
                else if (nextPhase == 0)
                {
                    gameManager.DealNewHands();
                }
            });
    }


    public void TweenToPoint(GameObject obj, Vector3 toPosition, float length)
    {
        obj.transform.DOMove(toPosition, length)
            .SetEase(Ease.OutQuint);
    }

    public void TweenToLocal(GameObject obj, Vector3 toPosition, float length)
    {
        obj.transform.DOLocalMove(toPosition, length)
            .SetEase(Ease.OutQuint);
    }

    public void TweenX(GameObject obj, float newX, float length)
    {
        obj.transform.DOMoveX(newX, length)
            .SetEase(Ease.OutQuint);
    }

    public void TextTween(TMP_Text textElement, float endValue, float length)
    {
        Sequence textSequence = DOTween.Sequence();
        textSequence.Append(textElement.DOFade(endValue, length))
          .AppendInterval(length)
          .Append(textElement.DOFade(0, length*2))
          //.Append(textElement.rectTransform.DOMoveX(-7f, length))
          .Insert(0, textElement.rectTransform.DOScale(new Vector3(7, 7, 7), textSequence.Duration()/2f))
          .OnComplete(() => {
              textElement.rectTransform.localScale = new Vector3(1,1,1);
              gameManager.asd();
          });
        textSequence.Play();
    }

    public void TweenWithEaseInBack(Transform obj, Vector3 toPosition, float length, float delay)
    {
        obj.DOMove(toPosition, length)
            .SetEase(Ease.InBack)
            .SetDelay(delay);
    }


    public void TweenWithEaseInBack(Transform obj, Vector3 toPosition, float length, int nextPhase, float delay, bool last)
    {
        obj.DOMove(toPosition, length)
            .SetEase(Ease.InBack)
            .SetDelay(delay)
            .OnComplete(() => {
                if (last)
                {
                    gameManager.phase = nextPhase;
                    if (nextPhase == 2)
                    {
                        gameManager.BotMove();

                    }
                    else if (nextPhase == 0)
                    {
                        gameManager.DealNewHands();
                    }
                }
                Destroy(obj.gameObject, 5f);
                
            });
    }
}
