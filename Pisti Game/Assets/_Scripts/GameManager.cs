﻿using UnityEngine;
using TMPro;
using System.Collections;
using System;

public class GameManager : MonoBehaviour
{

    private const float DEAL_TIME = 0.2f;


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

    public Player playerScript;
    public Bot botScript;

    public Transform middlePos;
    public GameObject cardPrefab;
    public GameObject defaultSprite;
    private SpriteRenderer defaultRenderer;
    private float cardWidth;
    private float cardHeight;


    private int middleCount = 0;



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
        StartCoroutine(DealFirstHands());
        playerScript.lastCardZ = cardZ_Offset;

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log(phase);
        }
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

    private void DealNewHands()
    {

        if (!CheckGameEnd())
        {
            if (phase == 0)
            {
                StartCoroutine(DealHands());
            }
        }else if (playerScript.totalPoint >= pointToWin || botScript.totalPoint >= pointToWin)
        {
            // Game Over. Announce Winner
        }
        else
        {
            //New Round
        }
    }
    
    private void DealMiddle()
    {

        lastPlayed = deckManager.CreateMiddleStack(cardPrefab, handCount, middlePos);
        lastPlayedDisplay = lastPlayed.GetComponent<CardDisplay>();
        middleCount = 4;
        middleCountText.text = middleCount.ToString();

    }

    public IEnumerator DealFirstHands()
    {
        deckManager.CreateHand(cardPrefab, handCount, playerHand, 1, CARD_GAP);
        yield return new WaitForSeconds(handCount * DEAL_TIME);
        deckManager.CreateHand(cardPrefab, handCount, botHand, -1, CARD_GAP);
        yield return new WaitForSeconds(handCount * DEAL_TIME);
        DealMiddle();
        yield return new WaitForSeconds(handCount * DEAL_TIME);
        ChangePhase(1);

    }

    public IEnumerator DealHands()
    {
        deckManager.CreateHand(cardPrefab, handCount, playerHand, 1, CARD_GAP);
        yield return new WaitForSeconds(handCount * DEAL_TIME);
        deckManager.CreateHand(cardPrefab, handCount, botHand, -1, CARD_GAP);
        CheckDeckCount();
        yield return new WaitForSeconds(handCount * DEAL_TIME);
        ChangePhase(1);

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
        HandleMatches(player, match);

        
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

    public void BotMove()
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

    private void HandleMatches(int player, bool match)
    {
        if (match)
        {
            MoveCardsToStash(player);
        }
        else
        {
            lastPlayed = selected;
            lastPlayedDisplay = selectedDisplay;
        }
    }

    private void MoveCardsToStash(int player)
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


    private bool CheckGameEnd()
    {
        if (deckManager.currentDeck.Count <= 0)
        {
            int playerStashCount = playerScript.stashCount;
            int botStashCount = botScript.stashCount;
            if (playerStashCount >= botStashCount)
            {
                playerScript.turnPoint += 3;
                playerScript.UpdateInfoText();
                MoveCardsToStash(1);
            }
            else if (playerStashCount < botStashCount)
            {
                botScript.turnPoint += 3;
                botScript.UpdateInfoText();
                MoveCardsToStash(-1);

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
            Debug.Log("Round winner is: " + winner);
            return true;
        }
        return false;
    }
}
