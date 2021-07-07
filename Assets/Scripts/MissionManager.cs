using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionManager : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

[System.Serializable]
public class Mission
{
    public enum Type { none, getDeck, getBack, playGame, winGame, winGameStreak, useItem, useDeck, useBack, getEO }
    public Type ConType = Type.none;

    public int Count = 0;

    public enum GameType { none, easyToWin, newGame }
    public GameType ConGameTime = GameType.none;
    public bool GameWithoutItem = false;
    public Reward.Type ItemType = 0;

    public int Progress = 0;
    public int MaxProgress = 0;


}

[System.Serializable]
public class Reward
{
    public enum Type { none, undo, hint, shuffle, deck, back }
    public Type RewardType = Type.none;
}
