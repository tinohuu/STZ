using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
public class Setting : MonoBehaviour
{
    public enum Type { none, toggle, swap, bar }
    public Type SettingType = Type.none;
    public Toggle Toggle = null;
    public Button ButtonOn = null;
    public Button ButtonOff = null;
    public List<Button> Bar = new List<Button>();
    public List<Image> BarImages = new List<Image>();
    //public List<Image> BarImages = new List<Image>();
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnEnable()
    {
        //if (SettingType == Type.swap)
        //{
         //   if (ButtonOn.gameObject.activeSelf) ButtonOff.onClick.Invoke();
         //   else ButtonOn.onClick.Invoke();
        //}
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool ToggleData
    {
        set
        {
            Toggle.isOn = value;
            Toggle.onValueChanged.Invoke(value);
        }
        get
        {
            return Toggle.isOn;
        }
    }

    public bool ButtonData
    {
        set
        {
            Debug.Log("ButtonData " + value);
            if (value == true)
            {
                ButtonOn.onClick.Invoke();
            }
            else
            {
                ButtonOff.onClick.Invoke();
            }
        }
        get
        {
            return ButtonOn.gameObject.activeSelf;
        }
    }

    public int BarData
    {
        set
        {
            Bar[value].onClick.Invoke();
        }
        get
        {
            for (int i = 0; i < BarImages.Count; i++)
            {
                if (BarImages[i].gameObject.activeSelf) return i;
            }

            // Return first if not activated
            Bar[0].onClick.Invoke();
            return 0;
        }
    }

}
