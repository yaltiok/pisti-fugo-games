using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckManager : MonoBehaviour
{

    public ScriptableCard[] cards;

    ArrayList currentDeck;

    private DeckHelper deckHelper = new DeckHelper();


    private float cardOffset = 1.4f;
    private float xStart = -2.1f;

    
    public GameObject cardPrefab;


    private void Awake()
    {
        CreateNewDeck();
    }

    void Start()
    {
    }

    void Update()
    {
        
    }

    public void DealHand(int count, GameObject hand, int botFactor)
    {
        for (int i = 0; i < count; i++)
        {
            ScriptableCard card = GetRandomCard();

            Vector3 cardPos = hand.transform.position + new Vector3(xStart + cardOffset * i, .5f * botFactor);
            GameObject cardObject = Instantiate(cardPrefab, cardPos, Quaternion.identity, hand.transform);

            card.orientation = botFactor;

            cardObject.gameObject.GetComponent<CardDisplay>().card = card;

        }
    }

    private ScriptableCard GetRandomCard()
    {
        int index = Random.Range(0, currentDeck.Count);
        ScriptableCard card = (ScriptableCard)currentDeck[index];
        currentDeck.RemoveAt(index);
        return card;
    }


    private void CreateNewDeck()
    {
        currentDeck = new ArrayList();
        for (int i = 0; i < cards.Length; i++)
        {
            currentDeck.Add(cards[i]);
        }
    }

}
