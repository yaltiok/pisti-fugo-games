using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public int handCount = 4;


    public GameObject playerHand;
    public GameObject botHand;

    public DeckManager deckManager;
    private bool a = true;

    private float rayTime = .5f;
    private float rayCounter = 0f;
    
    private int phase = 0; //0: Deal new hands 1: Player's turn 2: Bot's turn 3: Collecting cards from middle. 4: Transition

    private Camera cam;

    void Start()
    {
        cam = Camera.main;
        SetHandPositions();
        DealNewHands();
    }

    

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && a)
        {
            a = false;
            for (int i = 0; i < playerHand.transform.childCount; i++)
            {
                playerHand.transform.GetChild(i).GetComponent<CardDisplay>().SwitchOrientation();
            }
            for (int i = 0; i < botHand.transform.childCount; i++)
            {
                botHand.transform.GetChild(i).GetComponent<CardDisplay>().SwitchOrientation();
            }
        }



        




    }


    private void FixedUpdate()
    {
        if (rayCounter > rayTime)
        {
            RaycastHit2D hit = Physics2D.Raycast(cam.ScreenToWorldPoint(Input.mousePosition), cam.transform.forward);
            if (hit.collider != null)
            {
                Debug.Log(hit.transform.GetComponent<CardDisplay>().card.name);
            }
            rayCounter = 0f;
        }
        else
        {
            rayCounter += Time.fixedDeltaTime;
        }
    }


    private void SetHandPositions()
    {
        Vector3 topCenter = cam.ScreenToWorldPoint(new Vector3(cam.pixelWidth/2,cam.pixelHeight, -cam.transform.position.z));
        Vector3 bottomCenter = cam.ScreenToWorldPoint(new Vector3(cam.pixelWidth / 2, 0, -cam.transform.position.z));

        playerHand.transform.position = bottomCenter;
        botHand.transform.position = topCenter;

    }


    private void DealNewHands()
    {
        deckManager.DealHand(handCount, playerHand, 1);
        deckManager.DealHand(handCount, botHand, -1);
    }

}
