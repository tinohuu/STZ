using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UndoManager : MonoBehaviour
{
    CardManager cardManager;
    public List<Undo> Undos;
    ViewManager cardViewManager;
    private void Awake()
    {
        cardManager = FindObjectOfType<CardManager>();
        cardViewManager = FindObjectOfType<ViewManager>();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        GetComponentInChildren<Button>().interactable = Undos.Count != 0;
    }

    public void Undo()
    {
        if (cardViewManager.IsAnyCardAnimating() || Undos.Count == 0) return;

        Pile oriPile = cardManager.AllPiles[Undos.Last().OriPileIndex];
        Pile curPile = cardManager.AllPiles[Undos.Last().CurPileIndex];

        Card card = Undos.Last().Card;

        // Flip down the last card
        if (!Undos.Last().IsLastCardUp && oriPile.Cards.Count > 0)
        {
            oriPile.Cards.Last().IsFaceUp = false;
        }

        List<Card> cards = new List<Card>();
        //List<Vector3> newPositions = new List<Vector3>();
        //List<bool> newFaceUps = new List<bool>();
        

        PileView oriPileView = cardViewManager.PileToPileView(oriPile);
        PileView newPileView = cardViewManager.PileToPileView(curPile);

        for (int i = curPile.Cards.IndexOf(card); i < curPile.Cards.Count; i++)
        {

            //Debug.LogWarning("NewPileIndex " + i);
            //Debug.LogWarning("Card " + card.Suit + card.Number);
            cards.Add(curPile.Cards[i]);
            //CardView cardView = newPileView.CardToCardView(curPile[i]);

            //newPositions.Add(cardView ? cardView.transform.position : newPileView.transform.position);
            //newFaceUps.Add(cardView ? cardView.Card.IsFaceUp : false);

            //if (oriPileView.PileType == PileView.Type.talon) curPile[i].IsFaceUp = true;

            //Sphere.transform.position = newPileView.CardToCardView(newPile[i]).transform.position;
        }

        if (Undos.Last().IsHandCard)
        {
            oriPile.Cards.InsertRange(0, cards);
        }
        else
        {
            oriPile.Cards.AddRange(cards);
        }

        curPile.Cards.RemoveRange(curPile.Cards.IndexOf(card), curPile.Cards.Count - curPile.Cards.IndexOf(card));



        
        //Debug.LogWarning("Test3");

        for (int i = 0; i < cards.Count; i++)
        {
            CardView cardView = oriPileView.CardToCardView(cards[i]);
            if (!cardView)
            {
                cardView = Instantiate(cardViewManager.CardPrefab, FindObjectOfType<Canvas>().transform).GetComponent<CardView>();
                cardView.transform.position = oriPileView.transform.position;
                cardView.Card = cards[i];
                cardView.Card.IsFaceUp = false;
                cardView.IsPlaceholder = true;
                //Debug.LogWarning("New");
            }
            //cardView.OriViewPos = newPositions[i];
            //cardView.SetOriViewPosData(newPositions[i]);
            //Sphere.transform.position = newPositions[i];
        }

        //Debug.LogWarning("Test4");
        oriPileView.UpdatePileView();
        newPileView.UpdatePileView();
        Undos.RemoveAt(Undos.Count - 1);
        FindObjectOfType<GameManager>().Moves++;
    }
}

[System.Serializable]
public struct Undo
{
    public int OriPileIndex;
    public int CurPileIndex;
    public bool IsLastCardUp;
    public bool IsHandCard;
    public Card Card;
    List<List<Card>> OldPiles;
    public Undo(Card card, int oriPileIndex, int newPileIndex, bool isLastCardUp = true, bool isHandCard = false, List<List<Card>> oldPiles = null)
    {
        Card = card;
        OriPileIndex = oriPileIndex;
        CurPileIndex = newPileIndex;
        IsLastCardUp = isLastCardUp;
        OldPiles = oldPiles;
        IsHandCard = isHandCard;
    }
}