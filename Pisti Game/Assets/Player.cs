using System.Collections;
using TMPro;
using UnityEngine;

public class Player : MonoBehaviour
{
    Camera cam;
    private GameObject selected;
    private CardDisplay selectedDisplay;
    private bool holdingCard = false;

    public AreaHighlight areaHighlight;
    public GameManager gameManager;
    public TMP_Text infoText;
    public GameObject stashObject;
    private Vector3 stashPos;

    public ArrayList cardObjects = new ArrayList();
    public ArrayList cardDisplays = new ArrayList();
    public ArrayList stash = new ArrayList();


    private float camZ_Offset;
    private int holdIndex = -1;

    [HideInInspector]
    public int turnPoint = 0;
    [HideInInspector]
    public int totalPoint = 0;
    [HideInInspector]
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

    public void AddToStash(GameObject[] objects,CardDisplay[] displays)
    {
        for (int i = 0; i < displays.Length; i++)
        {
            turnPoint += displays[i].card.value;
            stashCount++;
        }
        UpdateInfoText();
        MoveToStash(objects, displays);
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


    public void UpdateInfoText()
    {
        string currentText = infoText.text;
        string[] arr = currentText.Split(char.Parse("|"));
        string temp = arr[0] + " | " + turnPoint + " | " + totalPoint;
        infoText.text = temp;
    }

    public void MoveToStash(GameObject[] objects, CardDisplay[] cardDisplays)
    {
        for (int i = 0; i < objects.Length; i++)
        {
            stash.Add(objects[i]);
            objects[i].transform.parent = stashObject.transform;
            cardDisplays[i].TweenToPosition(stashPos, 1f);

        }
    }

    public void SetStashPos()
    {
        stashPos = stashObject.transform.position;
    }

}
