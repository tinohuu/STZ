using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UISwitcher : MonoBehaviour
{
    public enum Direction { none, left, right, top, bottom }
    public Direction InitialLocation = Direction.none;
    float? targetLeft = null;
    public float Speed = 1;
    RectTransform rectTransform;
    public UnityEvent OnExit = null;
    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        if (InitialLocation == Direction.left) rectTransform.SetLeft(Screen.width);
        if (InitialLocation == Direction.right) rectTransform.SetRight(Screen.width);
        if (InitialLocation == Direction.top) rectTransform.SetBottom(Screen.height);
        if (InitialLocation == Direction.bottom) rectTransform.SetBottom(Screen.height);
    }

    private void Update()
    {
        if (targetLeft != null)
        {
            rectTransform.SetLeft(Mathf.Lerp(rectTransform.offsetMin.x, (float)targetLeft, Time.deltaTime * Speed));
            if (Mathf.Abs((float)targetLeft - rectTransform.offsetMin.x) < 0.01f)
            {
                rectTransform.SetLeft((float)targetLeft);
                targetLeft = null;
                if (targetLeft == 0) gameObject.SetActive(false);
            }
        }
    }
    private void OnDisable()
    {
        OnExit.Invoke();
    }
    public void RightExit()
    {
        gameObject.SetActive(true);
        if (Mathf.Abs(rectTransform.offsetMin.x - Screen.width) < 0.01f)
            targetLeft = 0;
        else
            targetLeft = Screen.width;
    }
}
