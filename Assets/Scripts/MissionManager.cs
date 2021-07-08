using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionManager : MonoBehaviour
{
    public static MissionManager Instance;
    public Mission Mission = new Mission();
    public GameManager.Handler OnOpenDaily = null;
    public GameManager.Handler OnDoneMission = null;
    private void Awake()
    {
        Instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        Mission.Initialize();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

[System.Serializable]
public class Mission
{
    public MissionData MissionData = null;
    GameManager.Handler AddProgress;
    GameManager.Handler AddProgressWithCons;
    public Mission()
    {
        AddProgress = () => MissionData.Progress++;
        AddProgressWithCons = new GameManager.Handler(CheckToAddProgress);
    }
    public void Initialize()
    {
        MissionData = new MissionData();
        MissionData.MissionType = MissionData.Type.useDeck;
        MissionData.MaxProgress = 3;
        switch (MissionData.MissionType)
        {
            case MissionData.Type.getDeck:
                SkinManager.Instance.OnGetDeck += AddProgress;
                break;
            case MissionData.Type.getBack:
                SkinManager.Instance.OnGetBack += AddProgress;
                break;
            case MissionData.Type.useDeck:
                SkinManager.Instance.OnUseDeck += AddProgress;
                break;
            case MissionData.Type.useBack:
                SkinManager.Instance.OnUseDeck += AddProgress;
                break;
            case MissionData.Type.playGame:
                ViewManager.Instance.OnStartNew += AddProgressWithCons;
                break;
            case MissionData.Type.winGame:
                ViewManager.Instance.OnWin += AddProgressWithCons;
                break;
            case MissionData.Type.useHint:
                HintManager.Instance.OnHint += AddProgressWithCons;
                break;
            case MissionData.Type.useShuffle:
                ViewManager.Instance.OnShuffle += AddProgress;
                break;
            case MissionData.Type.openDaily:
                MissionManager.Instance.OnOpenDaily += AddProgressWithCons;
                break;
            case MissionData.Type.doneOther:
                MissionManager.Instance.OnDoneMission += AddProgressWithCons;
                break;
        }    
    }

    void CheckToAddProgress()
    {
        if (MissionData.isETW && GameManager.Instance.GameData.GameMode != GameData.Mode.easy) return;
        if (MissionData.isChallenge && GameManager.Instance.GameData.GameMode != GameData.Mode.challenge) return;
        if (MissionData.noUndo && GameManager.Instance.GameData.UndoUses > 0) return;
        if (MissionData.noHint && GameManager.Instance.GameData.HintUses > 0) return;
        if (MissionData.noShuffle && GameManager.Instance.GameData.ShuffleUses > 0) return;
        if (MissionData.isContinuous && GameManager.Instance.GameData.WinningStreak == 0) return;
        MissionData.Progress++;
    }
}

[System.Serializable]
public class MissionData
{
    public enum Type { none, getDeck, getBack, playGame, winGame, useHint, useShuffle, useDeck, useBack, doneEO, doneOther, openDaily }
    public Type MissionType = Type.none;

    public bool isETW = false;
    public bool isChallenge = false;
    public bool noUndo = false;
    public bool noHint = false;
    public bool noShuffle = false;
    public bool isContinuous = false;

    public int Progress = 0;
    public int MaxProgress = 0;
    public int Difficulty = 0;
}



[System.Serializable]
public class Reward
{
    public enum Type { none, deck, back, hint, shuffle, both }
    public Type RewardType = Type.none;
    public int Count = 0;
}
