using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
public class Setting : MonoBehaviour
{
    public Button ButtonOn = null;
    public Button ButtonOff = null;
    public List<Button> Bar = new List<Button>();
    public List<Image> BarImages = new List<Image>();
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool Data
    {
        set
        {
            if (value) ButtonOff.onClick.Invoke();
            else ButtonOn.onClick.Invoke();
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
