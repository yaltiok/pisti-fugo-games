using System.Collections;
using UnityEngine;
using TMPro;

public class DeckManager : MonoBehaviour
{

    private const float DEAL_TIME = 0.2f;

    public ScriptableCard[] cards;

    public ArrayList currentDeck;
    public TMP_Text deckTextElement;
    public GameObject deck;
    private int deckCount;
    public Vector3 deckPos;
    

    [HideInInspector]
    public float cardOffset = 1.4f;
    public Vector3 leftBottom;

    private void Awake()
    {
        CreateNewDeck();
    }

    private void Start()
    {
        deck.transform.position = deckPos;
    }


    public void CreateHand(GameObject cardPrefab, int count, GameObject hand, int botFactor, float gap)
    {
        ScriptableCard[] arr = new ScriptableCard[count];
        for (int i = 0; i < arr.Length; i++)
        {
            arr[i] = GetRandomCard();
        }
        ArrayList objects;
        ArrayList displays;


        if (botFactor == 1)
        {
            Player player = hand.GetComponent<Player>();
            objects = player.cardObjects;
            displays = player.cardDisplays;
        }
        else
        {
            Bot bot = hand.GetComponent<Bot>();
            objects = bot.cardObjects;
            displays = bot.cardDisplays;

        }

        
        for (int i = 0; i < arr.Length; i++)
        {
            ScriptableCard card = arr[i];
            Vector3 cardPos = hand.transform.position + new Vector3((-count / 2f + .5f + i) * cardOffset + (i - ((count - 1) / 2f)) * gap, .5f * botFactor);
            GameObject cardObject = Instantiate(cardPrefab, deckPos + new Vector3(cardOffset,0,0), Quaternion.identity, hand.transform);

            card.orientation = botFactor;

            CardDisplay display = cardObject.gameObject.GetComponent<CardDisplay>();
            display.positionInHand = cardPos;
            display.card = card;
            display.player = botFactor;
            objects.Add(cardObject);
            displays.Add(display);

        }
        CardDisplay[] temp = CustomToArray(displays);
        StartCoroutine(DealHand(temp));
    }
    

    

    public IEnumerator DealHand(CardDisplay[] arr)
    {
        for (int i = 0; i < arr.Length; i++)
        {
            arr[i].TweenToPosition(arr[i].positionInHand);
            deckCount--;
            deckTextElement.text = deckCount.ToString();
            yield return new WaitForSeconds(DEAL_TIME);
        }
    }


    public GameObject CreateMiddleStack(GameObject cardPrefab, int count, Transform hand)
    {

        ScriptableCard[] arr = new ScriptableCard[count];
        CardDisplay[] temp = new CardDisplay[count];
        for (int i = 0; i < arr.Length; i++)
        {
            arr[i] = GetRandomCard();
        }
        GameObject last = null;
        for (int i = 0; i < arr.Length; i++)
        {
            ScriptableCard card = arr[i];

            Vector3 cardPos = hand.position + new Vector3((cardOffset / 4) * -i + cardOffset / 2, 0f, 0.1f * (count - i));
            GameObject cardObject = Instantiate(cardPrefab, deckPos + new Vector3(cardOffset, 0, 0), Quaternion.identity, hand.transform);

            CardDisplay display = cardObject.gameObject.GetComponent<CardDisplay>();
            display.positionInHand = cardPos;


            if (i != count - 1)
            {
                card.orientation = -1;
            }
            else
            {
                card.orientation = 1;
                last = cardObject;

            }
            display.card = card;
            display.player = 0;
            cardObject.transform.tag = "Middle";
            temp[i] = display;

        }
        StartCoroutine(DealMiddle(temp));
        return last;

    }

    public IEnumerator DealMiddle(CardDisplay[] arr)
    {
        for (int i = 0; i < arr.Length; i++)
        {
            arr[i].TweenToPosition(arr[i].positionInHand);
            deckCount--;
            deckTextElement.text = deckCount.ToString();
            yield return new WaitForSeconds(DEAL_TIME);

        }

    }

    private ScriptableCard GetRandomCard()
    {
        int index = Random.Range(0, currentDeck.Count);
        ScriptableCard card = (ScriptableCard)currentDeck[index];
        currentDeck.RemoveAt(index);
        return card;
    }


    public void CreateNewDeck()
    {
        currentDeck = new ArrayList();
        for (int i = 0; i < cards.Length; i++)
        {
            currentDeck.Add(cards[i]);
        }
        deckCount = currentDeck.Count;
        deck.SetActive(true);
        deckTextElement.text = deckCount.ToString();
        ShuffleDeck();
    }
    private void ShuffleDeck()
    {
        for (int i = 0; i < currentDeck.Count; i++)
        {
            ScriptableCard tempCard = (ScriptableCard)currentDeck[i];
            int randomIndex = Random.Range(i,currentDeck.Count);
            currentDeck[i] = currentDeck[randomIndex];
            currentDeck[randomIndex] = tempCard;
        }
    }

    private CardDisplay[] CustomToArray(ArrayList list)
    {
        CardDisplay[] arr = new CardDisplay[list.Count];
        for (int i = 0; i < arr.Length; i++)
        {
            arr[i] = (CardDisplay)list[i];
        }
        return arr;
    }

}
