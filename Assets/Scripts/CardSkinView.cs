using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CardView))]
public class CardSkinView : MonoBehaviour
{
    public DeckSkin PreviewSkin = null;
    SkinManager skinManager;
    CardView cardView = null;
    private void Awake()
    {
        skinManager = FindObjectOfType<SkinManager>();
        cardView = GetComponent<CardView>();
        if (cardView)
        {
            cardView.UpdateViewDelegate += new CardView.UpdateView(UpdateView);
            cardView.UpdateViewDelegate += new CardView.UpdateView(UpdateBack);
        }

    }

    // Start is called before the first frame update
    void Start()
    {
        if (cardView) cardView.UpdateViewDelegate -= new CardView.UpdateView(UpdateView);
    }

    // Update is called once per frame
    void Update()
    {
    }

    void UpdateBack()
    {
        if (cardView.PileView && cardView.PileView.Pile.Type == Pile.PileType.hand && cardView.Card == cardView.PileView.Pile.Cards.Last())
            cardView.Back.sprite = skinManager.CurDeckSkin.CoverSprite;
        else
            cardView.Back.sprite = skinManager.CurDeckSkin.BackSprite;
    }

    public void UpdateView()
    {
        DeckSkin deckSkin = PreviewSkin ? PreviewSkin : skinManager.CurDeckSkin;
        // Display by image
        if (deckSkin)
        {
            /*bool isRed = Card.Color == Color.red;
            NumberImage.sprite = skinManager.GetNumberSprite(Card.Number);
            NumberImage.color = isRed ? skinManager.CurDeckSkin.RedColor : skinManager.CurDeckSkin.Blackolor;

            bool canTintSmall= true;
            SmallSuitImage.sprite = skinManager.GetSmallSuiteSprite(Card.Suit, out canTintSmall);
            if (canTintSmall) SmallSuitImage.color = isRed ? skinManager.CurDeckSkin.RedColor : skinManager.CurDeckSkin.Blackolor;
            else SmallSuitImage.color = Color.white;

            bool canTintBig = true;
            BigSuitImage.sprite = skinManager.GetBigSuiteSprite(Card, out canTintBig);
            if (!BigSuitImage.sprite) BigSuitImage.sprite = SmallSuitImage.sprite;
            if (canTintBig) BigSuitImage.color = isRed ? skinManager.CurDeckSkin.RedColor : skinManager.CurDeckSkin.Blackolor;
            else BigSuitImage.color = Color.white;

            Back.sprite = skinManager.CurDeckSkin.BackSprite;
            Back.color = Color.white;
            Face.sprite = skinManager.CurDeckSkin.FaceSprite;*/

            bool isRed = cardView.Card.Color == Color.red;
            cardView.NumberImage.sprite = deckSkin.NumberSprites[cardView.Card.Number - 1];
            cardView.NumberImage.color = isRed ? deckSkin.RedColor : deckSkin.BlackColor;

            cardView.SmallSuitImage.sprite = deckSkin.SmallSuitSprites[(int)cardView.Card.Suit];
            if (deckSkin.CanTintSmallSuit) cardView.SmallSuitImage.color = isRed ? deckSkin.RedColor : deckSkin.BlackColor;
            else cardView.SmallSuitImage.color = Color.white;

            cardView.BigSuitImage.sprite = deckSkin.BigSuitSprites[(int)cardView.Card.Suit][cardView.Card.Number - 1];
            if (deckSkin.CanTintBigSuit) cardView.BigSuitImage.color = isRed ? deckSkin.RedColor : deckSkin.BlackColor;
            else cardView.BigSuitImage.color = Color.white;

            cardView.Back.sprite = deckSkin.BackSprite;
            cardView.Back.color = Color.white;
            cardView.Face.sprite = deckSkin.FaceSprite;

            cardView.Alpha = 0;
        }
    }
}
