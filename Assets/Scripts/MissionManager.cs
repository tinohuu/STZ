using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionManager : MonoBehaviour
{
    [Header("Configuration")]
    public int MaxMissions = 4;
    public int UpdateHours = 18;
    public List<Mission> CurMissions = new List<Mission>();
    public DateTime MissionUpdateDate = new DateTime();

    [Header("Missions")]
    public List<MissionData> EasyMissions = new List<MissionData>();
    public List<MissionData> MidMissions = new List<MissionData>();
    public List<MissionData> HardMissions = new List<MissionData>();
    public List<MissionData> ExtraMissions = new List<MissionData>();
    public List<List<MissionData>> PresetMissions = new List<List<MissionData>>();
    [HideInInspector] public int PresetMissionsCount = 0;

    [Header("Rewards")]
    public List<RewardData> EasyRewards = new List<RewardData>();
    public List<RewardData> MidRewards = new List<RewardData>();
    public List<RewardData> HardRewards = new List<RewardData>();
    public List<RewardData> ExtraRewards = new List<RewardData>();
    public List<List<RewardData>> PresetRewards = new List<List<RewardData>>();
    [HideInInspector] public int PresetRewardsCount = 0;

    // Delegate
    public GameManager.Handler OnOpenDaily = null;
    public GameManager.Handler OnDoneMission = null;
    public GameManager.Handler OnDoneProgress = null;
    public GameManager.Handler OnCollect = null;
    public GameManager.Handler OneDoneEO = null;

    public static MissionManager Instance;
    private void Awake()
    {
        Instance = this;
        PresetMissions.Clear();
        PresetMissions.Add(EasyMissions);
        PresetMissions.Add(MidMissions);
        PresetMissions.Add(HardMissions);
        PresetMissions.Add(ExtraMissions);
        PresetRewards.Add(EasyRewards);
        PresetRewards.Add(MidRewards);
        PresetRewards.Add(HardRewards);
        PresetRewards.Add(ExtraRewards);

    }
    void Start()
    {
        if (GameManager.Instance.Save != null)
        {
            MissionUpdateDate = GameManager.Instance.Save.MissionUpdateTime;
            foreach (MissionData missionData in GameManager.Instance.Save.MissionDatas) CurMissions.Add(new Mission(missionData));
        }
        CheckToAddNew();
        MissionViewManager.Instance.UpdateView();
        //CheckToAddNew();
    }
    private void Update()
    {

    }
    public void LoadPresetMissions()
    {
        // Group missions by difficulty, 0 = easy
        PresetMissions.Clear();
        PresetMissions.Add(EasyMissions);
        PresetMissions.Add(MidMissions);
        PresetMissions.Add(HardMissions);
        PresetMissions.Add(ExtraMissions);
        foreach (List<MissionData> prsets in PresetMissions) prsets.Clear();

        // Load missions from text
        string rawPreset = Resources.Load<TextAsset>("PresetMissions").text;
        string[] rawMissions = rawPreset.Split("\n"[0]);

        // Analyse a row of string as a mission
        foreach (string rawMission in rawMissions)
        {
            MissionData baseMissionData = new MissionData();

            // Get cells of a mission: name, easy, mid, hard, extra
            string[] cells = rawMission.Split(","[0]);
            string[] cons = cells[0].Split("_"[0]);

            // Get a mission prototype according to its type and other conditions
            if (Enum.TryParse(cons[0], out MissionData.Type type))
            {
                baseMissionData.MissionType = type;
                for (int i = 1; i < cons.Length; i++)
                {
                    if (cons[i] == "isETW") baseMissionData.isETW = true;
                    else if (cons[i] == "isChallenge") baseMissionData.isChallenge = true;
                    else if (cons[i] == "noUndo") baseMissionData.noUndo = true;
                    else if (cons[i] == "noHint") baseMissionData.noHint = true;
                    else if (cons[i] == "noShuffle") baseMissionData.noShuffle = true;
                    else if (cons[i] == "isContinuous") baseMissionData.isContinuous = true;
                }

                // Generate missions by difficulies
                for (int i = 1; i < cells.Length; i++)
                {
                    if (int.TryParse(cells[i], out int progress) && progress != 0)
                    {
                        MissionData realMissionData = new MissionData(baseMissionData);
                        realMissionData.MaxProgress = int.Parse(cells[i]);
                        realMissionData.Difficulty = i - 1;
                        PresetMissions[i - 1].Add(realMissionData);
                    }
                }
            }
        }
        PresetMissionsCount = 0;
        foreach (List<MissionData> prsets in PresetMissions) PresetMissionsCount += prsets.Count;
    }

    public void LoadPresetRewards()
    {
        // Group rewards by difficulty, 0 = easy
        PresetRewards.Clear();
        PresetRewards.Add(EasyRewards);
        PresetRewards.Add(MidRewards);
        PresetRewards.Add(HardRewards);
        PresetRewards.Add(ExtraRewards);
        foreach (List<RewardData> prsets in PresetRewards) prsets.Clear();

        // Load rewards from text
        string rawPreset = Resources.Load<TextAsset>("PresetRewards").text;
        string[] rawRewards = rawPreset.Split("\n"[0]);

        // Analyse a row of string as a reward
        foreach (string rawReward in rawRewards)
        {
            RewardData baseRewardData = new RewardData();

            // Get cells of a reward: name, easy, mid, hard, extra
            string[] cells = rawReward.Split(","[0]);

            // Get a reward prototype according to its type
            if (Enum.TryParse(cells[0], out RewardData.Type type))
            {
                baseRewardData.RewardType = type;
                // Generate missions by difficulies
                for (int i = 1; i < cells.Length; i++)
                {
                    if (int.TryParse(cells[i], out int progress) && progress != 0)
                    {
                        RewardData realRewardData = new RewardData(baseRewardData);
                        realRewardData.Count = int.Parse(cells[i]);
                        PresetRewards[i - 1].Add(realRewardData);
                    }
                }
            }
        }

        PresetRewardsCount = 0;
        foreach (List<RewardData> prsets in PresetRewards) PresetRewardsCount += prsets.Count;
    }

    public void CheckToAddNew(bool forced = false)
    {
        if (forced || DateTime.Now.Date.Equals(MissionUpdateDate.Date) && DateTime.Now.Hour >= UpdateHours || (DateTime.Now - MissionUpdateDate).TotalHours >= 24)
        {
            MissionUpdateDate = DateTime.Now;

            foreach (Mission mission in CurMissions) mission.Clear();
            CurMissions.Clear();

            // Add the done other mission.
            if (!hasDoneOther)
            {
                MissionData missionData = new MissionData();
                missionData.MissionType = MissionData.Type.doneOther;
                missionData.Difficulty = 3;
                Mission mission = new Mission(missionData);
                CurMissions.Add(mission);
            }
            while (CurMissions.Count < MaxMissions)
            {
                int randomDifficulty = UnityEngine.Random.Range(0, 4);
                ///Debug.Log("diff: " + randomDifficulty);
                //Debug.Log("presets: " + PresetMissions.Count);
                List<MissionData> randomMissions  = PresetMissions[randomDifficulty];
                //Debug.Log("diff group: " + randomMissions.Count);
                MissionData randomMission = randomMissions[UnityEngine.Random.Range(0, randomMissions.Count)];
                //Debug.Log("mission: " + randomMission.MissionType);
                Mission mission = new Mission(randomMission);
                CurMissions.Add(mission);
            }
        }
    }
    bool hasDoneOther
    {
        get
        {
            foreach (Mission mission in CurMissions)
            {
                if (mission.MissionData.MissionType == MissionData.Type.doneOther) return true;
            }
            return false;
        }

    }
    public void Collect(Mission mission)
    {

        RewardData rewardData = mission.MissionData.RewardData;
        CurMissions.Remove(mission);
        MissionManager.Instance.OnDoneMission?.Invoke();
        Collect(rewardData);
        mission.Clear();

    }

    public void Collect(RewardData rewardData)
    {
        switch (rewardData.RewardType)
        {
            case RewardData.Type.deck:
                SkinManager.Instance.DeckSkinDatas[rewardData.SkinId].Durability += rewardData.Count;
                break;
            case RewardData.Type.back:
                SkinManager.Instance.BackSkinDatas[rewardData.SkinId].Durability += rewardData.Count;
                break;
            case RewardData.Type.shuffle:
                GameManager.Instance.GameData.ShuffleCount += rewardData.Count;
                break;
            case RewardData.Type.hint:
                GameManager.Instance.GameData.HintCount += rewardData.Count;
                break;
            case RewardData.Type.both:
                GameManager.Instance.GameData.HintCount += rewardData.Count / 10;
                GameManager.Instance.GameData.ShuffleCount += rewardData.Count % 10;
                break;
        }
        OnCollect?.Invoke();
    }
}

[System.Serializable]
public class Mission
{
    public MissionData MissionData = new MissionData();
    GameManager.Handler Done;
    GameManager.Handler DoneWithCons;
    GameManager.Handler DoneOther;
    public Mission(MissionData missionData)
    {
        MissionData = new MissionData(missionData);

        Done = new GameManager.Handler(_done);
        DoneWithCons = new GameManager.Handler(_doneWithCons);
        DoneOther = new GameManager.Handler(_doneOther);
        switch (MissionData.MissionType)
        {
            case MissionData.Type.getDeck:
                SkinManager.Instance.OnGetDeck += Done;
                break;
            case MissionData.Type.getBack:
                SkinManager.Instance.OnGetBack += Done;
                break;
            case MissionData.Type.useDeck:
                SkinManager.Instance.OnUseDeck += Done;
                break;
            case MissionData.Type.useBack:
                SkinManager.Instance.OnUseBack += Done;
                break;
            case MissionData.Type.playGame:
                ViewManager.Instance.OnStartNew += DoneWithCons;
                break;
            case MissionData.Type.winGame:
                ViewManager.Instance.OnWin += DoneWithCons;
                break;
            case MissionData.Type.useHint:
                HintManager.Instance.OnHint += Done;
                break;
            case MissionData.Type.useShuffle:
                ViewManager.Instance.OnShuffle += Done;
                break;
            case MissionData.Type.openDaily:
                MissionManager.Instance.OnOpenDaily += Done;
                break;
            case MissionData.Type.doneOther:
                MissionData.MaxProgress = MissionManager.Instance.MaxMissions - 1;
                MissionManager.Instance.OnDoneMission += DoneOther;
                break;
            case MissionData.Type.doneEO:
                MissionManager.Instance.OneDoneEO += Done;
                break;
        }

        //if (!initialiseReward) return;
        List<RewardData> rewardPool = new List<RewardData>();
        if (MissionData.Difficulty == 0) rewardPool = MissionManager.Instance.EasyRewards;
        else if (MissionData.Difficulty == 1) rewardPool = MissionManager.Instance.MidRewards;
        else if (MissionData.Difficulty == 2) rewardPool = MissionManager.Instance.HardRewards;
        else if (MissionData.Difficulty == 3) rewardPool = MissionManager.Instance.ExtraRewards;
        RewardData rewardData = rewardPool[UnityEngine.Random.Range(0, rewardPool.Count)];
        rewardData.Initialise();
        MissionData.RewardData = rewardData;
    }

    void _done()
    {
        MissionData.Progress++;
        CheckToComplete();
    }
    void _doneWithCons()
    {
        if (MissionData.isETW && GameManager.Instance.GameData.GameMode != GameData.Mode.easy) return;
        if (MissionData.isChallenge && GameManager.Instance.GameData.GameMode != GameData.Mode.challenge) return;
        if (MissionData.noUndo && GameManager.Instance.GameData.UndoUses > 0) return;
        if (MissionData.noHint && GameManager.Instance.GameData.HintUses > 0) return;
        if (MissionData.noShuffle && GameManager.Instance.GameData.ShuffleUses > 0) return;
        if (MissionData.isContinuous) MissionData.Progress = GameManager.Instance.GameData.WinningStreak;
        MissionData.Progress++;
        CheckToComplete();
    }
    void _doneOther()
    {
        MissionData.MaxProgress = MissionManager.Instance.MaxMissions - 1;
        MissionData.Progress = MissionData.MaxProgress - MissionManager.Instance.CurMissions.Count + 1;
        //if (MissionManager.Instance.CurMissions.Count == 1 && MissionManager.Instance.CurMissions[0] == this)
        //    MissionData.Progress = MissionData.MaxProgress;
        CheckToComplete();
    }
    public void CheckToComplete()
    {
        MissionManager.Instance.OnDoneProgress?.Invoke();
        if (MissionData.Progress == MissionData.MaxProgress)
        {
            Debug.LogWarning("You've completed a mission!");
        }
    }

    public void Clear()
    {
        switch (MissionData.MissionType)
        {
            case MissionData.Type.getDeck:
                SkinManager.Instance.OnGetDeck -= Done;
                break;
            case MissionData.Type.getBack:
                SkinManager.Instance.OnGetBack -= Done;
                break;
            case MissionData.Type.useDeck:
                SkinManager.Instance.OnUseDeck -= Done;
                break;
            case MissionData.Type.useBack:
                SkinManager.Instance.OnUseBack -= Done;
                break;
            case MissionData.Type.playGame:
                ViewManager.Instance.OnStartNew -= DoneWithCons;
                break;
            case MissionData.Type.winGame:
                ViewManager.Instance.OnWin -= DoneWithCons;
                break;
            case MissionData.Type.useHint:
                HintManager.Instance.OnHint -= DoneWithCons;
                break;
            case MissionData.Type.useShuffle:
                ViewManager.Instance.OnShuffle -= Done;
                break;
            case MissionData.Type.openDaily:
                MissionManager.Instance.OnOpenDaily -= Done;
                break;
            case MissionData.Type.doneOther:
                MissionManager.Instance.OnDoneMission -= DoneOther;
                break;
            case MissionData.Type.doneEO:
                MissionManager.Instance.OneDoneEO -= Done;
                break;
        }
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

    public RewardData RewardData = null;

    public MissionData()
    {

    }

    public MissionData(MissionData oriMissionData)
    {
        MissionType = oriMissionData.MissionType;
        isETW = oriMissionData.isETW;
        isChallenge = oriMissionData.isChallenge;
        noUndo = oriMissionData.noUndo;
        noHint = oriMissionData.noHint;
        noShuffle = oriMissionData.noShuffle;
        isContinuous = oriMissionData.isContinuous;
        Progress = oriMissionData.Progress;
        MaxProgress = oriMissionData.MaxProgress;
        Difficulty = oriMissionData.Difficulty;
        RewardData = null;
    }
}

[System.Serializable]
public class RewardData
{
    public enum Type { none, deck, back, hint, shuffle, both }
    public Type RewardType = Type.none;
    public int SkinId = 0;
    public int Count = 0;
    public RewardData()
    {
        RewardType = Type.none;
        SkinId = 0;
        Count = 0;
    }
    public RewardData(RewardData oriRewardData)
    {
        RewardType = oriRewardData.RewardType;
        SkinId = oriRewardData.SkinId;
        Count = oriRewardData.Count;
    }

    public void Initialise()
    {
        if (RewardType == Type.deck)
        {
            SkinId = UnityEngine.Random.Range(1, SkinManager.Instance.DeckSkins.Count);
        }
        else if (RewardType == Type.back)
        {
            SkinId = UnityEngine.Random.Range(1, SkinManager.Instance.BackSkins.Count);
        }
    }
}

[System.Serializable]
public class ExclusiveOffer
{
    public RewardData RewardData = new RewardData();
    public DateTime LastUpdateTime = new DateTime();
    public bool IsCooldown = false;
    public DateTime NextUpdateTime = new DateTime();
    public int CooldownMinutes = 0;
}