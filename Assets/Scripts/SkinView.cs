using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkinView : MonoBehaviour
{
    public Text Durability;
    public GameObject New;
    public Button UseButton;
    public GameObject UseText;
    public GameObject InUseText;

    protected SkinManager skinManager;
    private void Start()
    {
        skinManager = FindObjectOfType<SkinManager>();
        UpdateView();
    }
    private void OnEnable()
    {
        skinManager = FindObjectOfType<SkinManager>();
        UpdateView();
    }
    public virtual void Use()
    {

    }
    public virtual void UpdateView()
    {

    }
}
