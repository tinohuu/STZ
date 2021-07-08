using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingView : MonoBehaviour
{
    public enum Type { none, toggle, swap, multiple }
    public Type SettingType = Type.none;
    public Toggle Toggle = null;
    public Button SwapOn = null;
    public Button SwapOff = null;
    public List<Button> Multiple = new List<Button>();
    public List<Image> MultiImages = new List<Image>();
    bool isSetup = false;
    private void Awake()
    {
        if (isSetup) return;
        if (SettingType == Type.toggle) Toggle.gameObject.SetActive(true);
        else if (SettingType == Type.swap)
        {
            SwapOn.gameObject.SetActive(true);
            SwapOff.gameObject.SetActive(true);
        }
        else if (SettingType == Type.multiple)
        {
            foreach (Button button in Multiple) button.gameObject.SetActive(true);
            foreach (Image image in MultiImages) image.gameObject.SetActive(true);
        }
    }

    public bool ToggleData
    {
        set
        {
            Toggle.isOn = value;
            Toggle.onValueChanged.Invoke(value);
            isSetup = true;
        }
        get => Toggle.isOn;
    }

    public bool SwapData
    {
        set
        {
            //Debug.Log("ButtonData " + value);
            if (value == true) SwapOn.onClick.Invoke();
            else SwapOff.onClick.Invoke();
            isSetup = true;
        }
        get => SwapOn.gameObject.activeSelf;
    }

    public int MultipleData
    {
        set
        {
            Multiple[value].onClick.Invoke();
            isSetup = true;
        }
        get
        {
            for (int i = 0; i < MultiImages.Count; i++)
                if (MultiImages[i].gameObject.activeSelf) return i;
            Multiple[0].onClick.Invoke();
            return 0;
        }
    }

}
