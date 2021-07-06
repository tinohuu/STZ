using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Game Configuration")]
    public float SnapDistance = 180;
    public GameObject CheatText;

    [Header("Game Data")]
    public Save Save = null;
    public GameData GameData = null;
    public SettingsData SettingsData = null;
    public StatisticsData StatisticsData = null;

    [Header("Game Inspected")]
    //public int Moves = 0;
    public float Time = -1;
    public bool IsLocked = false;
    public bool IsAnyToEmptyPile = false;

    public delegate void MovesHandler();
    public event MovesHandler OnMove = null;
    //int _score = 0;
    //int _moves = 0;
    public static GameManager Instance;

    private void Awake()
    {
        Save = SaveSystem.Load();
        if (Save == null)
        {
            //GameData = new GameData();
            //SettingsData = new SettingsData();
            //StatisticsData = new StatisticsData();
        }
        else
        {
            GameData = Save.GameData;
            SettingsData = Save.SettingsData;
            StatisticsData = Save.StatisticsData;
        }

        Instance = this;
    }
    void Start()
    {
        OnMove += new MovesHandler(StartCountTime);
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) Application.Quit();
        if (Input.GetKeyDown(KeyCode.C))
        {
            IsAnyToEmptyPile = !IsAnyToEmptyPile;
            CheatText.SetActive(IsAnyToEmptyPile);
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            bool isSaved = SaveSystem.Save();
            Debug.Log("Saved " + isSaved);
        }
    }
    public int Score
    {
        get
        {
            return GameData.Score;
        }
        set
        {
            GameData.Score = value < 0 ? 0 : value;
        }
    }
    public int Moves
    {
        get
        {
            return GameData.Moves;
        }
        set
        {
            GameData.Moves = value < 0 ? 0 : value;
            if (OnMove != null) OnMove.Invoke();
        }
    }
    public void SetDrawCards(int number)
    {
        SettingsData.DrawCards = number;
    }
    public void StartCountTime()
    {
        if (Time == -1) Time = UnityEngine.Time.time;
    }
    public void SetDrawCards(bool isThree)
    {
        SettingsData.DrawCards = isThree? 3 : 1;
    }
}

[System.Serializable]
public class GameData
{
    public int Score = 0;
    public int Moves = 0;
    public float Time = 0;
    public int UndoUses = 0;
    public int HintUses = 0;
}


[System.Serializable]
public class SettingsData
{
    public int DrawCards = 1;
    public bool IsRightHand = true;
    public float AnimationSpeed = 20;
    public bool Info = true;
    public bool Volume = true;
    public bool IsEoExpanded = false;
}

[System.Serializable]
public class StatisticsData
{
    public int GameWon = 0;
    public int GameWon3 = 0;
    public int GamesPlayed = 0;
    public int GamesPlayed3 = 0;
    public int BestScore = 0;
    public int BestScore3 = 0;
    public int BestTime = 0;
    public int BestTime3 = 0;
    public int TotalTime = 0;
    public int TotalTime3 = 0;
}
