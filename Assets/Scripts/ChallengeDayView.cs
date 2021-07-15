using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChallengeDayView : MonoBehaviour
{
    [Header("References")]
    public GameObject View;
    public Image Circle;
    public Image CircleLine;
    public Text DayText;
    public GameObject Crown;
    public Button Button;
    public Color CloseColor = Color.grey;
    public Color ForcedOpenColor = Color.yellow;
    public Color OpenColor = Color.green;
    [Header("Inspected")]
    public ChallengeDayData Data = null;
    public void UpdateView()
    {
        Circle.color =  Data.IsForcedOpen ? ForcedOpenColor : IsOpen ? OpenColor : CloseColor;
        CircleLine.color = Data.IsForcedOpen ? ForcedOpenColor : IsOpen ? OpenColor : CloseColor;
        Circle.gameObject.SetActive(ChallengeManager.Instance.CurDayView == this);
        CircleLine.gameObject.SetActive(ChallengeManager.Instance.CurDayView != this && Data.Date.Date <= DateTime.Now.Date);
        Button.interactable = Data.Date.Date <= DateTime.Now.Date;
        //Debug.Log(Data.Date.Date + "===" + DateTime.Now.Date + "===" + CircleLine.gameObject.activeSelf);
        DayText.gameObject.SetActive(!Data.IsSolved);
        DayText.color = ChallengeManager.Instance.CurDayView == this ? Color.white : Data.Date.Date <= DateTime.Now.Date ? Color.black : Color.grey;
        Crown.SetActive(Data.IsSolved);
    }
    public void Select()
    {
        Debug.LogWarning("Select");
        ChallengeManager.Instance.Select(this);
    }

    public bool IsOpen
    {
        get => Data.Date.Date == DateTime.Now.Date || ChallengeManager.Instance.Data.IsSpecialEvent;
    }

    public void SetDate(DateTime data)
    {
        foreach (ChallengeDayData dayData in ChallengeManager.Instance.Data.ChallengeDayDatas)
        {
            if (dayData.Date == data.Date)
            {
                Data = dayData;
                return;
            }
        }
        Data = new ChallengeDayData();
        Data.Date = data.Date;
        return;
    }
}
