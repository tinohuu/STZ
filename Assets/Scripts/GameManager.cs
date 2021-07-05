using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int DrawCards = 1;
    public float SnapDistance = 180;
    public float AnimationSpeed = 20;
    public float InitialTime = -1;
    public int Moves = 0;
    int _score = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) Application.Quit();
    }

    public int Score
    {
        get
        {
            return _score;
        }
        set
        {
            _score = value < 0 ? 0 : value;
        }
    }
    public void SetDrawCards(int number)
    {
        DrawCards = number;
    }
    public void StartCountTime()
    {
        if (InitialTime == -1) InitialTime = Time.time;
    }
}
