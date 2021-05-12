using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bot : MonoBehaviour
{

    public ArrayList cardObjects = new ArrayList();
    public ArrayList cardDisplays = new ArrayList();

    public void AddToCardObjects(GameObject obj)
    {
        cardObjects.Add(obj);
        cardDisplays.Add(obj.GetComponent<CardDisplay>());
    }


    public int PlayCard()
    {
        return Random.Range(0, cardObjects.Count);
    }
}
