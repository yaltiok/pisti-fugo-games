using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckHelper
{

    public Dictionary<int, int> values =
              new Dictionary<int, int>(){{1, 1},{2, 2},{3, 3},{4, 4},{5, 5},
                  {6, 6},{7, 7},{8, 8},{9, 9},{10, 10},{11, 11},{12, 12},{13, 13}};



    public Sprite GetRoyalSprite(Sprite[] royals, int suitIndex, int cardNumber)
    {
        int index = suitIndex + (cardNumber % 10) * 4; // 10 non-royal cards. 4 suits.
        return royals[index];
    }

}
