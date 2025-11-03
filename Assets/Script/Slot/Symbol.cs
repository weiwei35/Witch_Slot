using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Symbol : MonoBehaviour
{
    public SymbolSO symbol;
    public Image image;
    public TMP_Text keepTimeText;

    [HideInInspector]
    public float keepTime;
    public bool isKeepBooster = false;

    private Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void Init(SymbolSO so)
    {
        symbol = so;
        image.sprite = so.symbolSprite;
        if (so.symbolName != "") GetComponent<UIDescription>().info.Name = so.symbolName;
        if (so.symbolDesc != "") GetComponent<UIDescription>().info.Description = so.symbolDesc;

        symbol.symbol = this;
    }

    private void Update()
    {
        if(keepTime > 0)
        {
            keepTimeText.gameObject.SetActive(true);
            keepTimeText.text = keepTime.ToString();
        }
        else
        {
            keepTimeText.gameObject.SetActive(false);
        }
    }

    public void AfterFight()
    {
        if(keepTime > 0 && isKeepBooster)
            keepTime--;
        if (keepTime <= 0 && isKeepBooster)
        {
            // Destroy(gameObject);
            GrayAnim();
        }
    }
    public void ActiveAnim()
    {
        anim.SetTrigger("active");
    }
    public void GrayAnim()
    {
        anim.SetTrigger("gray");
    }
}
