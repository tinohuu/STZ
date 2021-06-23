using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ViewManager : MonoBehaviour
{
    public Transform Area;
    public Transform Foundations;
    public Transform Temp;
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
    PileView OriDraggedPile;
    public List<CardView> DraggedCardViews = new List<CardView>();
    public Dictionary<Card, Vector3> CardViewOldPos = new Dictionary<Card, Vector3>();
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
            PileView pileView = Instantiate(PilePrefab, Area).GetComponent<PileView>();
            pileView.Pile = cardManager.Tableau[i];
            pileView.UpdatePileView();
            TableauViews.Add(pileView);
        }

        // Initialise foundation pile views
        for (int i = 0; i < 4; i++)
        {
            PileView pileView = Instantiate(PilePrefab, Foundations).GetComponent<PileView>();
            pileView.Pile = cardManager.Foundations[i];
            pileView.GetComponent<VerticalLayoutGroup>().spacing = -pileView.GetComponent<RectTransform>().rect.height;
            pileView.UpdatePileView();
            FoundationViews.Add(pileView);
        }

        // Initialise talon pile views
        TalonView.Pile = cardManager.Talon;
        TalonView.UpdatePileView();
        HandView.Pile = cardManager.Hand;
        TalonView.UpdatePileView();
    }


    void Update()
    {
        Rect screenRect = new Rect(0, 0, Screen.width, Screen.height);
        if (!screenRect.Contains(Input.mousePosition) || Input.GetMouseButtonUp(0)) OnPileEndDrag();
    }
    public void OnPileBeginDrag(PileView pileView, CardView cardView)
    {
        if (IsAnyCardAnimating()) return;

        if (DraggedCardViews.Count > 0 && DraggedCardViews[0] )
        {
            OnPileEndDrag();
            return;
        }

        // Store original pile
        OriDraggedPile = pileView;

        // Initialise dragged pile
        for (int i = pileView.CardViews.IndexOf(cardView); i < pileView.CardViews.Count; i++)
        {
            DraggedCardViews.Add(pileView.CardViews[i]);
            pileView.CardViews[i].OverrideSorting(true, i);
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
            else if (TableauViews[i].Pile != OriDraggedPile.Pile
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
                float dis = Vector3.Distance(DraggedCardViews[0].Image.transform.position, pileView.CardViews.Count > 0 ? pileView.CardViews.Last().transform.position : pileView.transform.position);
                //Debug.LogWarning(dis);
                if (closestDistance > dis)
                {
                    closestDistance = dis;
                    closestPileView = pileView;
                    
                }

            }
            if (Vector3.Distance(DraggedCardViews[0].transform.position, DraggedCardViews[0].Image.transform.position) < 0.1f) closestPileView = AlternativePileViews[0];


            // Scoring
            if (OriDraggedPile.Pile.Type == Pile.PileType.talon)
            {
                if (closestPileView.Pile.Type == Pile.PileType.foundation) gameManager.Score += 15;
                else if (closestPileView.Pile.Type == Pile.PileType.tableau) gameManager.Score += 10;
            }
            if (OriDraggedPile.Pile.Type == Pile.PileType.foundation && closestPileView.Pile.Type == Pile.PileType.tableau) gameManager.Score -= 15;

            DragToNew(closestPileView);
            return;
        }

        // Return to original
        for (int i = 0; i < DraggedCardViews.Count; i++)
        {
            if (DraggedCardViews[i])
                CardViewOldPos.Add(DraggedCardViews[i].Card, DraggedCardViews[i].Image.transform.position);
            //    AnimateCardInPileView(DraggedCardViews[i].Card, DraggedCardViews[i].Image.transform.position, OriDraggedPile);//  DraggedCardViews[i].CardToPileView(OriDraggedPile); //DraggedPile.CardViews[i].transform.SetParent(OriDraggedPile.transform);
            DraggedCardViews[i].GetComponentInChildren<Animator>().SetTrigger("Shake");
            Debug.LogWarning("Shake");
        }
        DraggedCardViews.Clear();
    }

    void DragToNew(PileView newPile)
    {
        // Start to count game time
        if (gameManager.InitialTime == -1) gameManager.InitialTime = Time.time;

        gameManager.Moves++;

        for (int j = 0; j < DraggedCardViews.Count; j++)
        {
            if (j == 0)
            {
                Card card = DraggedCardViews[j].Card;
                int index = OriDraggedPile.Pile.Cards.IndexOf(card);
                bool isLastCardUp = index != 0 && OriDraggedPile.Pile.Cards[index - 1].IsFaceUp;
                Undo undo = new Undo(DraggedCardViews[j].Card, cardManager.AllPiles.IndexOf(OriDraggedPile.Pile), cardManager.AllPiles.IndexOf(newPile.Pile), isLastCardUp);
                undoManager.Undos.Add(undo);
            }

            CardViewOldPos.Add(DraggedCardViews[j].Card, DraggedCardViews[j].Image.transform.position);

            cardManager.UpdateData(DraggedCardViews[j].Card, OriDraggedPile.Pile, newPile.Pile);


            /*//OriDraggedPile.Pile.Remove(DraggedCardViews[j].Card);
            OriDraggedPile.CardViews.Remove(DraggedCardViews[j]);

            if (OriDraggedPile.Pile.Count > 0 && j == DraggedCardViews.Count - 1)
            {
                if (OriDraggedPile.PileType == PileView.Type.tableau && !OriDraggedPile.Pile[OriDraggedPile.Pile.Count - 1].IsFaceUp)
                    gameManager.Score += 5;
                OriDraggedPile.Pile[OriDraggedPile.Pile.Count - 1].IsFaceUp = true;
            }



            //newPile.Pile.Add(DraggedCardViews[j].Card);
            //newPile.UpdateCardView(DraggedCardViews[j].Card);
            newPile.UpdatePileView();
            /*if (newPile.PileType == PileView.Type.tableau)
            {
                AnimateCardInPileView(DraggedCardViews[j].Card, DraggedCardViews[j].Image.transform.position, newPile);
            }
            else if (newPile.PileType == PileView.Type.foundation)
            {
                AnimateCardInPileView(DraggedCardViews[j].Card, DraggedCardViews[j].Image.transform.position, newPile);
            }*/
        }

        if (OriDraggedPile.Pile.Cards.Count > 0)
        {
            if (OriDraggedPile.Pile.Type == Pile.PileType.tableau && !OriDraggedPile.Pile.Cards.Last().IsFaceUp)
                gameManager.Score += 5;
            OriDraggedPile.Pile.Cards.Last().IsFaceUp = true;
        }

        OriDraggedPile.UpdatePileView();
        newPile.UpdatePileView();
        DraggedCardViews.Clear();
    }

    public PileView PileToPileView(Pile pile)
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
    }

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
        foreach (PileView pileView in TableauViews)
        {
            foreach (CardView cardView in pileView.CardViews)
            {
                if (cardView.IsAnimating) return true;
            }
        }
        foreach (PileView pileView in FoundationViews)
        {
            foreach (CardView cardView in pileView.CardViews)
            {
                if (cardView.IsAnimating) return true;
            }
        }
        foreach (CardView cardView in TalonView.CardViews)
        {
            if (cardView.IsAnimating) return true;
        }
        return false;
    }
}
