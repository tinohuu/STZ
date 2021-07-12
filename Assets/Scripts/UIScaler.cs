using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIScaler : MonoBehaviour
{
    RectTransform rectTransform;
    float? targetHeight = null;
    float? targetXSclae = null;
    public float Speed = 1;
    public bool IsLerp = true;
    public bool IsSwitch = false;

    public Vector2 oriLocalScale = new Vector2();
    public Vector3? targetLocalScale = null;
    public Vector3? targetSizeDelta = null;
    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }
    // Start is called before the first frame update
    void Start()
    {
        oriLocalScale = rectTransform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        if (targetHeight != null)
        {
            float oriHeight = rectTransform.sizeDelta.y;
            float curHeight = Mathf.Lerp(oriHeight, (float)targetHeight, Time.deltaTime * Speed);

            if (Mathf.Abs(oriHeight - curHeight) < 0.01f)
            {
                curHeight = (float)targetHeight;
                if (targetHeight == 0) gameObject.SetActive(false);
                targetHeight = null;
            }

            rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, curHeight);
        }

        if (targetLocalScale != null)
        {
            rectTransform.localScale = Vector3.Lerp(rectTransform.localScale, (Vector3)targetLocalScale, Time.deltaTime * Speed);

            if (Vector3.Distance(rectTransform.localScale, (Vector3)targetLocalScale) < 0.01f)
            {
                rectTransform.localScale = (Vector3)targetLocalScale;
                if (rectTransform.localScale.x == 0 || rectTransform.localScale.y == 0) gameObject.SetActive(false);
                targetLocalScale = null;
            }
        }

        if (targetSizeDelta != null)
        {
            rectTransform.sizeDelta = Vector3.Lerp(rectTransform.sizeDelta, (Vector3)targetSizeDelta, Time.deltaTime * Speed);

            if (Vector3.Distance(rectTransform.sizeDelta, (Vector3)targetSizeDelta) < 0.01f)
            {
                rectTransform.sizeDelta = (Vector3)targetSizeDelta;
                if (rectTransform.sizeDelta.x == 0 || rectTransform.sizeDelta.y == 0) gameObject.SetActive(false);
                targetSizeDelta = null;
            }
        }
    }

    public void ScaleHeight(float height)
    {
        //Debug.Log(name + " scales to " + height);
        if (gameObject.activeSelf) targetHeight = height;
        else rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, height);
    }

    public void SwitchHeight(float height)
    {
        if (Mathf.Abs(height - rectTransform.sizeDelta.y) < 0.01f)
            targetHeight = 0;
        else
        {
            gameObject.SetActive(true);
            targetHeight = height;
        }
    }

    public void ScaleXScale(float scale)
    {
        gameObject.SetActive(true);
        if (targetLocalScale == null) targetLocalScale = new Vector3(scale, rectTransform.localScale.y, rectTransform.localScale.z);
        else targetLocalScale = new Vector3(scale, ((Vector3)targetLocalScale).y, ((Vector3)targetLocalScale).z);
    }

    public void ScaleYScale(float scale)
    {
        if (targetLocalScale == null) targetLocalScale = new Vector3(rectTransform.localScale.x, scale, rectTransform.localScale.z);
        else targetLocalScale = new Vector3(((Vector3)targetLocalScale).x, scale, ((Vector3)targetLocalScale).z);
    }

    public void ScaleXSize(float height)
    {
        gameObject.SetActive(true);
        if (targetSizeDelta == null) targetSizeDelta = new Vector2(height, rectTransform.sizeDelta.y);
        else targetSizeDelta = new Vector3(height, ((Vector3)targetSizeDelta).y, ((Vector3)targetSizeDelta).z);
    }
}
