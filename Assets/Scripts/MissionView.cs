using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MissionView : MonoBehaviour
{
    [Header("References")]
    public GameObject DeckSkin;
    public Image DeckSkinImage;
    public GameObject BackSkin;
    public Image BackSkinImage;
    public GameObject Hint;
    public GameObject Shuffle;
    public GameObject Both;
    public TMP_Text CountText;
    public TMP_Text HintCountText;
    public TMP_Text ShuffleCountText;
    public Mission Mission = null;
    public TMP_Text MissionInfo;

    public void UpdateView()
    {
        // Reward
        RewardData rewardData = Mission.MissionData.RewardData;
        RewardData.Type rewardType = rewardData.RewardType;
        DeckSkin.SetActive(rewardType == RewardData.Type.deck);
        BackSkin.SetActive(rewardType == RewardData.Type.back);
        Hint.SetActive(rewardType == RewardData.Type.hint);
        Shuffle.SetActive(rewardType == RewardData.Type.shuffle);
        Both.SetActive(rewardType == RewardData.Type.both);
        CountText.gameObject.SetActive(rewardType != RewardData.Type.both);
        HintCountText.gameObject.SetActive(rewardType == RewardData.Type.both);
        ShuffleCountText.gameObject.SetActive(rewardType == RewardData.Type.both);

        CountText.text = rewardType.ToString() + rewardData.Count;
        HintCountText.text = "x" + rewardData.Count / 10;
        ShuffleCountText.text = "x" + rewardData.Count % 10;

        // Mission Info
        string missionInfo;
        switch (Mission.MissionData.MissionType)
        {
            case MissionData.Type.getDeck:
                missionInfo = "Get";
                break;
            case MissionData.Type.getBack:
                break;
        }
    }
}
