using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChallengeManager : MonoBehaviour
{
    [Header("Config")]
    public int SpecialEventDuration = 2;
    [Header("References")]
    public GameObject ChallengeDayViewPrefab;
    public Transform ChallengeDayParent;
    public Text MonthText;
    public Text CrownCountText;
    public Image TrophyProgressImage;
    public GameObject Crown;
    public GameObject NoCrown;
    public GameObject WatchButton;
    public GameObject RedealtButton;
    public Text DateText;
    public Text SolveText;
    public Text TimeText;
    public Text ScoreText;
    public Text MovesText;

    float TargetTrophyProgress = 0;
    [Header("Inspected")]
    public ChallengeDayView CurDayView = null;
    public List<ChallengeDayView> AllDayViews = new List<ChallengeDayView>();
    public List<ChallengeDayView> EnabledDayViews = new List<ChallengeDayView>();
    public ChallengeManagerData Data;

    public static ChallengeManager Instance;
    DateTime ViewedDate = DateTime.Now;
    private void Awake()
    {
        Instance = this;
        if (GameManager.Instance.Save != null)
        {
            Data = GameManager.Instance.Save.ChallengeManagerData;
        }
    }
    private void Start()
    {
        ViewManager.Instance.OnStartNew += new GameManager.Handler(LoseChallenge);
        ViewManager.Instance.OnWin += new GameManager.Handler(WinChallenge);
        CreateCalendar();
        UpdateCalendar();
    }
    private void Update()
    {
        TrophyProgressImage.fillAmount = Mathf.Lerp(TrophyProgressImage.fillAmount, TargetTrophyProgress, Time.deltaTime * 5);

        if (Input.GetKeyDown(KeyCode.W) && CurDayView && GameManager.Instance.IsCheated)
        {
            Data.PlayedDayData = CurDayView.Data;
            WinChallenge();
        }
    }
    void CreateCalendar()
    {
        ChallengeDayParent.DestoryChildren();
        for (int i = 0; i < 42; i++)
        {
            AllDayViews.Add(Instantiate(ChallengeDayViewPrefab, ChallengeDayParent).GetComponent<ChallengeDayView>());
        }
    }
    void UpdateCalendar()
    {
        MonthText.text = ViewedDate.ToString("MMMM yyyy");
        if (ViewedDate > DateTime.Now.Date) CurDayView = null;
        DateTime _dateTime = ViewedDate;
        DateTime firstDay = _dateTime.AddDays(-(_dateTime.Day - 1));
        int index = GetDays(firstDay.DayOfWeek);

        int date = 0;
        int crownCount = 0;
        List<ChallengeDayView> enabled = new List<ChallengeDayView>();

        for (int i = 0; i < 42; i++)
        {
            Text label = AllDayViews[i].DayText;
            AllDayViews[i].View.SetActive(false);
            enabled.Add(AllDayViews[i]);
            if (i >= index)
            {
                DateTime thatDay = firstDay.AddDays(date);
                if (thatDay.Month == firstDay.Month)
                {
                    AllDayViews[i].View.SetActive(true);
                    AllDayViews[i].SetDate(thatDay);

                    if (AllDayViews[i].Data.IsSolved) crownCount++;

                    label.text = (date + 1).ToString();
                    date++;
                }
            }
        }

        EnabledDayViews = enabled;
        foreach (ChallengeDayView v in EnabledDayViews) v.UpdateView();

        CrownCountText.text = crownCount + "/" + (date).ToString();
        TargetTrophyProgress = (float)crownCount / (date);
        //_yearNumText.text = _dateTime.Year.ToString();
        //_monthNumText.text = _dateTime.Month.ToString();
        UpdateDayInfo();
    }
    public void AddViewedMonth(int i)
    {
        ViewedDate = ViewedDate.AddMonths(i);
        UpdateCalendar();
    }
    int GetDays(DayOfWeek day)
    {
        switch (day)
        {
            case DayOfWeek.Monday: return 1;
            case DayOfWeek.Tuesday: return 2;
            case DayOfWeek.Wednesday: return 3;
            case DayOfWeek.Thursday: return 4;
            case DayOfWeek.Friday: return 5;
            case DayOfWeek.Saturday: return 6;
            case DayOfWeek.Sunday: return 0;
        }

        return 0;
    }
    public void Select(ChallengeDayView view)
    {
        CurDayView = view;
        foreach (ChallengeDayView v in EnabledDayViews) v.UpdateView();
        UpdateDayInfo();
    }

    public void UpdateDayInfo()
    {
        if (!CurDayView)
        {
            Crown.SetActive(false);
            NoCrown.SetActive(false);
            DateText.text = "";
            SolveText.text = "Select a Date";
            TimeText.text = "";
            ScoreText.text = "";
            MovesText.text = "";
            WatchButton.SetActive(false);
            RedealtButton.SetActive(true);
            RedealtButton.GetComponent<Button>().interactable = false;
            return;
        }
        Crown.SetActive(CurDayView.Data.IsSolved);
        NoCrown.SetActive(!CurDayView.Data.IsSolved);
        DateText.text = CurDayView.Data.Date.ToString("MMM d");
        SolveText.text = CurDayView.Data.IsSolved ? "Solved" : "Unsolved";
        TimeText.text = "Time:" + ((int)CurDayView.Data.Duration / 60).ToString("D2") + ":" + ((int)CurDayView.Data.Duration % 60).ToString("D2");
        ScoreText.text = "Score: " + CurDayView.Data.Score;
        MovesText.text = "Moves: " + CurDayView.Data.Moves;
        WatchButton.SetActive(!CurDayView.Data.IsForcedOpen && !CurDayView.IsOpen);
        RedealtButton.SetActive(CurDayView.Data.IsForcedOpen || CurDayView.IsOpen);
        RedealtButton.GetComponent<Button>().interactable = true;
    }

    public void WinChallenge()
    {
        if (GameManager.Instance.GameData.GameMode == GameData.Mode.challenge  || GameManager.Instance.IsCheated)
        {
            if (PlayedDayData == null) return;
            if (!Data.ChallengeDayDatas.Contains(PlayedDayData)) Data.ChallengeDayDatas.Add(PlayedDayData);
            PlayedDayData.IsSolved = true;
            PlayedDayData.Duration = GameManager.Instance.GameData.Time;
            PlayedDayData.Score = GameManager.Instance.GameData.Score;
            PlayedDayData.Moves = GameManager.Instance.GameData.Moves;
        }
        UpdateCalendar();
    }
    public void LoseChallenge()
    {
        if (GameManager.Instance.GameData.GameMode != GameData.Mode.challenge) Data.PlayedDayData = null;
    }
    public void ForceOpen()
    {
        //ViewManager.Instance.Ad.SetActive(true);
        CurDayView.Data.IsForcedOpen = true;
        if (!Data.ChallengeDayDatas.Contains(CurDayView.Data)) Data.ChallengeDayDatas.Add(CurDayView.Data);
        foreach (ChallengeDayView v in EnabledDayViews) v.UpdateView();
        UpdateDayInfo();
    }
    public void StartChallenge()
    {
        Data.PlayedDayData = CurDayView.Data;
        if (!Data.ChallengeDayDatas.Contains(CurDayView.Data)) Data.ChallengeDayDatas.Add(CurDayView.Data);
        ViewManager.Instance.StartGame(GameData.Mode.challenge);
    }

    ChallengeDayData PlayedDayData => Data.PlayedDayData;
}

[Serializable]
public class ChallengeDayData
{
    public DateTime Date = new DateTime();
    public bool IsSolved = false;
    public bool IsForcedOpen = false;
    public float Duration = 0;
    public int Score = 0;
    public int Moves = 0;
}

[Serializable]
public class ChallengeManagerData
{
    public ChallengeDayData PlayedDayData = null;
    public List<ChallengeDayData> ChallengeDayDatas = new List<ChallengeDayData>();
    public bool IsSpecialEvent = false;
    public DateTime NextSpecialEventTime = new DateTime();
}

