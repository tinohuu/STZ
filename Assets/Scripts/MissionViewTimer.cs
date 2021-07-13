using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MissionViewTimer : MonoBehaviour
{
    TMP_Text text;
    void Start()
    {
        text = GetComponent<TMP_Text>();
    }

    // Update is called once per frame
    void Update()
    {
        DateTime updateDateTime = DateTime.Now.Date.AddHours(MissionManager.Instance.UpdateHours);
        if ((DateTime.Now - MissionManager.Instance.MissionUpdateDate).TotalHours > 0) updateDateTime = updateDateTime.AddDays(1);
        text.text = "Time Left: " + (updateDateTime - DateTime.Now).ToString("hh':'mm':'ss");
    }
}
