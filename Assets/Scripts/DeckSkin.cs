using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckSkin : MonoBehaviour
{
    public string Name;

    public Sprite BoxSprite;
    public Sprite FaceSprite;
    public Sprite BackSprite;
    public Sprite HandBackSprite;

    public Color RedColor = Color.red;
    public Color Blackolor = Color.black;

    public List<NumberSprite> NumberSprites;
    public List<SmallSuitSprite> SmallSuitSprites;
    public List<BigSuitSprite> BigSuitSprites;
}

[System.Serializable]
public class NumberSprite
{
    public int Number = 1;
    public Sprite Sprite;
}

[System.Serializable]
public class SmallSuitSprite
{
    public Card.SuitType Suit = Card.SuitType.spades;
    public Sprite Sprite;
}

[System.Serializable]
public class BigSuitSprite
{
    public int Number = 1;
    public Card.SuitType Suit = Card.SuitType.spades;
    public Sprite Sprite;
    public bool CanTint = true;
}
