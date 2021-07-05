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
        Durability.text = "Durability: " + (BackSkinData.Durability == -1 ? "¡Þ" : BackSkinData.Durability.ToString());
        New.SetActive(BackSkinData.IsNew);
        /*Box.sprite = BackSkinData.DeckSkin.BoxSprite;
        foreach (CardSkinView cardSkinView in PreviewCards)
        {
            cardSkinView.PreviewSkin = BackSkinData.DeckSkin;
            cardSkinView.UpdateView();
        }*/
        BackSlotImage.sprite = BackSkinData.BackSkin.Main;
        BackSlotImage.color = BackSkinData.BackSkin.Tint;
        UseButton.interactable = skinManager.CurBackSkin != BackSkinData.BackSkin;
        UseText.SetActive(UseButton.interactable);
        InUseText.SetActive(!UseButton.interactable);
    }
}
