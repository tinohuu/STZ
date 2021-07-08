using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardManager : MonoBehaviour
{
    public List<Card> Deck = new List<Card>();

    public Pile Hand;
    public Pile Talon;
    public Pile[] Tableau = new Pile[7];
    public Pile[] Foundations = new Pile[4];

    public List<Pile> AllPiles;
    //public List<Card> Hand = new List<Card>();
    //public List<Card> Talon = new List<Card>();
    //public List<Card>[] Tableau = new List<Card>[7];
    //public List<Card>[] Foundations = new List<Card>[4];
    //public List<List<Card>> AllPiles = new List<List<Card>>(13);

    public List<Card> Dragged = new List<Card>();

    public static CardManager Instance;
    void Awake()
    {
        Instance = this;
        if (GameManager.Instance.Save == null) SetupStart();
        else SetupSaved();
    }

    public void SetupStart()
    {
        // Create foundations
        for (int i = 0; i < 4; i++) Foundations[i] = new Pile(Pile.PileType.foundation, new List<Card>());

        // Create a deck of cards
        for (int suit = 0; suit < Foundations.Length; suit++)
        {
            for (int number = 0; number < 13; number++)
            {
                Deck.Add(new Card((Card.SuitType)suit, number + 1));
            }
        }
        Shuffle(Deck);

        // Deal to tableau from hand
        Hand = new Pile(Pile.PileType.hand, new List<Card>(Deck));
        Talon = new Pile(Pile.PileType.talon, new List<Card>());
        for (int i = 0; i < Tableau.Length; i++)
        {
            Tableau[i] = new Pile(Pile.PileType.tableau, new List<Card>());
            for (int j = 0; j < i + 1; j++)
            {
                if (j == i) Hand.Cards[0].IsFaceUp = true;
                Tableau[i].Cards.Add(Hand.Cards[0]);
                Hand.Cards.RemoveAt(0);
            }
        }

        // Store all piles
        AllPiles.Add(Hand);
        AllPiles.Add(Talon);
        for (int i = 0; i < Tableau.Length; i++)
        {
            AllPiles.Add(Tableau[i]);
        }
        for (int i = 0; i < Foundations.Length; i++)
        {
            AllPiles.Add(Foundations[i]);
        }
    }
    public void SetupSaved()
    {
        // Create pile
        Hand = new Pile(Pile.PileType.hand, null);
        Talon = new Pile(Pile.PileType.talon, null);
        for (int i = 0; i < Tableau.Length; i++) Tableau[i] = new Pile(Pile.PileType.tableau, null);
        for (int i = 0; i < Foundations.Length; i++) Foundations[i] = new Pile(Pile.PileType.foundation, null);

        // Store all piles
        AllPiles.Add(Hand);
        AllPiles.Add(Talon);
        for (int i = 0; i < Tableau.Length; i++)
        {
            AllPiles.Add(Tableau[i]);
        }
        for (int i = 0; i < Foundations.Length; i++)
        {
            AllPiles.Add(Foundations[i]);
        }

        List<List<Card>> piles = GameManager.Instance.Save.Piles;
        for (int i = 0; i < piles.Count; i++)
        {
            Deck.AddRange(piles[i]);
            AllPiles[i].Cards = piles[i];
        }
    }
    public void ShuffleAll()
    {
        Shuffle(Deck);
        foreach (Card card in Deck) card.IsFaceUp = false;
        Hand.Cards.Clear();
        Talon.Cards.Clear();
        Hand.Cards.AddRange(Deck);
        for (int i = 0; i < Tableau.Length; i++)
        {
            Tableau[i].Cards.Clear();
            //Tableau[i] = new Pile(Pile.PileType.tableau, new List<Card>());
            for (int j = 0; j < i + 1; j++)
            {

                if (j == i) Hand.Cards[0].IsFaceUp = true;
                Tableau[i].Cards.Add(Hand.Cards[0]);
                Hand.Cards.RemoveAt(0);
            }
        }
        foreach (Pile pile in Foundations) pile.Cards.Clear();
    }

    void Shuffle(List<Card> cards)
    {
        for (int i = 0; i < cards.Count; i++)
        {
            Card temp = cards[i];
            int randomIndex = Random.Range(0, cards.Count);
            cards[i] = cards[randomIndex];
            //cards[i].IsFaceUp = false;
            cards[randomIndex] = temp;
            //cards[randomIndex].IsFaceUp = false;

        }
    }

    public int ShuffleFaceDown()
    {
        // Collect all face-down cards
        List<Card> flippedCards = new List<Card>();

        Hand.Cards.AddRange(Talon.Cards);
        Talon.Cards.Clear();
        flippedCards.AddRange(Hand.Cards);
        foreach (Pile pile in Tableau)
        {
            foreach (Card card in pile.Cards)
            {
                if (!card.IsFaceUp) flippedCards.Add(card);
            }
        }

        Shuffle(flippedCards);

        // Deal
        for(int i = 0; i < Hand.Cards.Count; i++)
        {
            Hand.Cards[i] = flippedCards[i];
            flippedCards[i].IsFaceUp = false;
        }

        int flippedIndex = Hand.Cards.Count;
        foreach (Pile pile in Tableau)
        {
            for (int cardIdx = 0; cardIdx < pile.Cards.Count; cardIdx++)
            {
                if (!pile.Cards[cardIdx].IsFaceUp)
                {
                    pile.Cards[cardIdx] = flippedCards[flippedIndex];
                    pile.Cards[cardIdx].IsFaceUp = false;
                    flippedIndex++;
                }
            }
        }

        return flippedCards.Count;
    }

    public void UpdateData(Card card, Pile oldPile, Pile newPile)
    {
        oldPile.Cards.Remove(card);
        newPile.Cards.Add(card);
    }

    public Pile GetFoundationDest(Card card, Pile pile, bool ignoreRules = false)
    {
        if (card != pile.Cards.Last() && !ignoreRules) return null;
        foreach (Pile foundationPile in Foundations)
        {
            if (foundationPile.Cards.Count == 0)
            {
                if (card.Number == 1) return foundationPile;
            }
            else if (foundationPile.Cards.Last().Suit == card.Suit && foundationPile.Cards.Last().Number == card.Number - 1)
                return foundationPile;
        }
        return null;
    }

    public Pile GetTableauDest(Card card, Pile pile)
    {
        if (card.Number == 13 && card == pile.Cards[0]) return null;
        foreach (Pile tableauPile in Tableau)
        {
            if (tableauPile.Cards.Count == 0)
            {
                if (card.Number == 13 && !tableauPile.Cards.Contains(card)) return tableauPile;
            }
            else if (tableauPile.Cards.Last().Color != card.Color && tableauPile.Cards.Last().Number == card.Number + 1)
                return tableauPile;
        }
        return null;
    }

    public Card SavedCardToCard(SavedCard savedCard)
    {
        foreach (Card card in Deck)
        {
            if (savedCard.Suit == card.Suit && savedCard.Number == card.Number) return card;
        }
        return null;
    }

    public bool IsAllFaceUp
    {
        get
        {
            //if (Hand.Cards.Count != 0) return false;
            foreach (Pile pile in Tableau) if (pile.Cards.Count > 0 && !pile.Cards[0].IsFaceUp) return false;
            return true;
        }
    }
}

[System.Serializable]
public class Card
{
    public enum SuitType { spade, heart, diamond, club, any }
    public SuitType Suit;
    public int Number;
    public bool IsFaceUp;

    public Card(SuitType suitType, int number, bool isUp = false)
    {
        Suit = suitType;
        Number = number;
        IsFaceUp = isUp;
    }

    public Color Color
    {
        get { return (Suit == SuitType.heart || Suit == SuitType.diamond) ? Color.red : Color.black; }
    }
}

[System.Serializable]
public struct SavedCard
{
    public Card.SuitType Suit;
    public int Number;
    public bool IsFaceUp;

    public SavedCard(Card.SuitType suitType, int number, bool isUp = false)
    {
        Suit = suitType;
        Number = number;
        IsFaceUp = isUp;
    }
}

[System.Serializable]
public class Pile
{
    public enum PileType { hand, talon, foundation, tableau, dragged }
    public PileType Type;
    public List<Card> Cards;

    public Pile (PileType type, List<Card> cards)
    {
        Type = type;
        Cards = cards;
    }
}
