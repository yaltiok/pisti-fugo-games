using System.Collections;
using System.Collections.Generic;
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
