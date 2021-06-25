using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PileView : MonoBehaviour
{
    public Pile Pile;
    //public List<CardView> CardViews = new List<CardView>();

    GameManager gameManager;
    ViewManager viewManager;
    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        viewManager = FindObjectOfType<ViewManager>();
    }

    private void Start()
    {
        UpdatePileView();
    }

    public void CreateCardViews()
    {
        for (int i = 0; i < Pile.Cards.Count; i++)
        {
            CardView cardView = Instantiate(viewManager.CardPrefab, transform).GetComponent<CardView>();
            cardView.Card = Pile.Cards[i];
            viewManager.CardToCardView.Add(Pile.Cards[i], cardView);
            cardView.gameObject.SetActive(false);
            //CardViews.Add(cardView);
        }
    }

    public void UpdatePileView()
    {
        foreach (Card card in Pile.Cards)
        {
            CardView cardView = viewManager.CardToCardView[card];
            cardView.Alpha = 0;
            cardView.transform.SetParent(transform);
            cardView.transform.SetAsLastSibling();
            cardView.gameObject.SetActive(true);
            cardView.UpdateCardView(cardView.Image.transform.position);
        }
        //LayoutRebuilder.ForceRebuildLayoutImmediate(transform.GetComponent<RectTransform>());
        /*DestroyChildren();
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
        }*/
    }
}
