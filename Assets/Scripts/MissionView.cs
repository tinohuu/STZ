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
    public Image ProgressFill;
    public TMP_Text ProgressText;
    public Button CollectButton;

    float? targetProgressFill = null;
    public void Start()
    {
        MissionManager.Instance.OnDoneProgress += new GameManager.Handler(DoneProgress);
    }
    private void OnDestroy()
    {
        MissionManager.Instance.OnDoneProgress -= new GameManager.Handler(DoneProgress);
    }
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

        string rewardText = "";
        if (rewardType == RewardData.Type.deck || rewardType == RewardData.Type.back) rewardText = "Durability x";
        else if (rewardType == RewardData.Type.hint) rewardText = "Hint x";
        else if (rewardType == RewardData.Type.shuffle) rewardText = "Shuffle x";
        rewardText += rewardData.Count;
        CountText.text = rewardText;
        HintCountText.text = "x" + rewardData.Count / 10;
        ShuffleCountText.text = "x" + rewardData.Count % 10;

        if (rewardType == RewardData.Type.deck) DeckSkinImage.sprite = SkinManager.Instance.DeckSkins[rewardData.SkinId].BoxSprite;
        if (rewardType == RewardData.Type.back) BackSkinImage.sprite = SkinManager.Instance.BackSkins[rewardData.SkinId].Main;

        // Mission Info
        string missionInfo = "";
        switch (Mission.MissionData.MissionType)
        {
            case MissionData.Type.getDeck:
                missionInfo = string.Format("Increase the durability of {0} deck", Mission.MissionData.MaxProgress);
                break;
            case MissionData.Type.getBack:
                missionInfo = string.Format("Increase the durability of {0} background", Mission.MissionData.MaxProgress);
                break;
            case MissionData.Type.useDeck:
                missionInfo = string.Format("Use {0} deck", Mission.MissionData.MaxProgress);
                break;
            case MissionData.Type.useBack:
                missionInfo = string.Format("Use {0} background", Mission.MissionData.MaxProgress);
                break;
            case MissionData.Type.useHint:
                missionInfo = string.Format("Use {0} hint", Mission.MissionData.MaxProgress);
                break;
            case MissionData.Type.useShuffle:
                missionInfo = string.Format("Use {0} shuffle", Mission.MissionData.MaxProgress);
                break;
            case MissionData.Type.playGame:
                missionInfo = string.Format("Play {0} game", Mission.MissionData.MaxProgress);
                break;
            case MissionData.Type.winGame:
                missionInfo = string.Format("Win {0} game", Mission.MissionData.MaxProgress);
                break;
            case MissionData.Type.doneOther:
                missionInfo = string.Format("Complete all mission", Mission.MissionData.MaxProgress);
                break;
            case MissionData.Type.openDaily:
                missionInfo = string.Format("Open Daily Missions", Mission.MissionData.MaxProgress);
                break;
            case MissionData.Type.doneEO:
                missionInfo = string.Format("Earn {0} Exclusive Offer", Mission.MissionData.MaxProgress);
                break;
        }
        if (Mission.MissionData.isChallenge) missionInfo = missionInfo.Replace("game", "Challenge Level");
        if (Mission.MissionData.isETW) missionInfo = missionInfo.Replace("game", "Easy to Win game");
        missionInfo += Mission.MissionData.MaxProgress > 1 ? "s" : "";
        if (Mission.MissionData.isContinuous) missionInfo += " in a row";
        if (Mission.MissionData.noHint) missionInfo += " without using hint";
        if (Mission.MissionData.noUndo) missionInfo += " without using undo";
        if (Mission.MissionData.noShuffle) missionInfo += " without using shuffle";
        MissionInfo.text = missionInfo;

        DoneProgress();
    }

    public void DoneProgress()
    {
        targetProgressFill = (float)Mission.MissionData.Progress / Mission.MissionData.MaxProgress;
        ProgressText.text = Mathf.Clamp(Mission.MissionData.Progress, 0, Mission.MissionData.MaxProgress) + " / " + Mission.MissionData.MaxProgress;
        CollectButton.interactable = Mission.MissionData.Progress >= Mission.MissionData.MaxProgress;
    }

    public void Update()
    {
        if (targetProgressFill != null)
        {
            ProgressFill.fillAmount = Mathf.Lerp(ProgressFill.fillAmount, (float)targetProgressFill, Time.deltaTime * 2);
            if (Mathf.Abs(ProgressFill.fillAmount - (float)targetProgressFill) < 0.01f)
            {
                ProgressFill.fillAmount = (float)targetProgressFill;
                targetProgressFill = null;
            }
        }
    }

    public void Collect()
    {
        MissionManager.Instance.Collect(Mission);
        MissionViewManager.Instance.UpdateView();
    }
}
