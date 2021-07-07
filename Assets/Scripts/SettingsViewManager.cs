using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsViewManager : MonoBehaviour
{
    public SettingView Expand = null;
    public SettingView DrawCards = null;
    public SettingView RightHand = null;
    public SettingView Info = null;
    public SettingView AnimSpeed = null;
    public SettingView Volume = null;
    public static SettingsViewManager Instance;
    private void Awake()
    {
        Instance = this;
        if (GameManager.Instance.SettingsData != null)
        {
            Expand.SwapData = GameManager.Instance.SettingsData.IsEoExpanded;
            DrawCards.ToggleData = GameManager.Instance.SettingsData.DrawCards == 3;
            RightHand.ToggleData = GameManager.Instance.SettingsData.IsRightHand;
            Info.ToggleData = GameManager.Instance.SettingsData.ShowInfo;
            Volume.SwapData = GameManager.Instance.SettingsData.IsMuted;
            AnimSpeed.MultipleData = GameManager.Instance.SettingsData.AnimationSpeed < 20 ? 0 : (GameManager.Instance.SettingsData.AnimationSpeed < 40? 1 : 2); 
        }
    }

    public void SetAnimSpeed(int speed)
    {
        GameManager.Instance.SettingsData.AnimationSpeed = speed;
    }
}
