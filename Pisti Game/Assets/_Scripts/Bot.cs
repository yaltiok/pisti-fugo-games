using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bot : MonoBehaviour
{

    public ArrayList cardObjects = new ArrayList();
    public ArrayList cardDisplays = new ArrayList();
    public ArrayList stash = new ArrayList();

    public int turnPoint = 0;
    public int totalPoint = 0;
    public int stashCount = 0;

    public void AddToCardObjects(GameObject obj)
    {
        cardObjects.Add(obj);
        cardDisplays.Add(obj.GetComponent<CardDisplay>());
    }


    public int PlayCard()
    {
        return Random.Range(0, cardObjects.Count);
    }

    public void AddToStash(CardDisplay[] arr)
    {
        for (int i = 0; i < arr.Length; i++)
        {
            turnPoint += arr[i].card.value;
            stashCount++;
        }
    }

    private void ResetValues()
    {
        turnPoint = 0;
        totalPoint = 0;
        stashCount = 0;
    }
}
