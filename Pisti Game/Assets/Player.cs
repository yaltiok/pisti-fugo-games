using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    Camera cam;
    private GameObject selected;
    private CardDisplay selectedDisplay;
    private bool holdingCard = false;

    public AreaHighlight areaHighlight;
    public GameManager gameManager;

    public ArrayList cardObjects = new ArrayList();
    public ArrayList cardDisplays = new ArrayList();
    public ArrayList stash = new ArrayList();


    private float camZ_Offset;
    private int holdIndex = -1;

    public int turnPoint = 0;
    public int totalPoint = 0;
    public int stashCount = 0;




    void Start()
    {
        cam = Camera.main;
        camZ_Offset = -cam.transform.position.z;
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

    private void CardHandle()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (!holdingCard)
            {
                RaycastHit2D hit = Physics2D.Raycast(cam.ScreenToWorldPoint(Input.mousePosition), cam.transform.forward);
                if (hit.collider != null)
                {
                    selected = hit.transform.gameObject;
                    selectedDisplay = selected.GetComponent<CardDisplay>();
                    gameManager.selected = selected;
                    gameManager.selectedDisplay = selectedDisplay;
                    holdingCard = true;
                    holdIndex = GetCardIndex();
                }
            }
            
        }
        else if (Input.GetMouseButtonUp(0))
        {
            if (holdingCard)
            {
                if (selected.transform.localPosition.y > 2f)
                {
                    if (gameManager.phase == 1)
                    {
                        // Play Card
                        //ChangePhase(4);
                        gameManager.CardPlayed(1f,2,1);
                        cardObjects.RemoveAt(holdIndex);
                        cardDisplays.RemoveAt(holdIndex);
                        holdIndex = -1;

                    }
                    else
                    {
                        Debug.Log("Sıranı Bekle!!");
                        selected.transform.position = selectedDisplay.positionInHand;
                    }
                }
                else
                {
                    //Put card back in hand
                    selected.transform.position = selectedDisplay.positionInHand;
                }
                holdingCard = false;
                selected = null;
                selectedDisplay = null;
                areaHighlight.ResetHighlight();
            }
        }
    }

    private void HoldingCard()
    {
        if (selected != null && holdingCard)
        {
            selected.transform.position = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, camZ_Offset));
            if (selected.transform.localPosition.y > 2f)
            {
                areaHighlight.HighlightArea(0);
            }
            else
            {
                areaHighlight.HighlightArea(1);
            }
        }
    }

    private void ChangePhase(int i)
    {
        gameManager.phase = i;
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

}
