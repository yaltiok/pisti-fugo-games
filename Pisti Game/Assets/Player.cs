using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class Player : MonoBehaviour
{

    private const float HOLDING_OFFSET = -0.1f;
    Camera cam;
    private GameObject selected;
    private CardDisplay selectedDisplay;
    private bool holdingCard = false;

    public AreaHighlight areaHighlight;
    public GameManager gameManager;
    public TMP_Text infoText;
    public GameObject stashObject;
    private Vector3 stashPos;
    public float lastCardZ;

    private float handAreaHeight;


    public ArrayList cardObjects = new ArrayList();
    public ArrayList cardDisplays = new ArrayList();
    public ArrayList stash = new ArrayList();


    private float camZ_Offset;
    private int holdIndex = -1;

    [HideInInspector]
    public int turnPoint = 0;
    [HideInInspector]
    public int totalPoint = 0;
    [HideInInspector]
    public int stashCount = 0;




    void Start()
    {
        cam = Camera.main;
        camZ_Offset = -cam.transform.position.z;
        handAreaHeight = areaHighlight.handAreaHeight;
    }

    // Update is called once per frame
    void Update()
    {
        CardHandle();
    }

    private void FixedUpdate()
    {
        HoldingCard();
    }

    public void AddToStash(GameObject[] objects,CardDisplay[] displays, int nextPhase, float delay)
    {
        for (int i = 0; i < displays.Length; i++)
        {
            turnPoint += displays[i].card.value;
            stashCount++;
        }
        UpdateInfoText();
        MoveToStash(objects, displays,nextPhase, delay);
    }

    private void ResetValues()
    {
        turnPoint = 0;
        totalPoint = 0;
        stashCount = 0;
    }

    private void CardHandle()
    {
        
        if (Input.GetMouseButtonDown(0))
        {
            if (!holdingCard)
            {
                RaycastHit2D hit = Physics2D.Raycast(cam.ScreenToWorldPoint(Input.mousePosition), cam.transform.forward);
                if (hit.collider != null)
                {
                    if (!hit.transform.CompareTag("Middle"))
                    {
                        selected = hit.transform.gameObject;
                        selectedDisplay = selected.GetComponent<CardDisplay>();
                        gameManager.selected = selected;
                        gameManager.selectedDisplay = selectedDisplay;
                        holdingCard = true;
                        //selectedDisplay.shadow.SetActive(true);
                        ShadowTween(selectedDisplay, true);
                        if (selectedDisplay.player == 1)
                        {
                            holdIndex = GetCardIndex();
                        }
                    }
                }
            }
            
        }
        else if (Input.GetMouseButtonUp(0))
        {
            if (!holdingCard)
            {
                return;
            }
            if (gameManager.phase == 1)
            {
                if (selected.transform.localPosition.y > handAreaHeight - transform.position.y && selectedDisplay.player == 1)
                {
                    // Play Card
                    cardObjects.RemoveAt(holdIndex);
                    cardDisplays.RemoveAt(holdIndex);
                    gameManager.CardPlayed(1f, 2, 1);
                    holdIndex = -1;
                }
                else
                {
                    PutBackInHand();
                }
            }
            else
            {
                PutBackInHand();
            }
            holdingCard = false;
            selected = null;
            //selectedDisplay.shadow.SetActive(false);
            ShadowTween(selectedDisplay, false);
            selectedDisplay = null;
            areaHighlight.ResetHighlight();

        }
        else if (holdingCard && gameManager.phase != 0 && selectedDisplay.player == 1 && selected.transform.localPosition.y < 2f)
        {
            SwapCardPlaces();
        }
    }

    private void HoldingCard()
    {
        if (selected != null && holdingCard)
        {
            selected.transform.position = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, camZ_Offset + lastCardZ));
            if (selected.transform.localPosition.y > handAreaHeight - transform.position.y)
            {
                areaHighlight.HighlightArea(0);
            }
            else
            {
                areaHighlight.HighlightArea(1);
            }
        }
    }

    private void PutBackInHand()
    {
        selectedDisplay.TweenToPosition(selectedDisplay.positionInHand,0.5f);

    }

    private int GetCardIndex()
    {
        for (int i = 0; i < cardObjects.Count; i++)
        {
            if (selected == (GameObject)cardObjects[i])
            {
                return i;
            }
        }
        return -1;
    }


    public void UpdateInfoText()
    {
        string currentText = infoText.text;
        string[] arr = currentText.Split(char.Parse("|"));
        string temp = arr[0] + "| " + turnPoint + " | " + totalPoint;
        infoText.text = temp;
    }

    //public void MoveToStash(GameObject[] objects, CardDisplay[] cardDisplays, float delay)
    //{
    //    bool last = false;
    //    for (int i = 0; i < objects.Length; i++)
    //    {
    //        stash.Add(objects[i]);
    //        objects[i].transform.SetParent(stashObject.transform);
    //        if (i == objects.Length - 1)
    //        {
    //            last = true;
    //        }
    //        cardDisplays[i].TweenWithEaseInBack(objects[i].transform, stashPos, 1f, 2, delay + i * .02f, last);

    //    }
    //}

    public void MoveToStash(GameObject[] objects, CardDisplay[] cardDisplays,int nextPhase, float delay)
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

    private void ShadowTween(CardDisplay display, bool up)
    {
        display.ShadowTween(up);
    }

    public void SetStashPos()
    {
        stashPos = stashObject.transform.position;
    }


    public void RepositionCards(float cardWidth, float gap)
    {
        float a = cardObjects.Count;
        for (int i = 0; i < cardDisplays.Count; i++)
        {
            CardDisplay temp = (CardDisplay)cardDisplays[i];
            float x = transform.position.x + (-a / 2f + .5f + i) * cardWidth + (i  - ((a - 1) / 2f)) * gap;

            temp.TweenX(x, .2f);
        }
         
    }

    public void TurnEnd()
    {
        totalPoint += turnPoint;
        turnPoint = 0;
        stashCount = 0;
        stash = new ArrayList();
        UpdateInfoText();
    }

    private void SwapCardPlaces()
    {
        for (int i = 0; i < cardObjects.Count; i++)
        {
            GameObject otherObject = (GameObject)cardObjects[i];
            CardDisplay otherDisplay = (CardDisplay)cardDisplays[i];
            if (holdIndex > i && otherObject.transform.position.x > selected.transform.position.x)
            {
                //Soluna geçti mi diye bak. X i onlardan küçük mü diye.
                //Swap the two
                SwapTwo(otherObject, otherDisplay, i);
            }
            else if(holdIndex < i && otherObject.transform.position.x < selected.transform.position.x)
            {
                //Sağına geçti mi diye bak. X i onlardan büyük mü diye.
                SwapTwo(otherObject, otherDisplay, i);
            }
        }
    }

    private void SwapTwo(GameObject otherObject, CardDisplay otherDisplay, int i)
    {
        Vector3 tempPos = otherDisplay.positionInHand;
        GameObject tempObject = otherObject;
        CardDisplay tempDisplay = otherDisplay;
        cardObjects[i] = cardObjects[holdIndex];
        cardObjects[holdIndex] = tempObject;
        cardDisplays[i] = cardDisplays[holdIndex];
        cardDisplays[holdIndex] = tempDisplay;
        otherDisplay.TweenX(selectedDisplay.positionInHand.x, .3f);
        otherDisplay.positionInHand = selectedDisplay.positionInHand;
        selectedDisplay.positionInHand = tempPos;
        holdIndex = i;
    }

}
