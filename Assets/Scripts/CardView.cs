using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardView : MonoBehaviour, IDragHandler, IDropHandler, IBeginDragHandler, IPointerClickHandler
{
    [Header("Settings")]
    public Card Card = new Card(Card.SuitType.spade, 1);
    public bool IsHint = false;
    public bool HasSkin = false;
    public bool IsPlaceholder = false; // TBR

    [Header("References")]
    public Transform Image;
    public Image Back;
    public Image Face;
    public Text NumberText;
    public Text SuitSmallText;
    public Text SuitBigText;
    public Image Highlight;
    public Image NumberImage;
    public Image SmallSuitImage;
    public Image BigSuitImage;

    [Header("Inspected")]
    public float FlipAnimTimer = -1;
    public Vector3? AnimStartPos = null;
    public float AnimStartTime = 0;
    public bool IsAnimating { get { return isAnimatingMove; } }
    public bool isAnimatingMove = false;

    public delegate void UpdateView();
    public event UpdateView OnUpdateCardView = null;
    public Vector2 oriSize;
    GameManager gameManager;
    Canvas canvas = null;
    RectTransform rectTransform;
    ViewManager viewManager;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        gameManager = FindObjectOfType<GameManager>();
        viewManager = FindObjectOfType<ViewManager>();
        oriSize = rectTransform.sizeDelta;
        //skinManager = FindObjectOfType<SkinManager>();
        Alpha = 0;

        Back.gameObject.SetActive(true);
        Highlight.gameObject.SetActive(true);
    }
    private void Start()
    {
        //Animate();
        InitialiseCardView();
        UpdateCardView();
        //if (!IsHint) Alpha = 1;
        FlipAnimTimer = Card.IsFaceUp ? 1 : -1;
        //rectTransform.sizeDelta = Image.GetComponent<RectTransform>().sizeDelta;
    }

    private void Update()
    {
        //Image.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        Animate();
        if (Back.color.a == 0) Alpha = 1;
    }
    private void OnDisable()
    {
        Image.transform.localPosition = Vector3.zero;
        Image.transform.localRotation = Quaternion.identity;
    }

    public void OnBeginDrag(PointerEventData data)
    {
        if (!Card.IsFaceUp || IsHint || !PileView) return;

        if (PileView.Pile.Type != Pile.PileType.tableau && PileView.Pile.Cards.Last() != Card) return;

        viewManager.OnViewBeginDrag(Card);
    }

    public void OnDrag(PointerEventData data)
    {
        if (!Card.IsFaceUp || IsHint) return;
        viewManager.OnViewDra(data.position);
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
        viewManager.OnViewEndDrag();
    }
    public void UpdateCardView(Vector3? animStartPos = null, float animPauseTime = 0)
    {
        Highlight.gameObject.SetActive(IsHint);

        //if (Vector3.Distance(Image.transform.position, transform.position) > 0.1f) Alpha = 0;

        if (animStartPos != null)
        {
            AnimStartPos = animStartPos;
            AnimStartTime = Time.time + animPauseTime;
        }


        // Resize card according to pile type
        if (PileView)
        {
            if (PileView.Pile.Type == Pile.PileType.tableau)
                rectTransform.sizeDelta = Card.IsFaceUp ? new Vector2(oriSize.x, oriSize.y / 3) : new Vector2(oriSize.x, oriSize.y / 9);
            else if (PileView && PileView.Pile.Type == Pile.PileType.talon)
            {
                if (PileView.Pile.Cards.IndexOf(Card) < PileView.Pile.Cards.Count - gameManager.SettingsData.DrawCards) rectTransform.sizeDelta = new Vector2(0, oriSize.y);
                else rectTransform.sizeDelta = rectTransform.sizeDelta = oriSize;
            }
            else rectTransform.sizeDelta = oriSize;
        }

        if (OnUpdateCardView != null) OnUpdateCardView.Invoke();
    }

    void Animate()
    {
        if (Card.IsFaceUp && FlipAnimTimer <= 1)
        {
            FlipAnimTimer += Time.deltaTime * gameManager.SettingsData.AnimationSpeed / 2;
        }

        if (!Card.IsFaceUp && FlipAnimTimer >= -1)
        {
            FlipAnimTimer -= Time.deltaTime * gameManager.SettingsData.AnimationSpeed / 2;
        }
        Face.transform.localScale = new Vector3(Mathf.Clamp(FlipAnimTimer, 0, 1), 1, 1);
        Back.transform.localScale = new Vector3(Mathf.Clamp(-FlipAnimTimer, 0, 1), 1, 1);


        if (AnimStartPos != null)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(transform.GetComponent<RectTransform>());
            LayoutRebuilder.ForceRebuildLayoutImmediate(transform.parent.GetComponent<RectTransform>());
            Image.transform.position = (Vector3)AnimStartPos;
            //Image.gameObject.SetActive(true);
            isAnimatingMove = true;
            AnimStartPos = null;
            //OverrideSorting(true, 1);
            //Alpha = 1;
        }

        if (isAnimatingMove && Time.time >= AnimStartTime)
        {
            if (IsHint)
            {
                Image.transform.position = Vector3.Lerp(Image.transform.position, transform.position, Time.deltaTime * 10);
                Alpha = Mathf.Clamp(Vector3.Distance(Image.transform.position, transform.position) / 10, 0, 1);
            }
            else Image.transform.position = Vector3.Lerp(Image.transform.position, transform.position, Time.deltaTime * gameManager.SettingsData.AnimationSpeed);

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

    public float Alpha
    {
        set
        {
            Back.SetAlpha(value);
            Face.SetAlpha(value);

            NumberText.SetAlpha(value);
            SuitSmallText.SetAlpha(value);
            SuitBigText.SetAlpha(value);

            NumberImage.SetAlpha(value);
            SmallSuitImage.SetAlpha(value);
            BigSuitImage.SetAlpha(value);

            Highlight.SetAlpha(value);
        }
    }

    void InitialiseCardView()
    {
        if (HasSkin) return;

        // Display by text
        if (Card.Number == 1) NumberText.text = "A";
        else if (Card.Number == 11) NumberText.text = "J";
        else if (Card.Number == 12) NumberText.text = "Q";
        else if (Card.Number == 13) NumberText.text = "K";
        else NumberText.text = Card.Number.ToString();

        if (Card.Suit == Card.SuitType.club)
        {
            SuitSmallText.text = "♣";
            SuitBigText.text = "♣";
        }
        else if (Card.Suit == Card.SuitType.diamond)
        {
            SuitSmallText.text = "♦";
            SuitBigText.text = "♦";
        }
        else if (Card.Suit == Card.SuitType.heart)
        {
            SuitSmallText.text = "♥";
            SuitBigText.text = "♥";
        }
        else if (Card.Suit == Card.SuitType.spade)
        {
            SuitSmallText.text = "♠";
            SuitBigText.text = "♠";
        }
    }
}
