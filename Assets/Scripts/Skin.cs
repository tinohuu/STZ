using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skin : MonoBehaviour
{
    public int Durability = 0;

    public Sprite BoxSprite;
    public Sprite FaceSprite;
    public Sprite BackSprite;
    public Sprite HandBackSprite;

    public Color RedColor = Color.white;
    public Color Blackolor = Color.white;

    public List<NumberSprite> NumberSprites;
    public List<SmallSprite> SmallSuitSprites;
    public List<BigSprite> BigSuitSprites;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

[System.Serializable]
public class NumberSprite
{
    public int Number = 1;
    public Sprite Sprite;
}

[System.Serializable]
public class SmallSprite
{
    public Card.SuitType Suit = Card.SuitType.spades;
    public Sprite Sprite;
}

[System.Serializable]
public class BigSprite
{
    public int Number = 1;
    public Card.SuitType Suit = Card.SuitType.spades;
    public Sprite Sprite;
    public bool CanTint = true;
}
