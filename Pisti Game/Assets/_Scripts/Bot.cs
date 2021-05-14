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
        for (int i = 0; i < objects.Length; i++)
        {
            stash.Add(objects[i]);
            objects[i].transform.parent = stashObject.transform;
            cardDisplays[i].TweenToPosition(stashPos, 1f);
            
        }
    }

    public void SetStashPos()
    {
        stashPos = stashObject.transform.position;
    }


    private void Move(int i)
    {
        //Debug.Log(cardDisplays.Count +  " " + i);
        
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
}
