using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UIAnchorsAnimator : MonoBehaviour
{
    public Vector2? targetAnchorMin = null;
    public Vector2? targetAnchorMax = null;
    RectTransform rectTransform;
    public UnityEvent OnExit = null;
    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    private void Update()
    {
        //rectTransform.sizeDelta = new Vector2(Screen.width, Screen.height);

        if (targetAnchorMin != null && targetAnchorMax != null)
        {
            rectTransform.anchorMin = Vector2.Lerp(rectTransform.anchorMin, (Vector2)targetAnchorMin, Time.deltaTime * GameManager.Instance.SettingsData.AnimationSpeed / 2);
            rectTransform.anchorMax = Vector2.Lerp(rectTransform.anchorMax, (Vector2)targetAnchorMax, Time.deltaTime * GameManager.Instance.SettingsData.AnimationSpeed / 2);
            if ((rectTransform.anchorMin - (Vector2)targetAnchorMin).magnitude < 0.01f)
            {
                rectTransform.anchorMin = (Vector2)targetAnchorMin;
                rectTransform.anchorMax = (Vector2)targetAnchorMax;
                targetAnchorMin = null;
                targetAnchorMax = null;
                if ((rectTransform.anchorMin - new Vector2(1, 0)).magnitude + (rectTransform.anchorMax - new Vector2(2, 1)).magnitude < 0.01f) OnExit?.Invoke();
            }
        }
    }
    public void RightExit()
    {
        targetAnchorMin = new Vector2(1, 0);
        targetAnchorMax = new Vector2(2, 1);
    }

    public void Back()
    {
        targetAnchorMin = new Vector2(0, 0);
        targetAnchorMax = new Vector2(1, 1);
    }
}
