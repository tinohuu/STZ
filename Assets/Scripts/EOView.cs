using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EOView : MonoBehaviour
{
    public GameObject Shuffle;
    public GameObject Hint;
    public GameObject Both;
    public GameObject DeckSkin;
    public GameObject BackSkin;
    public GameObject Wait;
    public Image DeckSkinImage;
    public Image BackSkinImage;
    public TMP_Text Info;
    public TMP_Text TimeText;
    //public RewardData RewardData;
    public Color Color = Color.white;
    //public bool IsWaiting = false;
    public ExclusiveOffer Offer;
    public Button CollectButton;

    public void Collect()
    {
        ViewManager.Instance.Ad.SetActive(true);
        MissionManager.Instance.Collect(Offer.RewardData);
        MissionManager.Instance.OneDoneEO?.Invoke();
        Renew();
    }

    private void Update()
    {
        System.TimeSpan timeSpan = System.DateTime.Now - Offer.NextUpdateTime;
        if (timeSpan.TotalSeconds >= 0)
        {
            Renew();
        }
        if (GameManager.Instance.IsCheated && Input.GetKeyDown(KeyCode.E))
        {
            Renew();
            Offer.CooldownMinutes = 1;
        }
        TimeText.text = timeSpan.ToString("hh':'mm':'ss");
    }

    public void Renew()
    {
        Offer.IsCooldown = !Offer.IsCooldown;
        Offer.NextUpdateTime = System.DateTime.Now;

        if (!Offer.IsCooldown)
        {
            if (Offer.RewardData.RewardType == RewardData.Type.deck || Offer.RewardData.RewardType == RewardData.Type.back) Offer.RewardData.Count = 7;
            else if (Offer.RewardData.RewardType == RewardData.Type.both) Offer.RewardData.Count = Random.Range(1, 6) * 10 + Random.Range(1, 6);
            else Offer.RewardData.Count = Random.Range(1, 6);
            Offer.RewardData.Initialise();

            Offer.NextUpdateTime = System.DateTime.Now.AddMinutes(Random.Range(1, 3) * 5);
        }
        else
        {
            Offer.CooldownMinutes = Mathf.Clamp(Offer.CooldownMinutes + 1, 1, 3);
            Offer.NextUpdateTime = System.DateTime.Now.AddMinutes(Offer.CooldownMinutes);
        }

        CollectButton.interactable = !Offer.IsCooldown;

        UpdateView();
    }

    public void UpdateView()
    {
        GetComponent<Image>().color = Offer.IsCooldown ? Color.grey : Color;
        Shuffle.SetActive(Offer.RewardData.RewardType == RewardData.Type.shuffle && !Offer.IsCooldown);
        Hint.SetActive(Offer.RewardData.RewardType == RewardData.Type.hint && !Offer.IsCooldown);
        Both.SetActive(Offer.RewardData.RewardType == RewardData.Type.both && !Offer.IsCooldown);
        DeckSkin.SetActive(Offer.RewardData.RewardType == RewardData.Type.deck && !Offer.IsCooldown);
        BackSkin.SetActive(Offer.RewardData.RewardType == RewardData.Type.back && !Offer.IsCooldown);
        Wait.SetActive(Offer.IsCooldown);

        if (Offer.IsCooldown)
        {
            Info.text = "More will be available soon";
            return;
        }

        switch (Offer.RewardData.RewardType)
        {
            case RewardData.Type.shuffle:
                Info.text = "Shuffle x" + Offer.RewardData.Count;
                break;
            case RewardData.Type.hint:
                Info.text = "Hint x" + Offer.RewardData.Count;
                break;
            case RewardData.Type.both:
                Info.text = "Hint x" + Offer.RewardData.Count / 10 + "\n" + "Shuffle x" + Offer.RewardData.Count % 10;
                break;
            case RewardData.Type.deck:
                Info.text = SkinManager.Instance.DeckSkins[Offer.RewardData.SkinId].Name + " Deck\n" + "Durability: " + Offer.RewardData.Count;
                DeckSkinImage.sprite = SkinManager.Instance.DeckSkins[Offer.RewardData.SkinId].BoxSprite;
                break;
            case RewardData.Type.back:
                Info.text = SkinManager.Instance.BackSkins[Offer.RewardData.SkinId].Name + " Background\n" + "Durability: " + Offer.RewardData.Count;
                BackSkinImage.sprite = SkinManager.Instance.BackSkins[Offer.RewardData.SkinId].Main;
                break;
        }
    }
}