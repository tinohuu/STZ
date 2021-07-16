using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Game Configuration")]
    public float SnapDistance = 180;
    public Vector2 AnimSpeed = new Vector2(20, 30);
    public GameObject CheatText;

    [Header("Game Data")]
    public Save Save = null;
    public GameData GameData = null;
    public SettingsData SettingsData = null;
    public StatisticsData StatisticsData = null;

    [Header("Game Inspected")]
    //public int Moves = 0;
    //public float Time = -1;
    public bool IsLocked = false;
    public bool IsCheated = false;

    public event Handler OnMove = null;
    //int _score = 0;
    //int _moves = 0;
    public static GameManager Instance;
    public delegate void Handler();
    private void Awake()
    {
        if (!Directory.Exists(Application.persistentDataPath + "/Saves"))
            Directory.CreateDirectory(Application.persistentDataPath + "/Saves");

        Save = SaveSystem.Load();
        if (Save != null)
        {
            GameData = Save.GameData;
            SettingsData = Save.SettingsData;
            StatisticsData = Save.StatisticsData;
            //Debug.LogWarning("Save Time:" + GameData.DateTime);
        }

        Instance = this;
    }
    void Start()
    {
        OnMove += new Handler(StartCountTime);
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SaveSystem.Save();
            Application.Quit();
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            IsCheated = !IsCheated;
            CheatText.SetActive(IsCheated);
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
        if (GameData.Time == -1) GameData.Time = UnityEngine.Time.time;
    }
    public void SetDrawCards(bool isThree)
    {
        SettingsData.DrawCards = isThree? 3 : 1;
    }

}

[System.Serializable]
public class GameData
{
    public enum Mode { normal, easy, challenge }
    public Mode GameMode = Mode.normal;
    int _score = 0;
    public int Moves = 0;
    public float Time = -1;
    public int UndoUses = 0;
    public int HintUses = 0;
    public int ShuffleUses = 0;
    public int WinningStreak = 0;
    public int ShuffleCount = 0;
    public int HintCount = 0;
    public bool IsWon = false;

    public int Score
    {
        set { _score = value < 0 ? 0 : value; }
        get => _score;
    }
}


[System.Serializable]
public class SettingsData
{
    public int DrawCards = 1;
    public bool IsRightHand = true;
    public float AnimationSpeed = 20;
    public bool ShowInfo = true;
    public bool IsMuted = true;
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
