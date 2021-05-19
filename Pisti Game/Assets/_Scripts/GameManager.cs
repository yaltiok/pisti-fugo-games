using UnityEngine;
using TMPro;
using System.Collections;
public class GameManager : MonoBehaviour
{


    private const float DEAL_TIME = 0.5f;
    private const float DEAL_FACTOR = .5f;


    private const float CARD_Z_OFFSET = -0.1f;

    private const float CARD_GAP = 0.2f;
    public int handCount = 4;
    public int pointToWin = 151;
    private int playerCount = 2;

    private int roundControl = 0;
    private int roundMax;


    public GameObject playerHand;
    public GameObject botHand;
    public DeckManager deckManager;
    public TweenManager tweenManager;
    public TMP_Text middleCountText;
    public TMP_Text roundText;

    public Player playerScript;
    public Bot botScript;

    public Transform middlePos;
    public GameObject cardPrefab;
    public GameObject defaultSprite;
    public GameObject endGamePanel;
    public TMP_Text winnerTextElement;

    private SpriteRenderer defaultRenderer;

    private float cardWidth;
    private float cardHeight;


    private int middleCount = 0;

    private bool firstRound = true;
    private bool roundEnd = false;



    private float camZ_Offset;
    private float cardZ_Offset = -1f;
    private Vector3 leftBottom;
    private Vector3 rightBottom;
    private float screenWidthInWorld;


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
        CalculateCardPositions(handCount);
        StartDeal();
        playerScript.lastCardZ = cardZ_Offset;
        

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
    }

    public void DealNewHands()
    {
        
        
        if (!CheckRoundEnd())
        {
            if (phase == 0)
            {
                StartDeal();
            }
        }
        else
        {
            NewRound();
        }
    }


    public void StartDeal()
    {
        StartCoroutine(DealHands());
    }

    private void NewRound()
    {
        playerScript.RoundEnd();
        botScript.RoundEnd();

        if (CheckGameEnd())
        {
            endGamePanel.SetActive(true);
            return;
        }

        deckManager.CreateNewDeck();
        tweenManager.TextTween(roundText, 1f, 1f);
        
        firstCard = true;
        firstRound = true;
        roundEnd = false;
        
    }
    
    
    private void DealMiddle()
    {
        lastPlayed = deckManager.CreateMiddleStack(cardPrefab, handCount, middlePos);
        lastPlayedDisplay = lastPlayed.GetComponent<CardDisplay>();
        InformBot();
        middleCount = 4;
        middleCountText.text = middleCount.ToString();
        firstRound = false;

    }

    public IEnumerator DealHands()
    {

        deckManager.CreateHand(cardPrefab, handCount, playerHand, 1, CARD_GAP);
        yield return new WaitForSeconds(handCount * DEAL_TIME * DEAL_FACTOR);
        deckManager.CreateHand(cardPrefab, handCount, botHand, -1, CARD_GAP);
        yield return new WaitForSeconds(handCount * DEAL_TIME * DEAL_FACTOR);
        if (firstRound)
        {
            DealMiddle();
            yield return new WaitForSeconds(handCount * DEAL_TIME * DEAL_FACTOR);
        }
        CheckDeckCount();
        botScript.NewHand(deckManager.currentDeck.Count + playerScript.cardObjects.Count);
        ChangePhase(1);
        
    }

    private bool CheckGameEnd()
    {
        if (playerScript.totalPoint >= pointToWin || botScript.totalPoint >= pointToWin)
        {
            string winner = "Kazanan: ";
            if (playerScript.totalPoint >= botScript.totalPoint)
            {
                winnerTextElement.text = winner + "Siz!";
            }
            else
            {
                winnerTextElement.text = winner + "Bot!";
            }
            return true;
        }
        return false;
    }

    private void CheckDeckCount()
    {
        if (deckManager.currentDeck.Count <= 0)
        {
            deckManager.deck.SetActive(false);
        }
    }

    public void CardPlayed(float length, int nextPhase, int player)
    {
        RepositionHand(player);
        IsFirstCard();
        ChangePhase(4);
        
        nextPhase = ControlRound(nextPhase);
        bool match = CheckMatchings(player);
        SetTween(player, match, length, nextPhase);
        cardZ_Offset += CARD_Z_OFFSET;
        playerScript.lastCardZ = cardZ_Offset;
        selected.transform.SetParent(middlePos);
        middleCount++;
        middleCountText.text = middleCount.ToString();
        selectedDisplay.player = 0;
        selected.transform.tag = "Middle";
        HandleMatches(player, match, nextPhase);
        

        
    }

    private void IsFirstCard()
    {
        if (firstCard)
        {
            for (int i = 0; i < middlePos.childCount; i++)
            {
                GameObject obj = middlePos.GetChild(i).gameObject;
                Vector3 offset = new Vector3(0,0, obj.transform.position.z);
                Vector3 toPos = offset + middlePos.position;
                tweenManager.TweenToPoint(obj, toPos, 0.5f);
            }
            firstCard = false;
        }
    }

    private void SetTween(int player, bool match, float length, int nextPhase)
    {
        Vector3 z_Offset = new Vector3(0, 0, cardZ_Offset);
        Vector3 randomize = new Vector3(UnityEngine.Random.Range(-.25f, .25f), UnityEngine.Random.Range(-.25f, .25f), 0);
        Vector3 toPos = middlePos.position + z_Offset + randomize;
        selected.transform.position += z_Offset;
        if (match)
        {

            toPos.y += -player * cardHeight;
            tweenManager.TweenToPoint(selected, toPos, length);

        }
        else
        {
            tweenManager.CardPlayTween(selected, toPos, length, nextPhase);

        }
    }

    private void RepositionHand(int player)
    {
        if (player == 1)
        {
            playerScript.RepositionCards(cardWidth, CARD_GAP);
        }
        else
        {
            botScript.RepositionCards(cardWidth, CARD_GAP);
        }
    }

    private int ControlRound(int nextPhase)
    {
        roundControl++;
        if (roundControl >= roundMax)
        {
            roundControl = 0;
            return 0;
        }
        return nextPhase;
    }

    private void InformBot()
    {
        if (lastPlayedDisplay != null && !roundEnd)
        {
            botScript.InformBot(deckManager.currentDeck.Count + playerScript.cardObjects.Count, lastPlayedDisplay.card.number);
        }
    }

    private void ChangePhase(int i)
    {
        phase = i;
    }

    public void BotMove()
    {
        int midPoint = 0;
        for (int i = 0; i < middlePos.childCount; i++)
        {
            CardDisplay display = middlePos.GetChild(i).GetComponent<CardDisplay>();
            midPoint += display.card.value;
        }
        botScript.PlayCard(lastPlayedDisplay, middleCount, midPoint);
    }


    private void SetCamValues()
    {
        cam = Camera.main;
        camZ_Offset = -cam.transform.position.z;
        leftBottom = cam.ScreenToWorldPoint(new Vector3(0, 0, camZ_Offset));
        rightBottom = cam.ScreenToWorldPoint(new Vector3(cam.pixelWidth, 0, camZ_Offset));
        screenWidthInWorld = (rightBottom.x - leftBottom.x);
        botScript.screenWidth = screenWidthInWorld;

    }

    private void CalculateCardPositions(int cardCount)
    {
        defaultRenderer = defaultSprite.GetComponent<SpriteRenderer>();

        cardWidth = defaultRenderer.bounds.size.x;
        cardHeight = defaultRenderer.bounds.size.y;
        deckManager.leftBottom = leftBottom;
        deckManager.cardOffset = cardWidth;
    }

    private bool CheckMatchings(int player)
    {
        if (lastPlayed != null)
        {
            if (lastPlayedDisplay.card.number == selectedDisplay.card.number)
            {
                //A kind of pisti
                if (middleCount == 1)
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

    private void HandleMatches(int player, bool match, int nextPhase)
    {
        if (match)
        {
            MoveCardsToStash(player,nextPhase, .5f);
        }
        else
        {
            lastPlayed = selected;
            lastPlayedDisplay = selectedDisplay;
            if (player == 1)
            {
                InformBot();
            }

        }
    }

    private void MoveCardsToStash(int player, int nextPhase, float delay)
    {
        CardDisplay[] middleCards = new CardDisplay[middleCount];
        GameObject[] middleObjects = new GameObject[middleCount];
        lastPlayed = selected;
        lastPlayedDisplay = selectedDisplay;
        for (int i = 0; i < middleCount; i++)
        {
            middleObjects[i] = middlePos.GetChild(i).gameObject;
            middleCards[i] = middleObjects[i].GetComponent<CardDisplay>();
        }
        if (player == 1)
        {
            playerScript.AddToStash(middleObjects, middleCards, nextPhase, delay);
            //if (!roundEnd)
            //{
            //    InformBot();
            //}
            InformBot();
        }
        else
        {
            botScript.AddToStash(middleObjects, middleCards,nextPhase, delay);
        }
        middleCount = 0;
        middleCountText.text = middleCount.ToString();
        cardZ_Offset = -1;
        selected = null;
        lastPlayed = null;
        selectedDisplay = null;
        lastPlayedDisplay = null;
    }


    private bool CheckRoundEnd()
    {
        if (deckManager.currentDeck.Count <= 0)
        {
            roundEnd = true;
            int playerStashCount = playerScript.stashCount;
            int botStashCount = botScript.stashCount;
            if (playerStashCount >= botStashCount)
            {
                playerScript.turnPoint += 3;
                playerScript.UpdateInfoText();
                MoveCardsToStash(1,4, .7f);
            }
            else if (playerStashCount < botStashCount)
            {
                botScript.turnPoint += 3;
                botScript.UpdateInfoText();
                MoveCardsToStash(-1,4, .7f);

            }
            
            return true;
        }
        return false;
    }
}
