using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackSkinView : SkinView
{
    public BackSkinData BackSkinData;
    public Image BackSlotImage;
    public override void Use()
    {
        BackSkinData.IsNew = false;
        skinManager.ApplyBackSkin(BackSkinData);
        UpdateView();
    }

    public void Preview()
    {
        skinManager.PreviewBackSkin(BackSkinData);
    }

    public override void UpdateView()
    {
        Durability.text = "Durability: " + (BackSkinData.Durability == -1 ? "??" : BackSkinData.Durability.ToString());
        New.SetActive(BackSkinData.IsNew);
        /*Box.sprite = BackSkinData.DeckSkin.BoxSprite;
        foreach (CardSkinView cardSkinView in PreviewCards)
        {
            cardSkinView.PreviewSkin = BackSkinData.DeckSkin;
            cardSkinView.UpdateView();
        }*/
        BackSlotImage.sprite = SkinManager.Instance.BackSkins[BackSkinData.Id].Main;
        BackSlotImage.color = SkinManager.Instance.BackSkins[BackSkinData.Id].Tint;
        UseButton.interactable = SkinManager.Instance.CurBackSkin != SkinManager.Instance.BackSkins[BackSkinData.Id];
        UseText.SetActive(UseButton.interactable);
        InUseText.SetActive(!UseButton.interactable);
    }
}
