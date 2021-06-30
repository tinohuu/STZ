using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckSkin : MonoBehaviour
{
    [Header("Settings")]
    public int Id = 0;
    public string Name = "NewDeckSkin";

    public Sprite BoxSprite;
    public Sprite FaceSprite;
    public Sprite BackSprite;
    public Sprite CoverSprite;

    public Color RedColor = Color.red;
    public Color BlackColor = Color.black;

    public bool CanTintSmallSuit = false;
    public bool CanTintBigSuit = false;

    [Header("Assets")]
    public List<Sprite> NumberSprites = new List<Sprite>();
    public List<Sprite> SmallSuitSprites = new List<Sprite>();
    public List<Sprite> BigSpadeSprites = new List<Sprite>();
    public List<Sprite> BigDiamondSprites = new List<Sprite>();
    public List<Sprite> BigHeartSprites = new List<Sprite>();
    public List<Sprite> BigClubSprites = new List<Sprite>();

    public List<List<Sprite>> BigSuitSprites
    {
        get
        {
            List<List<Sprite>> big = new List<List<Sprite>>();
            big.Add(BigSpadeSprites);
            big.Add(BigDiamondSprites);
            big.Add(BigHeartSprites);
            big.Add(BigClubSprites);
            return big;
        }
    }
}
