using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HintViewManager : MonoBehaviour
{
    ViewManager cardViewManager;
    GameManager gameManager;
    public GameObject HintTextBackground;
    public Text HintText;
    List<CardView> OriCardViews = new List<CardView>();
    List<Vector3> TargetPositions = new List<Vector3>();

    List<CardView> HintCardViews = new List<CardView>();

    int hintIndex = 0;
    Canvas canvas;

    int hintMoves = 0;
    private void Awake()
    {
        cardViewManager = FindObjectOfType<ViewManager>();
        canvas = FindObjectOfType<Canvas>();
        gameManager = FindObjectOfType<GameManager>();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (HintCardViews.Count != 0 && hintMoves != gameManager.Moves)
        {
            ClearHint();
        }

        if ((HintCardViews.Count == 0 || !HintCardViews[0]) && OriCardViews.Count > 0)
        {
            HintTextBackground.SetActive(true);
            HintCardViews.Clear();
            HintText.text = "Move " + (hintIndex + 1) + " of " + OriCardViews.Count;


            if (hintIndex < OriCardViews.Count)
            {
                CardView HintCardView = Instantiate(cardViewManager.CardPrefab, canvas.transform).GetComponent<CardView>();
                HintCardView.transform.position = TargetPositions[hintIndex];
                //HintCardView.OriViewPos = OriCardViews[hintIndex].transform.position;
                HintCardView.Image.transform.position = OriCardViews[hintIndex].transform.position;
                //HintCardView.SetOriViewFaceUpData(OriCardViews[hintIndex].Card.IsFaceUp);
                HintCardView.Card = OriCardViews[hintIndex].Card;
                HintCardView.IsHint = true;
                HintCardViews.Add(HintCardView);

                PileView pileView = OriCardViews[hintIndex].PileView;
                if (pileView.Pile.Type == Pile.PileType.tableau)
                {
                    int index = pileView.CardViews.IndexOf(OriCardViews[hintIndex]);
                    if (index != pileView.CardViews.Count - 1)
                    {
                        for (int i = index + 1; i < pileView.CardViews.Count; i++)
                        {
                            CardView cardView = Instantiate(cardViewManager.CardPrefab, canvas.transform).GetComponent<CardView>();
                            cardView.transform.position = TargetPositions[hintIndex] + pileView.CardViews[i].transform.position - OriCardViews[hintIndex].transform.position;
                            //cardView.OriViewPos = pileView.CardViews[i].transform.position;
                            HintCardView.Image.transform.position = pileView.CardViews[i].transform.position;
                            //HintCardView.SetOriViewFaceUpData(pileView.CardViews[i].Card.IsFaceUp);
                            cardView.Card = pileView.CardViews[i].Card;
                            cardView.IsHint = true;
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
        hintMoves = gameManager.Moves;

        ClearHint();

        if (cardViewManager.TalonView.CardViews.Count > 0)
        {
            CardView ori = cardViewManager.TalonView.CardViews.Last();
            Vector3? target = FindTargetCardView(ori);
            if (target != null)
            {
                OriCardViews.Add(ori);
                TargetPositions.Add((Vector3)target);
            }
        }

        foreach (PileView pileView in cardViewManager.TableauViews)
        {
            CardView ori = null;
            foreach (CardView cardView in pileView.CardViews)
            {
                if (cardView.Card.IsFaceUp)
                {
                    ori = cardView;
                    break;
                }
            }
            if (!ori) continue;

            Vector3? target = FindTargetCardView(ori);
            if (target != null)
            {
                OriCardViews.Add(ori);
                TargetPositions.Add((Vector3)target);
            }
        }

        foreach (PileView pileView in cardViewManager.TableauViews)
        {
            if (pileView.CardViews.Count == 0) continue;

            CardView ori = pileView.CardViews.Last();

            if (OriCardViews.Contains(ori)) continue;

            Vector3? target = FindFoundationTarget(ori);
            if (target != null)
            {
                OriCardViews.Add(ori);
                TargetPositions.Add((Vector3)target);
            }
        }

        if (OriCardViews.Count == 0) FindObjectOfType<HandManager>().HighlightTimer = Time.time + 1;
    }

    Vector3? FindTargetCardView(CardView cardView)
    {
        Vector3? target = FindFoundationTarget(cardView);
        if (target != null) return target;
        return FindTableauTarget(cardView);
    }

    Vector3? FindFoundationTarget(CardView cardView)
    {
        if (cardView == cardView.PileView.CardViews.Last())
        {
            foreach (PileView pileView in cardViewManager.FoundationViews)
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
        foreach (PileView pileView in cardViewManager.TableauViews)
        {
            if (pileView.CardViews.Count == 0)
            {
                if (cardView.Card.Number == 13 && cardView.PileView.CardViews[0] != cardView) return pileView.transform.position;
            }
            else if (pileView.CardViews.Last().Card.Color != cardView.Card.Color
                && pileView.CardViews.Last().Card.Number == cardView.Card.Number + 1)
                return pileView.CardViews.Last().transform.position + Vector3.up * -40;
        }
        return null;
    }

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
