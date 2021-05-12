using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DeckManager : MonoBehaviour
{

    public ScriptableCard[] cards;

    ArrayList currentDeck;
    public TMP_Text textMeshPro;

    private DeckHelper deckHelper = new DeckHelper();

    private float cardOffset = 1.4f;
    private float xStart = -2.1f;


    private void Awake()
    {
        CreateNewDeck();
    }

    public IEnumerator DealHand(GameObject cardPrefab, int count, GameObject hand, int botFactor, Vector3 deckPos, Bot bot)
    {
        for (int i = 0; i < count; i++)
        {
            ScriptableCard card = GetRandomCard();

            Vector3 cardPos = hand.transform.position + new Vector3(xStart + cardOffset * i, .5f * botFactor);
            GameObject cardObject = Instantiate(cardPrefab, deckPos, Quaternion.identity, hand.transform);

            card.orientation = botFactor;

            CardDisplay display = cardObject.gameObject.GetComponent<CardDisplay>();
            display.positionInHand = cardPos;
            display.card = card;
            if (botFactor == -1)
            {
                bot.AddToCardObjects(cardObject);
            }
            yield return new WaitForSeconds(0.5f);

        }
    }

    private ScriptableCard GetRandomCard()
    {
        int index = Random.Range(0, currentDeck.Count);
        ScriptableCard card = (ScriptableCard)currentDeck[index];
        currentDeck.RemoveAt(index);
        textMeshPro.text = currentDeck.Count.ToString();
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
