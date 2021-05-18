using System.Collections;
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

    public void SetStashPos()
    {
        stashPos = stashObject.transform.position;
    }

    public void TurnEnd()
    {
        totalPoint += turnPoint;
        turnPoint = 0;
        stashCount = 0;
        stash = new ArrayList();
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
