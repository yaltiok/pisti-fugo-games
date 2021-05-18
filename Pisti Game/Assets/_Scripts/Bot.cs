﻿using System.Collections;
using TMPro;
using UnityEngine;

public class Bot : MonoBehaviour
{

    public TMP_Text infoText;


    public ArrayList cardObjects = new ArrayList();
    public ArrayList cardDisplays = new ArrayList();
    public ArrayList stash = new ArrayList();
    public GameObject stashObject;
    public GameManager gameManager;
    private Vector3 stashPos;
    public float screenWidth;



    [HideInInspector]
    public int turnPoint = 0;
    [HideInInspector]
    public int totalPoint = 0;
    [HideInInspector]
    public int stashCount = 0;


    public void PlayCard(CardDisplay lastPlayedDisplay)
    {
        if (lastPlayedDisplay != null)
        {
            for (int i = 0; i < cardDisplays.Count; i++)
            {
                CardDisplay cardInHand = (CardDisplay)cardDisplays[i];

                if (lastPlayedDisplay.card.number == cardInHand.card.number)
                {
                    Move(i);
                    return;
                }
            }
            RandomMove();
            return;

        }
        
        RandomMove();
        
    }

    public void AddToStash(GameObject[] objects, CardDisplay[] displays)
    {
        for (int i = 0; i < displays.Length; i++)
        {
            turnPoint += displays[i].card.value;
            stashCount++;
        }

        UpdateInfoText();
        MoveToStash(objects, displays);

    }

    private void ResetValues()
    {
        turnPoint = 0;
        stashCount = 0;
    }


    public void UpdateInfoText()
    {
        string currentText = infoText.text;
        string[] arr = currentText.Split(char.Parse("|"));
        string temp = arr[0] + " | " + turnPoint + " | " + totalPoint;
        infoText.text = temp;
    }


    public void MoveToStash(GameObject[] objects, CardDisplay[] cardDisplays)
    {
        bool last = false;

        for (int i = 0; i < objects.Length; i++)
        {
            stash.Add(objects[i]);
            objects[i].transform.SetParent(stashObject.transform);
            if (i == objects.Length - 1)
            {
                last = true;
            }
            cardDisplays[i].TweenWithEaseInBack(objects[i].transform, stashPos, 1f, 1, .5f + i * .02f, last);
            
        }
    }

    public void SetStashPos()
    {
        stashPos = stashObject.transform.position;
    }


    private void Move(int i)
    {
        gameManager.selected = (GameObject)cardObjects[i];
        gameManager.selectedDisplay = (CardDisplay)cardDisplays[i];
        gameManager.selectedDisplay.SwitchOrientation();
        cardObjects.RemoveAt(i);
        cardDisplays.RemoveAt(i);
        gameManager.CardPlayed(.5f, 1, -1);
    }

    private void RandomMove()
    {
        int a = Random.Range(0, cardDisplays.Count);
        Move(a);
    }

    public void RepositionCards(float cardWidth, float gap)
    {
        float a = cardObjects.Count;
        for (int i = 0; i < cardDisplays.Count; i++)
        {
            CardDisplay temp = (CardDisplay)cardDisplays[i];
            float x = transform.position.x + (-a / 2f + .5f + i) * cardWidth + (i - ((a - 1) / 2f)) * gap;

            temp.TweenX(x, .2f);
        }

    }
}
