using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HandManager: MonoBehaviour
{
    public Text NumberText;

    public Image Highlight;
    public float HighlightTimer = 0;
    //public Image HandCover;

    ViewManager viewManager;
    CardManager cardManager;
    GameManager gameManager;

    private void Awake()
    {   
        viewManager = FindObjectOfType<ViewManager>();
        cardManager = FindObjectOfType<CardManager>();
        gameManager = FindObjectOfType<GameManager>();
    }

    void Update()
    {
        Highlight.gameObject.SetActive(Time.time < HighlightTimer);
        NumberText.text = cardManager.Hand.Cards.Count == 0 ? "" : cardManager.Hand.Cards.Count.ToString();
        //HandCover.gameObject.SetActive(cardManager.Hand.Cards.Count != 0);
    }

    public void OnPointerClick()
    {
        if (GameManager.Instance.GameData.IsWon) return;
        gameManager.StartCountTime();

        // Reset hand
        if (cardManager.Hand.Cards.Count == 0 && cardManager.Talon.Cards.Count > 1)
        {
            if (cardManager.Talon.Cards.Count > 0) gameManager.Score -= 15;

            Undo undo = new Undo(cardManager.Talon, cardManager.Hand);
            FindObjectOfType<UndoManager>().Undos.Add(undo);

            List<Card> cards = new List<Card>(cardManager.Talon.Cards);
            foreach (Card card in cards)
            {
                card.IsFaceUp = false;
                cardManager.UpdateData(card, cardManager.Talon, cardManager.Hand);

            }
            viewManager.HandView.UpdatePileView();
            viewManager.TalonView.UpdatePileView();
        }

        // Deal hand to talon
        else
        {
            if (cardManager.Hand.Cards.Count == 0 && cardManager.Talon.Cards.Count > 1) return;

            Undo undo = new Undo(cardManager.Hand, cardManager.Talon);
            FindObjectOfType<UndoManager>().Undos.Add(undo);

            for (int i = 0; i < gameManager.SettingsData.DrawCards; i++)
            {
                if (cardManager.Hand.Cards.Count == 0) break;
                cardManager.Hand.Cards[0].IsFaceUp = true;
                cardManager.UpdateData(cardManager.Hand.Cards[0], cardManager.Hand, cardManager.Talon);

            }

            viewManager.TalonView.UpdatePileView();
            viewManager.HandView.UpdatePileView();

            gameManager.Moves++;
        }
    }
}
