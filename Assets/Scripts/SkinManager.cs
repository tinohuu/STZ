using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkinManager : MonoBehaviour
{
    [Header("Configuration")]
    public List<DeckSkinData> DeckSkins = new List<DeckSkinData>();
    public List<BackSkinData> BackSkins = new List<BackSkinData>();
    [Header("References")]
    public Image HandCover = null;
    public Image BackgroundMain = null;
    public Image BackgroundSecondary = null;
    public Transform DeckSkinsArea;
    public Transform BackSkinsArea;
    public GameObject DeckSkinViewPrefab;
    public GameObject BackSkinViewPrefab;
    public GameObject Preview;
    public Image PreviewBack;
    public Text PreviewBackName;
    [Header("Inspected")]
    public DeckSkin CurDeckSkin = null;
    public BackSkin CurBackSkin = null;

    List<DeckSkinView> deckSkinViews = new List<DeckSkinView>();
    List<BackSkinView> backSkinViews = new List<BackSkinView>();
    ViewManager viewManager;
    private void Start()
    {
        viewManager = FindObjectOfType<ViewManager>();
        if (CurDeckSkin) HandCover.sprite = CurDeckSkin.CoverSprite;

        // Create deck skin UI slots
        DeckSkinsArea.DestoryChildren();
        foreach (DeckSkinData deckSkinData in DeckSkins)
        {
            DeckSkinView deckSkinView = Instantiate(DeckSkinViewPrefab, DeckSkinsArea).GetComponent<DeckSkinView>();
            deckSkinView.DeckSkinData = deckSkinData;
            deckSkinViews.Add(deckSkinView);
        }

        // Create back skin UI slots
        BackSkinsArea.DestoryChildren();
        foreach (BackSkinData backSkinData in BackSkins)
        {
            Debug.Log("Create back skin slots.");
            BackSkinView backSkinView = Instantiate(BackSkinViewPrefab, BackSkinsArea).GetComponent<BackSkinView>();
            backSkinView.BackSkinData = backSkinData;
            backSkinViews.Add(backSkinView);
        }

        //ApplyDeckSkin(DeckSkins[0]);
        //ApplyBackSkin(BackSkins[0]);
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

    public void ApplyBackSkin(BackSkinData backSkinData)
    {
        if (backSkinData.Durability == 0) return;
        backSkinData.Durability = backSkinData.Durability == -1 ? -1 : backSkinData.Durability - 1;
        CurBackSkin = backSkinData.BackSkin;

        BackgroundMain.sprite = backSkinData.BackSkin.Main;
        BackgroundMain.color = backSkinData.BackSkin.Tint;
        BackgroundSecondary.sprite = backSkinData.BackSkin.Secondary;
        BackgroundSecondary.gameObject.SetActive(BackgroundSecondary.sprite);
        //foreach (CardView cardView in viewManager.CardToCardView.Values) cardView.GetComponent<CardSkinView>().UpdateView();

        foreach (BackSkinView backSkinView in backSkinViews) backSkinView.UpdateView();
        //if (CurDeckSkin) HandCover.sprite = CurDeckSkin.CoverSprite;
    }

    public void PreviewBackSkin(BackSkinData backSkinData)
    {
        Preview.SetActive(true);
        // Update the skin of preview cards
        CardSkinView[] cardSkinViews = Preview.GetComponentsInChildren<CardSkinView>();
        foreach (CardSkinView cardSkinView in cardSkinViews) cardSkinView.UpdateView();
        // Update preview background
        PreviewBack.sprite = backSkinData.BackSkin.Main;
        PreviewBack.color = backSkinData.BackSkin.Tint;
        PreviewBackName.text = backSkinData.BackSkin.Name;
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

