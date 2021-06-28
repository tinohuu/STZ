using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TestUI : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler
{
    public RectTransform CardView;
    public float SizeX;
    public float SizeY;
    private void Start()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            CardView.sizeDelta = new Vector2(SizeX, SizeY);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            CardView.sizeDelta = new Vector2(SizeX, SizeY);
            CardView.GetChild(0).GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -50);
            LayoutRebuilder.ForceRebuildLayoutImmediate(CardView.transform.parent.GetComponent<RectTransform>());
        }
    }

    public void ChangeSize(int size)
    {
        CardView.sizeDelta = new Vector2(size, size);
    }
    public void OnPointerDown(PointerEventData eventData) { Debug.Log(this.gameObject + " Down"); }
    public void OnPointerEnter(PointerEventData eventData) { eventData.pointerPress = gameObject; }
    public void OnPointerUp(PointerEventData eventData) { Debug.Log(this.gameObject + " Up"); }
}
