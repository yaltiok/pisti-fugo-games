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

    public Player playerScript;
    public Bot botScript;

    public Transform middlePos;
    public GameObject cardPrefab;

    private int middleCount = 0;



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
        botScript.SetStashPos();
        playerScript.SetStashPos();

    }

    private void SetDeckPosition()
    {
        Vector3 deckPos = cam.ScreenToWorldPoint(new Vector3(cam.pixelWidth, cam.pixelHeight / 2, camZ_Offset));
        deckManager.deckPos = deckPos;
        //deckPosition.position = deckPos;
    }

    private void DealNewHands()
    {

        if (!CheckGameEnd())
        {
            if (phase == 0)
            {
                deckManager.CreateHand(cardPrefab, handCount, playerHand, 1);
                deckManager.CreateHand(cardPrefab, handCount, botHand, -1);
                ChangePhase(1);
            }
        }
    }
    private void DealMiddle()
    {

        lastPlayed = deckManager.CreateMiddleStack(cardPrefab, handCount, middlePos);
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
        ChangePhase(4);
        Vector3 z_Offset = new Vector3(0, 0, cardZ_Offset);
        Vector3 randomize = new Vector3(Random.Range(-.2f, .2f), Random.Range(-.2f, .2f),0);
        Vector3 toPos = middlePos.position + z_Offset + randomize;
        selected.transform.position += z_Offset;
        nextPhase = ControlRound(nextPhase);
        tweenManager.CardPlayTween(selected, toPos,length, nextPhase);
        cardZ_Offset += CARD_Z_OFFSET;
        selected.transform.SetParent(middlePos);
        middleCount++;
        middleCountText.text = middleCount.ToString();
        HandleMatches(player);

        
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
            botScript.PlayCard(lastPlayedDisplay);
        }
    }


    private void SetCamValues()
    {
        cam = Camera.main;
        camZ_Offset = -cam.transform.position.z;
    }

    private void CalculateCardPositions()
    {
        Vector3 leftBottom = cam.ScreenToWorldPoint(new Vector3(0, 0 , camZ_Offset));
        Vector3 rightBottom = cam.ScreenToWorldPoint(new Vector3(cam.pixelWidth, 0, camZ_Offset));

        float diff = (rightBottom.x - leftBottom.x);
        deckManager.leftBottom = leftBottom;
        deckManager.cardOffset = diff / handCount;

    }

    private bool CheckMatchings(int player)
    {
        if (lastPlayed != null)
        {
            if (lastPlayedDisplay.card.number == selectedDisplay.card.number)
            {
                //A kind of pisti
                if (middleCount == 2)
                {
                    // Big pisti
                    if (player == 1)
                    {
                        playerScript.turnPoint += 10;
                    }
                    else
                    {
                        botScript.turnPoint += 10;
                    }
                }
                return true;
            }else if (selectedDisplay.card.number == 11) // Jack played.
            {
                return true;
            }
        }
        return false;
    }

    private void HandleMatches(int player)
    {
        if (CheckMatchings(player))
        {
            CardDisplay[] middleCards = new CardDisplay[middleCount];
            GameObject[] middleObjects = new GameObject[middleCount];
            for (int i = 0; i < middleCount; i++)
            {
                middleObjects[i] = middlePos.GetChild(i).gameObject;
                middleCards[i] = middleObjects[i].GetComponent<CardDisplay>();
            }
            if (player == 1)
            {
                playerScript.AddToStash(middleObjects, middleCards);
            }
            else
            {
                botScript.AddToStash(middleObjects, middleCards);
            }
            middleCount = 0;
            middleCountText.text = middleCount.ToString();
            cardZ_Offset = -1;
            selected = null;
            lastPlayed = null;
            selectedDisplay = null;
            lastPlayedDisplay = null;
        }
        else
        {
            lastPlayed = selected;
            lastPlayedDisplay = selectedDisplay;
        }
    }


    private bool CheckGameEnd()
    {
        if (deckManager.currentDeck.Count <= 0)
        {

            int a = playerScript.stashCount;
            int b = botScript.stashCount;
            if (a > b)
            {
                playerScript.turnPoint += 3;
                playerScript.UpdateInfoText();
            }
            else if (a < b)
            {
                botScript.turnPoint += 3;
                botScript.UpdateInfoText();
            }
            string winner = "";
            if (playerScript.turnPoint > botScript.turnPoint)
            {
                winner = "player";
            }
            else
            {
                winner = "bot";
            }
            Debug.Log("Game Over! Winner is: " + winner);
            return true;
        }
        return false;
    }


}
