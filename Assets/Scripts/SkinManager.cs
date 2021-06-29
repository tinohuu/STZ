using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkinManager : MonoBehaviour
{
    public DeckSkin CurDeckSkin = null;

    public Sprite GetNumberSprite(int i)
    {
        foreach (NumberSprite numberSprite in CurDeckSkin.NumberSprites)
        {
            if (numberSprite.Number == i) return numberSprite.Sprite;
        }

        Debug.LogError("No sprite of the number.");
        return null;
    }

    public Sprite GetSmallSuiteSprite(Card.SuitType suitType)
    {
        foreach (SmallSuitSprite smallSuitSprite in CurDeckSkin.SmallSuitSprites)
        {
            if (smallSuitSprite.Suit == suitType) return smallSuitSprite.Sprite;
        }

        Debug.LogError("No sprite of the suit.");
        return null;
    }

    public Sprite GetBigSuiteSprite(Card card, out bool canTint)
    {
        foreach (BigSuitSprite bigSuitSprite in CurDeckSkin.BigSuitSprites)
        {
            if (bigSuitSprite.Suit == card.Suit && bigSuitSprite.Number == card.Number)
            {
                canTint = bigSuitSprite.CanTint;
                return bigSuitSprite.Sprite;
            }
        }

        Debug.LogError("No sprite of the card.");
        canTint = false;
        return null;
    }
}
