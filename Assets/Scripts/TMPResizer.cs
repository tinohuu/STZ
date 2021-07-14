using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TMPResizer : MonoBehaviour
{
    TMP_Text tmp = null;
    float oriFontSize;
    float oriFontSizeMin;
    float oriFontSizeMax;
    public float Ratio = 1;
    private void Awake()
    {
        tmp = GetComponent<TMP_Text>();
        oriFontSize = tmp.fontSize;
        oriFontSizeMin = tmp.fontSizeMin;
        oriFontSizeMax = tmp.fontSizeMax;
        Debug.LogWarning(Screen.width + "/" + Screen.height);
        Ratio = ((float)Screen.width/Screen.height) / (750F /1334f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable()
    {

        tmp.fontSize = oriFontSize * Ratio;
        tmp.fontSizeMin = oriFontSizeMin * Ratio;
        tmp.fontSizeMax = oriFontSizeMax * Ratio;
    }
}
