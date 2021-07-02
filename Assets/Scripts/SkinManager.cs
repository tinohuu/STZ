using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkinManager : MonoBehaviour
{
    public DeckSkin CurDeckSkin = null;
    public Image HandCover = null;
    public List<DeckSkinData> DeckSkins = new List<DeckSkinData>();
    public List<BackSkinData> BackSkins = new List<BackSkinData>();
    public Transform SkinsArea;
    public GameObject DeckSkinViewPrefab;
    List<DeckSkinView> deckSkinViews = new List<DeckSkinView>();
    ViewManager viewManager;
    private void Start()
    {
        viewManager = FindObjectOfType<ViewManager>();
        if (CurDeckSkin) HandCover.sprite = CurDeckSkin.CoverSprite;

        SkinsArea.DestoryChildren();
        foreach (DeckSkinData deckSkinData in DeckSkins)
        {
            DeckSkinView deckSkinView = Instantiate(DeckSkinViewPrefab, SkinsArea).GetComponent<DeckSkinView>();
            deckSkinView.DeckSkinData = deckSkinData;
            deckSkinViews.Add(deckSkinView);
        }
    }

    public void ApplyDeckSkin(DeckSkinData deckSkinData)
    {
        if (deckSkinData.Durability == 0) return;
        deckSkinData.Durability = deckSkinData.Durability == -1 ? -1 : deckSkinData.Durability - 1;
        CurDeckSkin = deckSkinData.DeckSkin;
        foreach (CardView cardView in viewManager.CardToCardView.Values) cardView.GetComponent<CardSkinView>().UpdateView();

        foreach (DeckSkinView deckSkinView in deckSkinViews) deckSkinView.UpdateView();
        if (CurDeckSkin) HandCover.sprite = CurDeckSkin.CoverSprite;
    }
}



[System.Serializable]
public class DeckSkinData : SkinData
{
    public DeckSkin DeckSkin = null;
}

[System.Serializable]
public class BackSkinData : SkinData
{
    public BackSkin BackSkin = null;
}

[System.Serializable]
public class SkinData
{
    public int Durability = 0; // -1 : infinite
    public bool IsNew = true;
}

