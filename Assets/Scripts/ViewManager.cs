using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ViewManager : MonoBehaviour
{
    public Transform TableauArea;
    public Transform FoundationArea;
    //GameObject CardPrefab;
    public GameObject PilePrefab;
    public GameObject CardPrefab;
    public List<PileView> TableauViews;
    public List<PileView> FoundationViews;
    public PileView HandView;
    public PileView TalonView;

    //public List<CardView> TempCardViews = new List<CardView>();
    CardManager cardManager;
    UndoManager undoManager;
    GameManager gameManager;
    PileView DraggedFrom;
    public List<CardView> DraggedCardViews = new List<CardView>();
    public Dictionary<Card, CardView> CardToCardView = new Dictionary<Card, CardView>();
    public Dictionary<Pile, PileView> PileToPileView = new Dictionary<Pile, PileView>();
    private void Awake()
    {
        cardManager = FindObjectOfType<CardManager>();
        gameManager = FindObjectOfType<GameManager>();
        undoManager = FindObjectOfType<UndoManager>();
    }
    void Start()
    {
        // Initialise tableau pile views
        for (int i = 0; i < cardManager.Tableau.Length; i++)
        {
            PileView pileView = Instantiate(PilePrefab, TableauArea).GetComponent<PileView>();
            pileView.Pile = cardManager.Tableau[i];
            pileView.CreateCardViews();
            TableauViews.Add(pileView);
            PileToPileView.Add(cardManager.Tableau[i], pileView);
        }

        // Initialise foundation pile views
        for (int i = 0; i < 4; i++)
        {
            PileView pileView = Instantiate(PilePrefab, FoundationArea).GetComponent<PileView>();
            pileView.Pile = cardManager.Foundations[i];
            pileView.GetComponent<VerticalLayoutGroup>().spacing = -pileView.GetComponent<RectTransform>().rect.height;
            pileView.CreateCardViews();
            FoundationViews.Add(pileView);
            PileToPileView.Add(cardManager.Foundations[i], pileView);
        }

        // Initialise talon pile views
        TalonView.Pile = cardManager.Talon;
        TalonView.CreateCardViews();
        TalonView.UpdatePileView();
        PileToPileView.Add(cardManager.Talon, TalonView);
        HandView.Pile = cardManager.Hand;
        HandView.CreateCardViews();
        HandView.UpdatePileView();
        PileToPileView.Add(cardManager.Hand, HandView);
    }


    void Update()
    {
        Rect screenRect = new Rect(0, 0, Screen.width, Screen.height);
        if (!screenRect.Contains(Input.mousePosition) || Input.GetMouseButtonUp(0)) OnPileEndDrag();
    }
    public void OnBeginDragCard(Card card)
    {
        if (IsAnyCardAnimating()) return;

        if (DraggedCardViews.Count > 0 && DraggedCardViews[0] )
        {
            OnPileEndDrag();
            return;
        }

        // Store original pile
        DraggedFrom = CardToCardView[card].PileView;

        // Initialise dragged pile
        for (int i = DraggedFrom.Pile.Cards.IndexOf(card); i < DraggedFrom.Pile.Cards.Count; i++)
        {
            CardView cardView = CardToCardView[DraggedFrom.Pile.Cards[i]];
            DraggedCardViews.Add(cardView);
            cardView.OverrideSorting(true, i);
        }
    }
    public void OnPileDrag(Vector3 pos)
    {
        if (DraggedCardViews.Count == 0 || !DraggedCardViews[0]) return;
        
        if (DraggedCardViews.Count == 1)
        {
            DraggedCardViews[0].Image.transform.position = pos;
        }
        else
        {
            Vector3 offset = DraggedCardViews[1].Image.transform.position - DraggedCardViews[0].Image.transform.position;
            DraggedCardViews[0].Image.transform.position = pos;
            for (int i = 1; i < DraggedCardViews.Count; i++)
            {
                DraggedCardViews[i].Image.transform.position = DraggedCardViews[i - 1].Image.transform.position + offset;
            }
        }
    }

    public void OnPileEndDrag()
    {
        if (DraggedCardViews.Count == 0 || !DraggedCardViews[0]) return;



        List<PileView> AlternativePileViews = new List<PileView>();

        // Foundation
        if (DraggedCardViews.Count == 1)
        {
            for (int i = 0; i < FoundationViews.Count; i++)
            {
                if (cardManager.Foundations[i].Cards.Count == 0)
                {
                    if (DraggedCardViews[0].Card.Number == 1)
                    {
                        AlternativePileViews.Add(FoundationViews[i]);
                        //DragToNew(FoundationViews[i]);
                        //return;
                    }
                }
                else if (cardManager.Foundations[i].Cards[0].Suit == DraggedCardViews[0].Card.Suit
                        && cardManager.Foundations[i].Cards.Count == DraggedCardViews[0].Card.Number - 1)
                {
                    AlternativePileViews.Add(FoundationViews[i]);
                    //DragToNew(FoundationViews[i]);
                    //return;
                }
            }
        }

        // Pile
        for (int i = 0; i < TableauViews.Count; i++)
        {
            if (TableauViews[i].Pile.Cards.Count == 0)
            {
                if (DraggedCardViews[0].Card.Number == 13)
                {
                    AlternativePileViews.Add(TableauViews[i]);
                    //DragToNew(PileViews[i]);
                    //return;
                }
            }
            else if (TableauViews[i].Pile != DraggedFrom.Pile
                && TableauViews[i].Pile.Cards.Last().Color != DraggedCardViews[0].Card.Color
                && TableauViews[i].Pile.Cards.Last().Number == DraggedCardViews[0].Card.Number + 1)
            {
                AlternativePileViews.Add(TableauViews[i]);
                //DragToNew(PileViews[i]);
                //return;
            }
        }

        if (AlternativePileViews.Count > 0)
        {
            float closestDistance = gameManager.SnapDistance;
            PileView closestPileView = AlternativePileViews[0];
            foreach (PileView pileView in AlternativePileViews)
            {
                float dis = Vector3.Distance(DraggedCardViews[0].Image.transform.position, pileView.Pile.Cards.Count > 0 ? CardToCardView[pileView.Pile.Cards.Last()].transform.position : pileView.transform.position);
                //Debug.LogWarning(dis);
                if (closestDistance > dis)
                {
                    closestDistance = dis;
                    closestPileView = pileView;
                    
                }

            }
            if (Vector3.Distance(DraggedCardViews[0].transform.position, DraggedCardViews[0].Image.transform.position) < 0.1f) closestPileView = AlternativePileViews[0];


            // Scoring
            if (DraggedFrom.Pile.Type == Pile.PileType.talon)
            {
                if (closestPileView.Pile.Type == Pile.PileType.foundation) gameManager.Score += 15;
                else if (closestPileView.Pile.Type == Pile.PileType.tableau) gameManager.Score += 10;
            }
            if (DraggedFrom.Pile.Type == Pile.PileType.foundation && closestPileView.Pile.Type == Pile.PileType.tableau) gameManager.Score -= 15;

            DragToNew(closestPileView);
            return;
        }

        // Return to original
        bool canShake = Vector3.Distance(DraggedCardViews[0].Image.transform.position, DraggedCardViews[0].transform.position) < 1;
        //Debug.LogWarning("Dis " + Vector3.Distance(DraggedCardViews[0].Image.transform.position, DraggedCardViews[0].transform.position) + " " + canShake.ToString());
        for (int i = 0; i < DraggedCardViews.Count; i++)
        {
            //DraggedCardViews[i].UpdateCardView(DraggedCardViews[i].transform.position);
            //if (DraggedCardViews[i])
            //    CardViewOldPos.Add(DraggedCardViews[i].Card, DraggedCardViews[i].Image.transform.position);
            //    AnimateCardInPileView(DraggedCardViews[i].Card, DraggedCardViews[i].Image.transform.position, OriDraggedPile);//  DraggedCardViews[i].CardToPileView(OriDraggedPile); //DraggedPile.CardViews[i].transform.SetParent(OriDraggedPile.transform);
            if (canShake) DraggedCardViews[i].GetComponentInChildren<Animator>().SetTrigger("Shake");
            //Debug.LogWarning("Shake");
        }
        DraggedFrom.UpdatePileView();
        DraggedCardViews.Clear();
    }

    void DragToNew(PileView newPile)
    {
        // Start to count game time
        if (gameManager.InitialTime == -1) gameManager.InitialTime = Time.time;
        // Count moves
        gameManager.Moves++;

        Card card = DraggedCardViews[0].Card;
        int index = DraggedFrom.Pile.Cards.IndexOf(card);
        bool isLastCardUp = index != 0 && DraggedFrom.Pile.Cards[index - 1].IsFaceUp;
        Undo undo = new Undo(DraggedCardViews[0].Card, cardManager.AllPiles.IndexOf(DraggedFrom.Pile), cardManager.AllPiles.IndexOf(newPile.Pile), isLastCardUp);
        undoManager.Undos.Add(undo);

        for (int j = 0; j < DraggedCardViews.Count; j++)
        {
            cardManager.UpdateData(DraggedCardViews[j].Card, DraggedFrom.Pile, newPile.Pile);
        }

        if (DraggedFrom.Pile.Cards.Count > 0)
        {
            if (DraggedFrom.Pile.Type == Pile.PileType.tableau && !DraggedFrom.Pile.Cards.Last().IsFaceUp)
                gameManager.Score += 5;
            DraggedFrom.Pile.Cards.Last().IsFaceUp = true;
        }

        DraggedFrom.UpdatePileView();
        newPile.UpdatePileView();
        DraggedCardViews.Clear();
    }

    /*public PileView PileToPileView(Pile pile)
    {
        if (TalonView.Pile == pile) return TalonView;
        if (HandView.Pile == pile) return HandView;
        foreach (PileView pileView in TableauViews)
        {
            if (pileView.Pile == pile) return pileView;
        }
        foreach (PileView pileView in FoundationViews)
        {
            if (pileView.Pile == pile) return pileView;
        }
        return null;
    }*/

    public void Shuffle()
    {
        if (cardManager.Hand.Cards.Count + cardManager.Talon.Cards.Count == 0)
        {
            Debug.LogWarning("Noting to shuffle.");
            return;
        }
        cardManager.ShuffleFaceDown();
        foreach (PileView pile in TableauViews) pile.UpdatePileView();
        undoManager.Undos.Clear();
        FindObjectOfType<HandManager>().OnPointerClick();
    }

    public bool IsAnyCardAnimating()
    {
        foreach (CardView cardView in CardToCardView.Values)
        {
            if (cardView.IsAnimating) return true;
        }
        return false;
    }
}
