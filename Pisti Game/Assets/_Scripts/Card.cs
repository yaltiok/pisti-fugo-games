using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Card
{
    public int number;
    public int value;
    public Sprite suit;
    private Sprite numberSprite;


    public Card(int number, int value, Sprite suit)
    {
        this.number = number;
        this.value = value;
        this.suit = suit;



        

    }
}
