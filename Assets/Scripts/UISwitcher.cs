using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UISwitcher : MonoBehaviour
{
    public enum Direction { none, left, right, top, bottom }
    public Direction InitialLocation = Direction.none;
    //float? targetLeft = null;
    public Vector2? TargetPos = null;
    public float Speed = 1;
    RectTransform rectTransform;
    public UnityEvent OnExit = null;
    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(Screen.width, Screen.height);
        rectTransform.anchoredPosition = new Vector2(Screen.width, 0);
        //if (InitialLocation == Direction.left) rectTransform.SetLeft(Screen.width);
        //if (InitialLocation == Direction.right) rectTransform.SetRight(Screen.width);
        //if (InitialLocation == Direction.top) rectTransform.SetBottom(Screen.height);
        //if (InitialLocation == Direction.bottom) rectTransform.SetBottom(Screen.height);
    }

    private void Update()
    {
        rectTransform.sizeDelta = new Vector2(Screen.width, Screen.height);
        if (TargetPos != null)
        {
            rectTransform.anchoredPosition = Vector2.Lerp(rectTransform.anchoredPosition, (Vector2)TargetPos, Time.deltaTime * Speed);
            if ((rectTransform.anchoredPosition - (Vector2)TargetPos).magnitude < 0.1f)
            {
                rectTransform.anchoredPosition = (Vector2)TargetPos;
                TargetPos = null;
                if (rectTransform.anchoredPosition.magnitude > 0.1f) OnExit?.Invoke();
            }
        }
        /*if (targetLeft != null)
        {
            rectTransform.SetLeft(Mathf.Lerp(rectTransform.offsetMin.x, (float)targetLeft, Time.deltaTime * Speed));
            if (Mathf.Abs((float)targetLeft - rectTransform.offsetMin.x) < 0.01f)
            {
                rectTransform.SetLeft((float)targetLeft);
                targetLeft = null;
                if (targetLeft == 0) gameObject.SetActive(false);
            }
        }*/
    }
    public void RightExit()
    {
        if (rectTransform.anchoredPosition.magnitude > 0.1f)
            TargetPos = Vector2.zero; 
        else
            TargetPos = new Vector2(Screen.width, 0);
    }
}
