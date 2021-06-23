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
    float FlipAnimTimer = -1;
    GameManager gameManager;
    Canvas canvas = null;
    public bool IsAnimating
    {
        get { return isAnimatingMove || isAnimatingFlip; }
    }

    bool isAnimatingMove = false;
    bool isAnimatingFlip = false;
    ViewManager viewManager;
    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        viewManager = FindObjectOfType<ViewManager>();
        
    }
    private void Start()
    {
        //Animate();
        UpdateCardView();
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

        viewManager.OnPileBeginDrag(transform.parent.GetComponent<PileView>(), this);
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
    public void UpdateCardView()
    {
        Highlight.SetActive(IsHint);
        //Face.transform.localScale = new Vector3(Card.IsFaceUp? 1 : 0, 1, 1);
        //Back.transform.localScale = new Vector3(!Card.IsFaceUp? 0 : 1, 1, 1);
        //Background.gameObject.SetActive(!Card.IsFaceUp);
        //Face.gameObject.SetActive(Card.IsFaceUp);

        if (!Card.IsFaceUp)
        {
            NumberText.text = "";
            SuitSmallText.text = "";
            SuitBigText.text = "";
            return;
        }


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
        // Lerp Anim
        /*
        if (OriViewPos != null)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(transform.GetComponent<RectTransform>());
            LayoutRebuilder.ForceRebuildLayoutImmediate(transform.parent.GetComponent<RectTransform>());

            Image.transform.position = (Vector3)OriViewPos;
            OriViewPos = null;
            OverrideSorting(true);

            IsAnimating = true;
            //transform.parent.GetComponent<LayoutGroup>().enabled = false;
            //transform.parent.GetComponent<LayoutGroup>().enabled = true;
        }*/

        if (Card.IsFaceUp && FlipAnimTimer != 1)
        {

            FlipAnimTimer += Time.deltaTime * gameManager.AnimationSpeed / 2;
        }

        if (!Card.IsFaceUp && FlipAnimTimer != -1)
        {
            FlipAnimTimer -= Time.deltaTime * gameManager.AnimationSpeed/ 2;
        }
        Face.transform.localScale = new Vector3(Mathf.Clamp(FlipAnimTimer, 0, 1), 1, 1);
        Back.transform.localScale = new Vector3(Mathf.Clamp(-FlipAnimTimer, 0, 1), 1, 1);


        if (viewManager.CardViewOldPos.ContainsKey(Card))
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(transform.GetComponent<RectTransform>());
            LayoutRebuilder.ForceRebuildLayoutImmediate(transform.parent.GetComponent<RectTransform>());
            Image.transform.position = viewManager.CardViewOldPos[Card];
            Image.gameObject.SetActive(true);
            isAnimatingMove = true;
            viewManager.CardViewOldPos.Remove(Card);
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

        /*if (!isAnimatingFlip && IsFaceUp != Card.IsFaceUp)
        {
            isAnimatingFlip = true;
            Background.transform.localScale = new Vector3(IsFaceUp ? 0 : 1, 1, 1);
            Face.transform.localScale = new Vector3(IsFaceUp ? 1 : 0, 1, 1);
            //Background.gameObject.SetActive(IsFaceUp);
            //Face.gameObject.SetActive(IsFaceUp);
            //Image.transform.localScale = Vector3.right * (IsFaceUp ? 1 : );
            //Debug.LogWarning("Animate Flip");
        }

        if (isAnimatingFlip)
        {
            Background.transform.localScale += Vector3.right * Time.deltaTime * gameManager.AnimationSpeed / 4 * (Card.IsFaceUp ? -1 : 1);
            Face.transform.localScale += Vector3.right * Time.deltaTime * gameManager.AnimationSpeed / 4 * (Card.IsFaceUp ? 1 : -1);

            bool FaceUpCon = Card.IsFaceUp && Background.transform.localScale.x <= 0 && Face.transform.localScale.x >= 1;
            bool FaceDownCon = !Card.IsFaceUp && Background.transform.localScale.x >= 1 && Face.transform.localScale.x <= 0;
            if (FaceUpCon || FaceDownCon)
            {
                IsFaceUp = Card.IsFaceUp;
                Background.transform.localScale = new Vector3(Card.IsFaceUp ? 0 : 1, 1, 1);
                Face.transform.localScale = new Vector3(Card.IsFaceUp ? 1 : 0, 1, 1);
                isAnimatingFlip = false;
            }
        }*/
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
