using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PileView : MonoBehaviour
{
    public Pile Pile;
    public List<CardView> CardViews = new List<CardView>();

    GameManager gameManager;
    ViewManager cardViewManager;
    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        cardViewManager = FindObjectOfType<ViewManager>();
    }

    public void UpdatePileView()
    {
        DestroyChildren();
        // Get viewable cards count
        int firstShownCardIdex = 0;
        if (Pile.Type == Pile.PileType.talon) firstShownCardIdex = Mathf.Clamp(Pile.Cards.Count - 2, 0, Pile.Cards.Count - 1);
        else if (Pile.Type == Pile.PileType.foundation) firstShownCardIdex = Mathf.Clamp(firstShownCardIdex = Pile.Cards.Count - 2, 0, 13);
        else if (Pile.Type == Pile.PileType.hand) firstShownCardIdex = Pile.Cards.Count;

        for (int i = firstShownCardIdex; i < Pile.Cards.Count; i++)
        {
            CardView cardView = Instantiate(cardViewManager.CardPrefab, transform).GetComponent<CardView>();
            cardView.Card = Pile.Cards[i];
            if (Pile.Type == Pile.PileType.hand) cardView.Card.IsFaceUp = false;
            if (Pile.Type == Pile.PileType.talon) cardView.Card.IsFaceUp = true;
            cardView.UpdateCardView();
            CardViews.Add(cardView);
        }
    }

    void DestroyChildren()
    {
        List<GameObject> childList = new List<GameObject>();
        int childCount = transform.childCount;
        for (int i = 0; i < childCount; i++)
        {
            GameObject child = transform.GetChild(i).gameObject;
            childList.Add(child);
        }
        for (int i = 0; i < childCount; i++)
        {
            DestroyImmediate(childList[i]);
        }
    }

    public CardView CardToCardView(Card card)
    {
        foreach (CardView cardView in CardViews)
        {
            if (cardView.Card == card)
            {
                return cardView;
            }
        }
        return null;
    }

    public CardView CardToCardView(Card card, List<CardView> cardViews)
    {
        foreach (CardView cardView in cardViews)
        {
            if (cardView.Card == card)
            {
                return cardView;
            }
        }
        return null;
    }

}
