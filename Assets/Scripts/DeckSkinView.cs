using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeckSkinView : SkinView
{
    public DeckSkinData DeckSkinData;
    public List<CardSkinView> PreviewCards;
    public Image Box;

    public override void Use()
    {
        DeckSkinData.IsNew = false;
        skinManager.ApplyDeckSkin(DeckSkinData);
        UpdateView();
    }

    public override void UpdateView()
    {
        Durability.text = "Durability: " + DeckSkinData.Durability;
        New.SetActive(DeckSkinData.IsNew);
        Box.sprite = DeckSkinData.DeckSkin.BoxSprite;
        foreach (CardSkinView cardSkinView in PreviewCards)
        {
            cardSkinView.PreviewSkin = DeckSkinData.DeckSkin;
            cardSkinView.UpdateView();
        }
        UseButton.interactable = skinManager.CurDeckSkin != DeckSkinData.DeckSkin;
        UseText.SetActive(UseButton.interactable);
        InUseText.SetActive(!UseButton.interactable);
    }
}
