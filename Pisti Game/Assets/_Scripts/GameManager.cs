using System;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{

    private const float CARD_Z_OFFSET = -0.1f;
    public int handCount = 4;
    private int playerCount = 2;

    private int roundControl = 0;
    private int roundMax;


    public GameObject playerHand;
    public GameObject botHand;
    public DeckManager deckManager;
    public TweenManager tweenManager;
    public AreaHighlight areaHighlight;
    public TMP_Text middleCountText;
    public TMP_Text playerInfo;
    public TMP_Text botInfo;

    public Player playerScript;
    public Bot botScript;

    public Transform middlePos;
    public Transform deckPosition;
    public GameObject cardPrefab;
    public Sprite defaultCardSprite;

    private int middleCount = 0;
    private float cardWidth;



    private float camZ_Offset;
    private float cardZ_Offset = -1f;


    [HideInInspector]
    public GameObject selected;
    [HideInInspector]
    public CardDisplay selectedDisplay;
    [HideInInspector]
    public GameObject lastPlayed;
    [HideInInspector]
    public CardDisplay lastPlayedDisplay;

    [HideInInspector]
    public bool holdingCard = false;
    private bool firstCard = true;

    
    [HideInInspector]
    public int phase = 0; //0: Deal new hands 1: Player's turn 2: Bot's turn 3: Collecting cards from middle. 4: Transition

    private Camera cam;

    void Start()
    {
        roundMax = handCount * playerCount;
        SetCamValues();
        SetHandPositions();
        SetDeckPosition();
        CalculateCardPositions();
        DealMiddle();
        DealNewHands();
    }

    

    void Update()
    {
        BotMove();
        //DealNewHands();
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
        if (deckManager.currentDeck.Count <= 0)
        {
            Debug.Log("Game Over! Winner is: " + "Winner!");
            return;
        }
        if (phase == 0)
        {
            deckManager.CreateHand(cardPrefab, handCount, playerHand, 1, deckPosition.position);
            deckManager.CreateHand(cardPrefab, handCount, botHand, -1, deckPosition.position);
            ChangePhase(1);
        }
    }
    private void DealMiddle()
    {

        lastPlayed = deckManager.CreateMiddleStack(cardPrefab, handCount, middlePos, deckPosition.position);
        lastPlayedDisplay = lastPlayed.GetComponent<CardDisplay>();
        middleCount = 4;
        middleCountText.text = middleCount.ToString();

    }

    public void CardPlayed(float length, int nextPhase, int player)
    {
        if (firstCard)
        {
            for (int i = 0; i < middlePos.childCount; i++)
            {
                tweenManager.TweenToPoint(middlePos.GetChild(i).gameObject,middlePos.position,0.5f);
            }
            firstCard = false;
        }

        phase = 4;
        Vector3 z_Offset = new Vector3(0, 0, cardZ_Offset);
        Vector3 toPos = middlePos.position + z_Offset;
        selected.transform.position += z_Offset;
        nextPhase = ControlRound(nextPhase);
        tweenManager.CardPlayTween(selected, toPos,length, nextPhase);
        cardZ_Offset += CARD_Z_OFFSET;
        selected.transform.SetParent(middlePos);
        middleCount++;
        middleCountText.text = middleCount.ToString();

        if (CheckMatchings())
        {
            CardDisplay[] middleCards = new CardDisplay[middleCount];
            for (int i = 0; i < middleCount; i++)
            {
                middleCards[i] = middlePos.GetChild(i).GetComponent<CardDisplay>();
            }
            if (player == 1)
            {
                playerScript.AddToStash(middleCards);
                UpdateInfoText(playerInfo, playerScript.turnPoint, playerScript.totalPoint);
            }
            else
            {
                botScript.AddToStash(middleCards);
                UpdateInfoText(botInfo, botScript.turnPoint, botScript.totalPoint);
            }

        }
        else
        {
            lastPlayed = selected;
            lastPlayedDisplay = selectedDisplay;
        }
    }

    private int ControlRound(int nextPhase)
    {
        roundControl++;
        if (roundControl >= roundMax)
        {
            phase = 0;
            DealNewHands();
            roundControl = 0;
            return 1;
        }
        return nextPhase;
    }


    private void ChangePhase(int i)
    {
        phase = i;
        if (i == 0)
        {
            DealNewHands();
        }
    }

    private void BotMove()
    {
        if (phase == 2)
        {
            int a = botScript.PlayCard();
            selected = (GameObject)botScript.cardObjects[a];
            selectedDisplay = (CardDisplay)botScript.cardDisplays[a];
            botScript.cardObjects.RemoveAt(a);
            botScript.cardDisplays.RemoveAt(a);
            selectedDisplay.SwitchOrientation();
            //ChangePhase(4);
            CardPlayed(.5f,1,-1);
        }
    }


    private void UpdateInfoText(TMP_Text textElement, int turnPoint, int totalPoint)
    {
        string temp = textElement.text;
        temp += turnPoint.ToString() + " | " + totalPoint.ToString(); 
    }


    private void SetCamValues()
    {
        cam = Camera.main;
        camZ_Offset = -cam.transform.position.z;
    }

    private void CalculateCardPositions()
    {
        cardWidth = defaultCardSprite.bounds.extents.x * 2;
        Vector3 leftBottom = cam.ScreenToWorldPoint(new Vector3(0, 0 , camZ_Offset));
        Vector3 rightBottom = cam.ScreenToWorldPoint(new Vector3(cam.pixelWidth, 0, camZ_Offset));

        float diff = (rightBottom.x - leftBottom.x);
        deckManager.leftBottom = leftBottom;
        deckManager.cardOffset = diff / handCount;

    }

    private bool CheckMatchings()
    {
        if (lastPlayed != null)
        {
            if (lastPlayedDisplay.card.number == selectedDisplay.card.number)
            {
                Debug.Log("HEYYYOOOOO!!");
                return true;
                //A kind of pisti
                if (middleCount == 1)
                {
                    // Big pisti
                }


            }
        }
        return false;
    }


}
