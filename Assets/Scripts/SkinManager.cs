using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkinManager : MonoBehaviour
{
    [Header("Configuration")]
    public List<DeckSkinData> DeckSkinDatas = new List<DeckSkinData>();
    public List<BackSkinData> BackSkinDatas = new List<BackSkinData>();
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
    public int CurDeckSkinId = 0;
    public int CurBackSkinId = 0;

    public List<DeckSkin> DeckSkins = new List<DeckSkin>();
    public List<BackSkin> BackSkins = new List<BackSkin>();

    public static SkinManager Instance;
    List<DeckSkinView> deckSkinViews = new List<DeckSkinView>();
    List<BackSkinView> backSkinViews = new List<BackSkinView>();
    ViewManager viewManager;

    public GameManager.Handler OnGetDeck = null;
    public GameManager.Handler OnGetBack = null;
    public GameManager.Handler OnUseDeck = null;
    public GameManager.Handler OnUseBack = null;
    private void Awake()
    {
        Instance = this;

        // Find all skins by path
        string rootPath = "DeckSkins/DeckSkin_";
        for (int i = 0; i >= 0; i++)
        {
            DeckSkin deckSkin = Resources.Load<DeckSkin>(rootPath + i);
            if (!deckSkin) break;
            DeckSkins.Add(deckSkin);
        }
        rootPath = "BackSkins/BackSkin_";
        for (int i = 0; i >= 0; i++)
        {
            BackSkin backSkin = Resources.Load<BackSkin>(rootPath + i);
            if (!backSkin) break;
            BackSkins.Add(backSkin);
        }

        if (GameManager.Instance.Save == null)
        {
            for (int i = 0; i < DeckSkins.Count; i++) DeckSkinDatas.Add(new DeckSkinData(DeckSkins[i].Id, i == 0 ? -1 : 10, true));
            for (int i = 0; i < BackSkins.Count; i++) BackSkinDatas.Add(new BackSkinData(BackSkins[i].Id, i == 0 ? -1 : 10, true));
            CurDeckSkin = DeckSkins[CurDeckSkinId];
            CurBackSkin = BackSkins[CurBackSkinId];
        }
        else
        {
            DeckSkinDatas = GameManager.Instance.Save.DeckSkinDatas;
            BackSkinDatas = GameManager.Instance.Save.BackSkinDatas;
            CurDeckSkin = DeckSkins[GameManager.Instance.Save.CurDeckSkinId];
            CurBackSkin = BackSkins[GameManager.Instance.Save.CurBackSkinId];
            CurDeckSkinId = GameManager.Instance.Save.CurDeckSkinId;
            CurBackSkinId = GameManager.Instance.Save.CurBackSkinId;
        }


    }
    private void Start()
    {
        viewManager = FindObjectOfType<ViewManager>();
        if (CurDeckSkin) HandCover.sprite = CurDeckSkin.CoverSprite;

        // Create deck skin UI slots
        DeckSkinsArea.DestoryChildren();
        foreach (DeckSkinData deckSkinData in DeckSkinDatas)
        {
            DeckSkinView deckSkinView = Instantiate(DeckSkinViewPrefab, DeckSkinsArea).GetComponent<DeckSkinView>();
            deckSkinView.DeckSkinData = deckSkinData;
            deckSkinViews.Add(deckSkinView);
        }

        // Create back skin UI slots
        BackSkinsArea.DestoryChildren();
        foreach (BackSkinData backSkinData in BackSkinDatas)
        {
            //Debug.Log("Create back skin slots.");
            BackSkinView backSkinView = Instantiate(BackSkinViewPrefab, BackSkinsArea).GetComponent<BackSkinView>();
            backSkinView.BackSkinData = backSkinData;
            backSkinViews.Add(backSkinView);
        }
        
        //ApplyDeckSkin(DeckSkinDatas[CurDeckSkinId]);
        ApplyBackSkin(BackSkinDatas[CurBackSkinId]);

        GameManager.Instance.OnMove += new GameManager.Handler(UseSkin);
        OnGetDeck += () => Debug.Log("You got a new deck.");
        OnGetBack += () => Debug.Log("You got a new back.");
        OnUseDeck += () => Debug.Log("You used a deck.");
        OnUseBack += () => Debug.Log("You used a back.");
        ViewManager.Instance.OnStartNew += new GameManager.Handler(UseCurDeckSkin);
        ViewManager.Instance.OnStartNew += new GameManager.Handler(UseCurBackSkin);
    }

    public void ApplyDeckSkin(DeckSkinData deckSkinData)
    {
        if (deckSkinData.Durability == 0) return;
        //deckSkinData.Durability = deckSkinData.Durability == -1 ? -1 : deckSkinData.Durability - 1;
        CurDeckSkin = DeckSkins[deckSkinData.Id];
        foreach (CardView cardView in viewManager.CardToCardView.Values) cardView.GetComponent<CardSkinView>().UpdateView();

        foreach (DeckSkinView deckSkinView in deckSkinViews) deckSkinView.UpdateView();
        //if (CurDeckSkin) HandCover.sprite = CurDeckSkin.CoverSprite;
    }

    public void ApplyBackSkin(BackSkinData backSkinData)
    {
        if (backSkinData.Durability == 0) return;
        //backSkinData.Durability = backSkinData.Durability == -1 ? -1 : backSkinData.Durability - 1;
        CurBackSkin = BackSkins[backSkinData.Id];

        BackgroundMain.sprite = BackSkins[backSkinData.Id].Main;
        BackgroundMain.color = BackSkins[backSkinData.Id].Tint;
        BackgroundSecondary.sprite = BackSkins[backSkinData.Id].Secondary;
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
        PreviewBack.sprite = BackSkins[backSkinData.Id].Main;
        PreviewBack.color = BackSkins[backSkinData.Id].Tint;
        PreviewBackName.text = BackSkins[backSkinData.Id].Name;
    }

    public void UpdateView()
    {
        foreach (DeckSkinView deckSkinView in deckSkinViews) deckSkinView.UpdateView();
        foreach (BackSkinView backSkinView in backSkinViews) backSkinView.UpdateView();
    }

    public void UseSkin()
    {
        if (CurDeckSkin.Id != CurDeckSkinId) UseCurDeckSkin();
        if (CurBackSkin.Id != CurBackSkinId) UseCurBackSkin();
    }

    public void UseCurDeckSkin()
    {
        if (DeckSkinDatas[CurDeckSkin.Id].Durability == 0)
        {
            CurDeckSkinId = 0;
            ApplyDeckSkin(DeckSkinDatas[0]);
            return;
        }
        else if (DeckSkinDatas[CurDeckSkin.Id].Durability == -1)
        {
            CurDeckSkinId = CurDeckSkin.Id;
            return;
        }

        DeckSkinDatas[CurDeckSkin.Id].Durability = DeckSkinDatas[CurDeckSkin.Id].Durability == -1 ? -1 : DeckSkinDatas[CurDeckSkin.Id].Durability - 1;
        CurDeckSkinId = CurDeckSkin.Id;
        OnUseDeck?.Invoke();
    }

    public void UseCurBackSkin()
    {
        if (BackSkinDatas[CurBackSkin.Id].Durability == 0)
        {
            CurDeckSkinId = 0;
            ApplyBackSkin(BackSkinDatas[0]);
            return;
        }
        else if (BackSkinDatas[CurBackSkin.Id].Durability == -1)
        {
            CurBackSkinId = CurBackSkin.Id;
            return;
        }

        BackSkinDatas[CurBackSkin.Id].Durability = BackSkinDatas[CurBackSkin.Id].Durability == -1 ? -1 : BackSkinDatas[CurBackSkin.Id].Durability - 1;
        CurBackSkinId = CurBackSkin.Id;
        OnUseBack?.Invoke();
    }
}



[System.Serializable]
public class DeckSkinData : SkinData
{
    public int Id = 0;
    public int Durability
    {
        set
        {
            if (value > _durability) SkinManager.Instance.OnGetDeck?.Invoke();
            else if (value < _durability) SkinManager.Instance.OnUseDeck?.Invoke();
            _durability = value;
        }
        get => _durability;
    }
    public DeckSkinData(int id, int durability, bool isNew)
    {
        Id = id;
        _durability = durability;
        IsNew = Id == 0 ? false : isNew;
    }
}

[System.Serializable]
public class BackSkinData : SkinData
{
    public int Id = 0;
    public int Durability
    {
        set
        {
            if (value > _durability) SkinManager.Instance.OnGetBack?.Invoke();
            else if (value < _durability) SkinManager.Instance.OnUseBack?.Invoke();
            _durability = value;
        }
        get => _durability;
    }
    public BackSkinData(int id, int durability, bool isNew)
    {
        Id = id;
        _durability = durability;
        IsNew = Id == 0 ? false : isNew;
    }
}

[System.Serializable]
public class SkinData
{
    [SerializeField] protected int _durability = 0; // -1 : infinite
    public bool IsNew = true;

}

