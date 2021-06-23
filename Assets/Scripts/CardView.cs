using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardView : MonoBehaviour, IDragHandler, IDropHandler, IBeginDragHandler, IPointerClickHandler
{
    public Card Card;
    public Transform Image;
    public Image Back;
    public Image Face;
    public Text NumberText;
    public Text SuitSmallText;
    public Text SuitBigText;
    public GameObject Highlight;

    public bool IsHint = false;
    public bool IsPlaceholder = false;
    public float FlipAnimTimer = -1;
    public Vector3? OriPos = null;

    GameManager gameManager;
    Canvas canvas = null;

    public bool IsAnimating { get { return isAnimatingMove ; } }
    public bool isAnimatingMove = false;
    ViewManager viewManager;
    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        viewManager = FindObjectOfType<ViewManager>();
        
    }
    private void Start()
    {
        //Animate();
        if (!IsHint) UpdateCardView();
        FlipAnimTimer = Card.IsFaceUp ? 1 : -1;
    }

    private void Update()
    {
        Animate();
    }

    public void OnBeginDrag(PointerEventData data)
    {
        if (!Card.IsFaceUp || IsHint) return;

        if (PileView.Pile.Type != Pile.PileType.tableau && PileView.Pile.Cards.Last() != Card) return;

        viewManager.OnBeginDragCard(Card);
    }

    public void OnDrag(PointerEventData data)
    {
        if (!Card.IsFaceUp || IsHint) return;
        viewManager.OnPileDrag(data.position);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!Card.IsFaceUp || IsHint) return;
        if (viewManager.DraggedCardViews.Count == 0)
        {
            OnBeginDrag(eventData);
            OnDrop(eventData);
        }
    }

    public void OnDrop(PointerEventData data)
    {
        if (!Card.IsFaceUp || IsHint) return;
        viewManager.OnPileEndDrag();
    }
    public void UpdateCardView(Vector3? oriPos = null)
    {


        Highlight.SetActive(IsHint);

        OriPos = oriPos;

        if (Card.Number == 1) NumberText.text = "A";
        else if (Card.Number == 11) NumberText.text = "J";
        else if (Card.Number == 12) NumberText.text = "Q";
        else if (Card.Number == 13) NumberText.text = "K";
        else NumberText.text = Card.Number.ToString();

        if (Card.Suit == Card.SuitType.clubs)
        {
            SuitSmallText.text = "♣";
            SuitBigText.text = "♣";
        }
        else if (Card.Suit == Card.SuitType.diamonds)
        {
            SuitSmallText.text = "♦";
            SuitBigText.text = "♦";
        }
        else if (Card.Suit == Card.SuitType.hearts)
        {
            SuitSmallText.text = "♥";
            SuitBigText.text = "♥";
        }
        else if (Card.Suit == Card.SuitType.spades)
        {
            SuitSmallText.text = "♠";
            SuitBigText.text = "♠";
        }

        NumberText.color = Card.Color;
        SuitSmallText.color = Card.Color;
        SuitBigText.color = Card.Color;
    }

    /*public void CardToPileView(PileView pileView)
    {
        OriPos = transform.position;
        //transform.SetParent(pileView.transform);
    }*/

    void Animate()
    {
        if (Card.IsFaceUp && FlipAnimTimer <= 1)
        {

            FlipAnimTimer += Time.deltaTime * gameManager.AnimationSpeed / 2;
        }

        if (!Card.IsFaceUp && FlipAnimTimer >= -1)
        {
            FlipAnimTimer -= Time.deltaTime * gameManager.AnimationSpeed/ 2;
        }
        Face.transform.localScale = new Vector3(Mathf.Clamp(FlipAnimTimer, 0, 1), 1, 1);
        Back.transform.localScale = new Vector3(Mathf.Clamp(-FlipAnimTimer, 0, 1), 1, 1);


        if (OriPos != null)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(transform.GetComponent<RectTransform>());
            LayoutRebuilder.ForceRebuildLayoutImmediate(transform.parent.GetComponent<RectTransform>());
            Image.transform.position = (Vector3)OriPos;
            Image.gameObject.SetActive(true);
            isAnimatingMove = true;
            OriPos = null;
        }

        if (isAnimatingMove)
        {
            if (IsHint) Image.transform.position = Vector3.Lerp(Image.transform.position, transform.position, Time.deltaTime * gameManager.AnimationSpeed / 2);
            else Image.transform.position = Vector3.Lerp(Image.transform.position, transform.position, Time.deltaTime * gameManager.AnimationSpeed);

            if (Vector3.Distance(Image.transform.position, transform.position) < 1f)
            {
                Image.transform.localPosition = Vector3.zero;
                OverrideSorting(false);
                isAnimatingMove = false;
                if (IsHint || IsPlaceholder) Destroy(gameObject);
            }
        }
    }

    public void OverrideSorting(bool enable, int order = 1)
    {
        if (enable)
        {
            if (!canvas) canvas = gameObject.AddComponent<Canvas>();
            canvas.overrideSorting = true;
            canvas.sortingOrder = transform.GetSiblingIndex() + 1;
        }
        else
        {
            Destroy(canvas);
        }
    }

    public PileView PileView
    {
        get { return transform.parent.GetComponent<PileView>(); }
    }
}
