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
            });
    }


    public void TweenToPoint(GameObject obj, Vector3 toPosition, float length)
    {
        obj.transform.DOMove(toPosition, length)
            .SetEase(Ease.OutQuint);
    }
}
