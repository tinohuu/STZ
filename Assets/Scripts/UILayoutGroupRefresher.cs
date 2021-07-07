using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILayoutGroupRefresher : MonoBehaviour
{
    private void OnEnable()
    {
        StartCoroutine(Refresh());
    }

    IEnumerator Refresh()
    {
        yield return 0;
        LayoutRebuilder.ForceRebuildLayoutImmediate(transform.GetComponent<RectTransform>());
        LayoutRebuilder.ForceRebuildLayoutImmediate(transform.parent.GetComponent<RectTransform>());
    }
}
