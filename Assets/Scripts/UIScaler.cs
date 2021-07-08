using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIScaler : MonoBehaviour
{
    RectTransform rectTransform;
    float? targetHeight = null;
    public float Speed = 1;
    public bool IsLerp = true;
    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (targetHeight != null)
        {
            float oriHeight = rectTransform.sizeDelta.y;
            float curHeight = Mathf.Lerp(oriHeight, (float)targetHeight, Time.deltaTime * Speed);

            if (Mathf.Abs(oriHeight - curHeight) < 0.1f)
            {
                curHeight = (float)targetHeight;
                targetHeight = null;
            }

            rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, curHeight);
        }
    }

    public void ScaleHeight(float height)
    {
        //Debug.Log(name + " scales to " + height);
        if (gameObject.activeSelf) targetHeight = height;
        else rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, height);
    }
}
