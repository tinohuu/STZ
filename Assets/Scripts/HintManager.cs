using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HintManager : MonoBehaviour
{
    ViewManager viewManager;
    GameManager gameManager;
    CardManager cardManager;
    public GameObject HintTextBackground;
    public Text HintText;
    List<CardView> OriCardViews = new List<CardView>();
    List<Vector3> TargetPositions = new List<Vector3>();

    List<CardView> HintCardViews = new List<CardView>();

    int hintIndex = 0;
    Canvas canvas;

    int hintMoves = 0;

    public static HintManager Instance;
    public GameManager.Handler OnHint = null;
    private void Awake()
    {
        Instance = this;
        viewManager = FindObjectOfType<ViewManager>();
        canvas = FindObjectOfType<Canvas>();
        gameManager = FindObjectOfType<GameManager>();
        cardManager = FindObjectOfType<CardManager>();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Clear hint when having a move
        if (HintCardViews.Count != 0 && hintMoves != gameManager.Moves) ClearHint();

        // Start a new hint
        if ((HintCardViews.Count == 0 || !HintCardViews[0]) && OriCardViews.Count > 0)
        {
            HintTextBackground.SetActive(true);
            HintCardViews.Clear();
            HintText.text = "Move " + (hintIndex + 1) + " of " + OriCardViews.Count;

            if (hintIndex < OriCardViews.Count)
            {
                CardView HintCardView = Instantiate(viewManager.CardPrefab, canvas.transform).GetComponent<CardView>();
                HintCardView.Alpha = 0;
                HintCardView.transform.position = TargetPositions[hintIndex];
                //HintCardView.Image.transform.position = OriCardViews[hintIndex].transform.position;
                HintCardView.Card = OriCardViews[hintIndex].Card;
                HintCardView.IsHint = true;
                HintCardView.UpdateCardView(OriCardViews[hintIndex].transform.position, 0.5f);
                HintCardViews.Add(HintCardView);

                PileView pileView = OriCardViews[hintIndex].PileView;
                if (pileView.Pile.Type == Pile.PileType.tableau)
                {
                    int index = pileView.Pile.Cards.IndexOf(OriCardViews[hintIndex].Card);
                    if (index != pileView.Pile.Cards.Count - 1)
                    {
                        for (int i = index + 1; i < pileView.Pile.Cards.Count; i++)
                        {
                            CardView cardView = Instantiate(viewManager.CardPrefab, canvas.transform).GetComponent<CardView>();
                            CardView oriCardView = viewManager.CardToCardView[pileView.Pile.Cards[i]];
                            cardView.transform.position = TargetPositions[hintIndex] + oriCardView.transform.position - OriCardViews[hintIndex].transform.position;
                            //cardView.OriViewPos = pileView.CardViews[i].transform.position;
                            //HintCardView.Image.transform.position = oriCardView.transform.position;
                            //HintCardView.SetOriViewFaceUpData(pileView.CardViews[i].Card.IsFaceUp);
                            cardView.Card = pileView.Pile.Cards[i];
                            cardView.IsHint = true;
                            cardView.UpdateCardView(oriCardView.transform.position, 0.5f);
                            HintCardViews.Add(cardView);
                        }
                    }
                }
            }
            else
            {
                ClearHint();
            }

            hintIndex++;
        }
    }

    public void Hint()
    {
        // Count moves
        hintMoves = gameManager.Moves;

        // Clear hints
        ClearHint();

        // Talon
        if (cardManager.Talon.Cards.Count > 0)
        {
            Card card = cardManager.Talon.Cards.Last();
            CardView ori = viewManager.CardToCardView[card];
            Vector3? target = GetDestPos(card, cardManager.Talon);
            if (target != null)
            {
                OriCardViews.Add(ori);
                TargetPositions.Add((Vector3)target);
            }
        }

        // First face-up card
        foreach (Pile pile in cardManager.Tableau)
        {
            Card card = null;
            foreach (Card c in pile.Cards)
            {
                if (c.IsFaceUp)
                {
                    card = c;
                    break;
                }
            }
            if (card == null) continue;

            Vector3? target = GetDestPos(card, pile);
            if (target != null)
            {
                OriCardViews.Add(viewManager.CardToCardView[card]);
                TargetPositions.Add((Vector3)target);
            }
        }

        // Last tableau card
        foreach (Pile pile in cardManager.Tableau)
        {
            if (pile.Cards.Count == 0) continue;

            Card card = pile.Cards.Last();

            if (OriCardViews.Contains(viewManager.CardToCardView[card])) continue;

            Vector3? target = GetDestPos(card, pile, true, false);
            if (target != null)
            {
                OriCardViews.Add(viewManager.CardToCardView[card]);
                TargetPositions.Add((Vector3)target);
            }
        }

        // Highlight hand card
        if (OriCardViews.Count == 0) FindObjectOfType<HandManager>().HighlightTimer = Time.time + 1;

        OnHint?.Invoke();
    }

    Vector3? GetDestPos(Card card, Pile pile, bool isToFoundation = true, bool isToTableau = true)
    {
        Pile destPile = null;
        PileView pileView = null;

        if (isToFoundation)
        {
            destPile = cardManager.GetFoundationDest(card, pile);
        }
        if (isToTableau && destPile == null)
        {
            destPile = cardManager.GetTableauDest(card, pile);
        }

        if (destPile != null)
        {
            pileView = viewManager.PileToPileView[destPile];
            if (destPile.Cards.Count == 0) return pileView.transform.position;
            else if (destPile.Type == Pile.PileType.tableau) return viewManager.CardToCardView[destPile.Cards.Last()].transform.position - Vector3.up * 40;
            else return viewManager.CardToCardView[destPile.Cards.Last()].transform.position;
        }

        return null;
    }

    /*
    Vector3? FindFoundationTarget(Card cardView)
    {
        if (cardView == cardView.PileView.CardViews.Last())
        {
            foreach (PileView pileView in viewManager.FoundationViews)
            {
                if (pileView.CardViews.Count == 0)
                {
                    if (cardView.Card.Number == 1) return pileView.transform.position;
                }
                else if (pileView.CardViews.Last().Card.Suit == cardView.Card.Suit
                    && pileView.CardViews.Last().Card.Number == cardView.Card.Number - 1)
                    return pileView.CardViews.Last().transform.position;
                else continue;
            }
        }
        return null;
    }

    Vector3? FindTableauTarget(CardView cardView)
    {
        foreach (PileView pileView in viewManager.TableauViews)
        {
            if (pileView.Pile.Cards.Count == 0)
            {
                if (cardView.Card.Number == 13 && cardView.PileView.CardViews[0] != cardView) return pileView.transform.position;
            }
            else if (pileView.CardViews.Last().Card.Color != cardView.Card.Color
                && pileView.CardViews.Last().Card.Number == cardView.Card.Number + 1)
                return pileView.CardViews.Last().transform.position + Vector3.up * -40;
        }
        return null;
    }*/

    void ClearHint()
    {
        HintTextBackground.SetActive(false);
        hintIndex = 0;
        OriCardViews.Clear();
        TargetPositions.Clear();

        if (HintCardViews.Count > 0)
        {
            for (int i = 0; i < HintCardViews.Count; i++)
            {
                if (HintCardViews[i]) Destroy(HintCardViews[i].gameObject);
            }
            HintCardViews.Clear();
        }
    }
}
