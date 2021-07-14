using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionViewManager : MonoBehaviour
{
    public GameObject MissionViewPrefab;
    public Transform MissionContent;
    public List<EOView> EOViews;
    public static MissionViewManager Instance;
    private void Awake()
    {
        Instance = this;

        for (int i = 0; i < EOViews.Count; i++)
        {
            if (GameManager.Instance.Save != null)
            {
                EOViews[i].Offer = GameManager.Instance.Save.ExclusiveOffers[i];
                EOViews[i].UpdateView();
            }
            else EOViews[i].Renew();
        }
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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M) && GameManager.Instance.IsCheated)
        {
            MissionManager.Instance.CheckToAddNew(true);
            UpdateView();
        }
    }
}
