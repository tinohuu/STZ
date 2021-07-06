using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Save
{
    public List<List<Card>> Piles = new List<List<Card>>();
    public GameData GameData = null;
    public SettingsData SettingsData = null;
    public StatisticsData StatisticsData = null;
    public List<BackSkinData> BackSkinDatas = new List<BackSkinData>();
    public List<DeckSkinData> DeckSkinDatas = new List<DeckSkinData>();
    public int CurDeckSkinId = 0;
    public int CurBackSkinId = 0;
}
