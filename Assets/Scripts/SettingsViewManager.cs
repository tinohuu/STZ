using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsViewManager : MonoBehaviour
{
    public Setting Expand = null;
    public Setting DrawCards = null;
    public Setting RightHand = null;
    public Setting Info = null;
    public Setting AnimSpeed = null;
    public Setting Volume = null;
    public static SettingsViewManager Instance;
    private void Awake()
    {
        Instance = this;
        if (GameManager.Instance.SettingsData != null)
        {
            Expand.ButtonData = GameManager.Instance.SettingsData.IsEoExpanded;
            DrawCards.ToggleData = GameManager.Instance.SettingsData.DrawCards == 3;
            RightHand.ToggleData = GameManager.Instance.SettingsData.IsRightHand;
            Info.ToggleData = GameManager.Instance.SettingsData.Info;
            //Volume.ButtonData = GameManager.Instance.SettingsData.Volume;
            AnimSpeed.BarData = Mathf.Clamp((int)GameManager.Instance.SettingsData.AnimationSpeed / 10, 0, 2);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        //Expand.ButtonData = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
