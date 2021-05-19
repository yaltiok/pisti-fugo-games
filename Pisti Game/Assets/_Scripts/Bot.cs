using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class Bot : MonoBehaviour
{

    private const int CLOSED_CARD_COUNT = 3;

    public TMP_Text infoText;


    public ArrayList cardObjects = new ArrayList();
    public ArrayList cardDisplays = new ArrayList();
    public ArrayList stash = new ArrayList();
    public GameObject stashObject;
    public GameManager gameManager;
    private Vector3 stashPos;
    public float screenWidth;
    private int deckCount;

    private int[] cardCounts = new int[13];

    [HideInInspector]
    public int turnPoint = 0;
    [HideInInspector]
    public int totalPoint = 0;
    [HideInInspector]
    public int stashCount = 0;



    private void Start()
    {
        InitCardCounts();
    }

    private void InitCardCounts()
    {
        for (int i = 0; i < cardCounts.Length; i++)
        {
            cardCounts[i] = 4;
        }


    }

    public void PlayCard(CardDisplay lastPlayedDisplay, int midCount, int midPoint)
    {
        int jackIdx = ListContainsNumber(cardDisplays, 11);
        if (lastPlayedDisplay != null)
        {
            // If bot has the card. Play it.
            for (int i = 0; i < cardDisplays.Count; i++)
            {
                CardDisplay cardInHand = (CardDisplay)cardDisplays[i];

                if (lastPlayedDisplay.card.number == cardInHand.card.number)
                {
                    Move(i);
                    return;
                }
            }

            //If there are more than 4 cards on table and if bot has Jack, play Jack.
            if ((midCount >= 5 || midPoint > 1) && jackIdx > 0)
            {
                Move(jackIdx);
                return;
            }

        }
        Move(GetLowestProbability());
    }

    private int GetLowestProbability()
    {
        int toReturn = -1;
        float lowest = 1f;
        for (int i = 0; i < cardDisplays.Count; i++)
        {
            CardDisplay display = (CardDisplay)cardDisplays[i];

            float tempProb = ((float)cardCounts[display.card.number - 1] / (float)(deckCount + CLOSED_CARD_COUNT)) * ((float)display.card.value + 1f);
            tempProb = Mathf.Clamp01(tempProb);
            if (tempProb <= lowest)
            {
                lowest = tempProb;
                toReturn = i;
            }
            
            
        }
        return toReturn;
    }

    public void InformBot(int remaining, int lastPlayedNumber)
    {
        deckCount = remaining;
        cardCounts[lastPlayedNumber - 1] -= 1;
    }

    public void AddToStash(GameObject[] objects, CardDisplay[] displays, int nextPhase, float delay)
    {
        for (int i = 0; i < displays.Length; i++)
        {
            turnPoint += displays[i].card.value;
            stashCount++;
        }

        UpdateInfoText();
        MoveToStash(objects, displays,nextPhase, delay);

    }


    public void UpdateInfoText()
    {
        string currentText = infoText.text;
        string[] arr = currentText.Split(char.Parse("|"));
        string temp = arr[0] + "| " + turnPoint + " | " + totalPoint;
        infoText.text = temp;
    }


    public void MoveToStash(GameObject[] objects, CardDisplay[] cardDisplays, int nextPhase, float delay)
    {
        bool last = false;
        for (int i = objects.Length - 1; i >= 0; i--)
        {
            stash.Add(objects[i]);
            objects[i].transform.SetParent(stashObject.transform);
            if (i == 0)
            {
                last = true;
            }
            cardDisplays[i].TweenWithEaseInBack(objects[i].transform, stashPos, 1f, nextPhase, delay + i * .06f, last);

        }
    }

    public void NewHand(int remaining)
    {
        for (int i = 0; i < cardDisplays.Count; i++)
        {
            CardDisplay display = (CardDisplay)cardDisplays[i];
            InformBot(remaining, display.card.number);
        }
    }

    public void SetStashPos()
    {
        stashPos = stashObject.transform.position;
    }

    public void RoundEnd()
    {
        totalPoint += turnPoint;
        turnPoint = 0;
        stashCount = 0;
        stash = new ArrayList();
        InitCardCounts();
        UpdateInfoText();

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

    private int ListContainsNumber(ArrayList list, int number)
    {
        for (int i = 0; i < list.Count; i++)
        {
            CardDisplay display = (CardDisplay)list[i];
            
            if (display.card.number == number)
            {
                return i;
            }
        }
        return -1;
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
