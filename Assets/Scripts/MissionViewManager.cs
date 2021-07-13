using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionViewManager : MonoBehaviour
{
    public GameObject MissionViewPrefab;
    public Transform MissionContent;
    public static MissionViewManager Instance;
    private void Awake()
    {
        Instance = this;
    }

    public void UpdateView()
    {
        MissionContent.DestoryChildren();
        foreach(Mission mission in MissionManager.Instance.CurMissions)
        {
            MissionView missionView = Instantiate(MissionViewPrefab, MissionContent).GetComponent<MissionView>();
            missionView.Mission = mission;
            missionView.UpdateView();
        }
    }
}
