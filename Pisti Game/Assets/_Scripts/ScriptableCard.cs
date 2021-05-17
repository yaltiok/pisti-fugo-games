using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Card")]
public class ScriptableCard : ScriptableObject
{
    public string cardName;
    public int number;
    public int value;
    public Sprite suit;
    public Sprite cardBack;
    public int orientation = 0;
    public bool isRed;

}
