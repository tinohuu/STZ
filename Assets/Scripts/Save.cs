using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Save
{
    public List<Card> Deck = new List<Card>();
    public List<List<Card>> Piles = new List<List<Card>>();
    public GameData GameData = null;
    public SettingsData SettingsData = null;
    public StatisticsData StatisticsData = null;
    // Skin System
    public List<BackSkinData> BackSkinDatas = new List<BackSkinData>();
    public List<DeckSkinData> DeckSkinDatas = new List<DeckSkinData>();
    public int CurDeckSkinId = 0;
    public int CurBackSkinId = 0;
    // Undo System
    public List<Undo> Undos = new List<Undo>();
    // Mission System
    public System.DateTime MissionUpdateTime = new System.DateTime();
    public List<MissionData> MissionDatas = new List<MissionData>();
    public List<ExclusiveOffer> ExclusiveOffers = new List<ExclusiveOffer>();
}
