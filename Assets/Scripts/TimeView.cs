using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeView : MonoBehaviour
{
    GameManager gameManager;
    float NextTimeScore = 30;
    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
    }
    void Update()
    {
        if (gameManager.Time >= 0)
        {
            if (Time.time - gameManager.Time >= NextTimeScore)
            {
                gameManager.Score -= 5;
                NextTimeScore += 30;
            }

            float gameTime = Time.time - gameManager.Time;
            int minute = (int)gameTime / 60;
            string minuteText = minute >= 10 ? minute.ToString() : "0" + minute;
            int second = (int)gameTime % 60;
            string secondText = second >= 10 ? second.ToString() : "0" + second;
            GetComponent<Text>().text = "Time: " + minuteText + ":" + secondText;
        }
    }
}
