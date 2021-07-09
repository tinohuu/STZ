using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ViewManager : MonoBehaviour
{
    [Header("References")]
    public Transform TableauArea;
    public Transform FoundationArea;
    public HorizontalLayoutGroup TopLayoutGroup;
    public HorizontalLayoutGroup TableauLayoutGroup;

    public GameObject PilePrefab;
    public GameObject CardPrefab;
    public GameObject AutoWinButton;
    public GameObject WinText;

    [Header("Data")]
    public List<PileView> TableauViews;
    public List<PileView> FoundationViews;
    public PileView HandView;
    public PileView TalonView;
    public List<CardView> DraggedCardViews = new List<CardView>();

    CardManager cardManager;
    UndoManager undoManager;
    GameManager gameManager;
    PileView DraggedFrom;
    bool isAutoWinning = false;
    public static ViewManager Instance;
    public Dictionary<Card, CardView> CardToCardView = new Dictionary<Card, CardView>();
    public Dictionary<Pile, PileView> PileToPileView = new Dictionary<Pile, PileView>();
    public GameManager.Handler OnStartNew = null;
    public GameManager.Handler OnWin = null;
    public GameManager.Handler OnShuffle = null;
    private void Awake()
    {
        Instance = this;
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

        gameManager.OnMove += new GameManager.Handler(ShowAutoWin);
        OnStartNew += () => Debug.Log("You started a new game.");
        OnWin += () => Debug.Log("You win.");
        OnShuffle += () => Debug.Log("You shuffled your hand.");
    }
    void Update()
    {
        Rect screenRect = new Rect(0, 0, Screen.width, Screen.height);
        if (!screenRect.Contains(Input.mousePosition) || Input.GetMouseButtonUp(0)) OnViewEndDrag();

        //Debug.Log("PileViewCount " + PileToPileView.Count);
        if (Input.GetKeyDown(KeyCode.X)) StartNew();
        
    }
    public void OnViewBeginDrag(Card card)
    {
        if (IsAnyCardAnimating() || isAutoWinning) return;

        if (DraggedCardViews.Count > 0 && DraggedCardViews[0] )
        {
            OnViewEndDrag();
            return;
        }

        // Store original pile
        DraggedFrom = CardToCardView[card].PileView;

        // Initialise dragged pile
        for (int i = DraggedFrom.Pile.Cards.IndexOf(card); i < DraggedFrom.Pile.Cards.Count; i++)
        {
            CardView cardView = CardToCardView[DraggedFrom.Pile.Cards[i]];
            DraggedCardViews.Add(cardView);
            cardView.OverrideSorting(true, i + 1);
        }
    }
    public void OnViewDrag(Vector3 pos)
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
    public void OnViewEndDrag()
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
                if (DraggedCardViews[0].Card.Number == 13 || gameManager.IsAnyToEmptyPile)
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
        if (gameManager.Time == -1) gameManager.Time = Time.time;


        Card card = DraggedCardViews[0].Card;
        int index = DraggedFrom.Pile.Cards.IndexOf(card);
        bool isLastCardUp = index != 0 && DraggedFrom.Pile.Cards[index - 1].IsFaceUp;
        Undo undo = new Undo(DraggedFrom.Pile, newPile.Pile);
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

        // Count moves
        gameManager.Moves++;
    }
    public void ShowAutoWin()
    {
        if ((cardManager.IsAllFaceUp || GameManager.Instance.CheatText.activeSelf) && !isAutoWinning) AutoWinButton.SetActive(true);
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
        OnShuffle?.Invoke();
    }
    public bool IsAnyCardAnimating()
    {
        foreach (CardView cardView in CardToCardView.Values)
        {
            if (cardView.gameObject.activeSelf && cardView.IsAnimating) return true;
        }
        return false;
    }
    public void SetRightHand(bool isRightHand)
    {
        GameManager.Instance.SettingsData.IsRightHand = isRightHand;
        TopLayoutGroup.reverseArrangement = !isRightHand;
        TableauLayoutGroup.reverseArrangement = !isRightHand;
    }

    public void AutoWin()
    {
        AutoWinButton.SetActive(false);
        isAutoWinning = true;
        undoManager.Undos.Clear();
        StartCoroutine(AnimateAutoWin());
    }
    IEnumerator AnimateAutoWin()
    {
        List<Pile> piles = new List<Pile>();
        piles.AddRange(cardManager.Tableau);
        piles.Add(cardManager.Talon);
        piles.Add(cardManager.Hand);

        for (int i = 0; i < piles.Count; i++)
        {
            if (piles[i].Cards.Count != 0)
            {
                Card card = null;
                Pile newPile = null;

                if (piles[i].Type == Pile.PileType.tableau && !GameManager.Instance.CheatText.activeSelf && false)
                {
                    card = piles[i].Cards.Last();
                    newPile = cardManager.GetFoundationDest(card, piles[i]);
                }
                else
                {
                    foreach (Card c in piles[i].Cards)
                    {
                        card = c;
                        newPile = cardManager.GetFoundationDest(c, piles[i], true);
                        if (newPile != null) break;
                    }
                }

                if (newPile != null)
                {
                    cardManager.UpdateData(card, piles[i], newPile);
                    card.IsFaceUp = true;
                    foreach (PileView pileView in PileToPileView.Values) Debug.Log(pileView.name + pileView.Pile.Type);
                    PileToPileView[piles[i]].UpdatePileView();
                    PileToPileView[newPile].UpdatePileView();
                    float pauseTime = Vector3.Distance(CardToCardView[card].transform.position, PileToPileView[newPile].transform.position) / gameManager.SettingsData.AnimationSpeed * 0.01f;
                    yield return new WaitForSeconds(Vector3.Distance(CardToCardView[card].transform.position, PileToPileView[newPile].transform.position) / gameManager.SettingsData.AnimationSpeed * 0.01F);
                }
            }

            if (i == piles.Count - 1)
            {
                int cardCount = 0;
                foreach (Pile pile in piles) cardCount += pile.Cards.Count;
                if (cardCount > 0) i = -1;
            }

        }
        Win();
        yield return null;
    }

    public void Win()
    {
        WinText.SetActive(true);
        OnWin.Invoke();
    }

    public void StartNew()
    {
        cardManager.Redeal();
        foreach (PileView pileView in PileToPileView.Values) pileView.UpdatePileView();
        GameManager.Instance.GameData.GameMode = GameData.Mode.normal;
        OnStartNew?.Invoke();
    }

    public void StartETW()
    {
        cardManager.Redeal();
        foreach (PileView pileView in PileToPileView.Values) pileView.UpdatePileView();
        GameManager.Instance.GameData.GameMode = GameData.Mode.easy;
        OnStartNew?.Invoke();
    }

    public void StartChallenge()
    {
        cardManager.Redeal();
        foreach (PileView pileView in PileToPileView.Values) pileView.UpdatePileView();
        GameManager.Instance.GameData.GameMode = GameData.Mode.challenge;
        OnStartNew?.Invoke();
    }

    public void Replay()
    {
        cardManager.Redeal(false);
        foreach (PileView pileView in PileToPileView.Values) pileView.UpdatePileView();
        GameManager.Instance.GameData.GameMode = GameData.Mode.challenge;
        //OnStartNew?.Invoke();
    }
}
