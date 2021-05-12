using System;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{

    private const float CARD_Z_OFFSET = -0.1f;
    public int handCount = 4;


    public GameObject playerHand;
    public GameObject botHand;
    public DeckManager deckManager;
    public AreaHighlight areaHighlight;
    public TMP_Text middleCountText;
    public Transform middlePos;
    public Transform deckPosition;
    public GameObject cardPrefab;
    public Bot bot;

    private int middleCount = 0;


    private float rayTime = 1f;
    private float rayCounter = 0f;
    private float camZ_Offset;
    private float cardZ_Offset = -1f;

    private GameObject selected;
    private CardDisplay selectedDisplay;
    private GameObject lastPlayed;
    private CardDisplay lastPlayedDisplay;

    private bool holdingCard = false;

    
    
    private int phase = 0; //0: Deal new hands 1: Player's turn 2: Bot's turn 3: Collecting cards from middle. 4: Transition

    private Camera cam;

    void Start()
    {
        cam = Camera.main;
        camZ_Offset = -cam.transform.position.z;
        SetHandPositions();
        SetDeckPosition();
        DealNewHands();
    }

    

    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Space) && a)
        //{
        //    a = false;
        //    for (int i = 0; i < playerHand.transform.childCount; i++)
        //    {
        //        playerHand.transform.GetChild(i).GetComponent<CardDisplay>().SwitchOrientation();
        //    }
        //    for (int i = 0; i < botHand.transform.childCount; i++)
        //    {
        //        botHand.transform.GetChild(i).GetComponent<CardDisplay>().SwitchOrientation();
        //    }
        //}

        if (phase == 2)
        {
            int a = bot.PlayCard();
            selected = (GameObject)bot.cardObjects[a];
            selectedDisplay = (CardDisplay)bot.cardDisplays[a];
            bot.cardObjects.RemoveAt(a);
            bot.cardDisplays.RemoveAt(a);
            Vector3 toPos = middlePos.position + new Vector3(0, 0, cardZ_Offset);
            cardZ_Offset += CARD_Z_OFFSET;
            selectedDisplay.SwitchOrientation();
            selectedDisplay.TweenToPosition(toPos);
            if (lastPlayed != null)
            {
                if (lastPlayedDisplay.card.number == selectedDisplay.card.number)
                {
                    Debug.Log("HEYYYOOOOO!!");
                }
            }
            lastPlayed = selected;
            lastPlayedDisplay = selectedDisplay;
            middleCount++;
            middleCountText.text = middleCount.ToString();
            selected.transform.SetParent(middlePos);
            phase = 1;


        }


        if (Input.GetMouseButtonUp(0))
        {
            if (!holdingCard)
            {
                RaycastHit2D hit = Physics2D.Raycast(cam.ScreenToWorldPoint(Input.mousePosition), cam.transform.forward);
                if (hit.collider != null)
                {
                    selected = hit.transform.gameObject;
                    selectedDisplay = selected.GetComponent<CardDisplay>();
                    holdingCard = true;
                }
            }
            else
            {
                if (selected.transform.localPosition.y > 2f)
                {
                    if (phase == 1)
                    {
                        // Play Card
                        Vector3 toPos = middlePos.position + new Vector3(0, 0, cardZ_Offset);
                        selectedDisplay.TweenToPosition(toPos);
                        cardZ_Offset += CARD_Z_OFFSET;
                        if (lastPlayed != null)
                        {
                            if (lastPlayedDisplay.card.number == selectedDisplay.card.number)
                            {
                                Debug.Log("HEYYYOOOOO!!");
                            }
                        }
                        lastPlayed = selected;
                        lastPlayedDisplay = selectedDisplay;
                        middleCount++;
                        middleCountText.text = middleCount.ToString();
                        selected.transform.SetParent(middlePos);
                        
                        phase = 2;
                    }
                    else
                    {
                        Debug.Log("Sıranı Bekle!!");
                        return;
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


    private void FixedUpdate()
    {
        if (selected != null && holdingCard)
        {
            selected.transform.position = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x,Input.mousePosition.y, camZ_Offset));
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


    private void SetHandPositions()
    {
        Vector3 topCenter = cam.ScreenToWorldPoint(new Vector3(cam.pixelWidth/2,cam.pixelHeight, camZ_Offset));
        Vector3 bottomCenter = cam.ScreenToWorldPoint(new Vector3(cam.pixelWidth / 2, 0, camZ_Offset));

        playerHand.transform.position = bottomCenter;
        botHand.transform.position = topCenter;

    }


    private void SetDeckPosition()
    {
        Vector3 deckPos = cam.ScreenToWorldPoint(new Vector3(cam.pixelWidth, cam.pixelHeight / 2, camZ_Offset));
        deckPosition.position = deckPos;
    }

    private void DealNewHands()
    {
        StartCoroutine(deckManager.DealHand(cardPrefab, handCount, playerHand, 1, deckPosition.position, bot));
        StartCoroutine(deckManager.DealHand(cardPrefab, handCount, botHand, -1, deckPosition.position, bot));
        phase = 1;
    }

    private void PhaseChange()
    {
        phase = 4;
    }
}
