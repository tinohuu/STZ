using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkinManager : MonoBehaviour
{
    public DeckSkin CurDeckSkin = null;
    public Image HandCover = null;

    
    private void Start()
    {
        if (CurDeckSkin) HandCover.sprite = CurDeckSkin.CoverSprite;
    }
    /*
    public Sprite GetNumberSprite(int i)
    {
        foreach (NumberSprite numberSprite in CurDeckSkin.NumberSprites)
        {
            if (numberSprite.Number == i) return numberSprite.Sprite;
        }

        Debug.LogError("No sprite of the number.");
        return null;
    }

    public Sprite GetSmallSuiteSprite(Card.SuitType suitType, out bool canTint)
    {
        foreach (SmallSuitSprite smallSuitSprite in CurDeckSkin.SmallSuitSprites)
        {
            if (smallSuitSprite.Suit == suitType)
            {
                canTint = smallSuitSprite.CanTint;
                return smallSuitSprite.Sprite;
            }
        }

        Debug.LogError("No sprite of the suit.");
        canTint = true;
        return null;
    }

    public Sprite GetBigSuiteSprite(Card card, out bool canTint)
    {
        foreach (BigSuitSprite bigSuitSprite in CurDeckSkin.BigSuitSprites)
        {
            if ((bigSuitSprite.Suit == Card.SuitType.any || bigSuitSprite.Suit == card.Suit ) && bigSuitSprite.Number == card.Number)
            {
                canTint = bigSuitSprite.CanTint;
                return bigSuitSprite.Sprite;
            }
        }

        canTint = true;
        return null;
    }*/
}
