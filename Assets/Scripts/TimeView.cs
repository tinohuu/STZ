using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeView : MonoBehaviour
{
    float NextTimeScore = 30;
    private void Awake()
    {
    }
    void Update()
    {
        if (GameManager.Instance.GameData.Time >= 0 && !GameManager.Instance.GameData.IsWon)
        {
            if (Time.time - GameManager.Instance.GameData.Time >= NextTimeScore)
            {
                GameManager.Instance.GameData.Score -= 5;
                NextTimeScore += 30;
            }

            float gameTime = Time.time - GameManager.Instance.GameData.Time;
            int minute = (int)gameTime / 60;
            string minuteText = minute >= 10 ? minute.ToString() : "0" + minute;
            int second = (int)gameTime % 60;
            string secondText = second >= 10 ? second.ToString() : "0" + second;
            GetComponent<Text>().text = "Time: " + minuteText + ":" + secondText;
        }
    }
}
