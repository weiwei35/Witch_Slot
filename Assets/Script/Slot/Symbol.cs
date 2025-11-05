using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Symbol : MonoBehaviour
{
    [Header("数据")]
    public BaseSymbolSO symbol;   // 由 Reel.Init 注入

    [Header("可选可视化")]
    public Image icon;

    private Animator _anim;

    private void Awake()
    {
        _anim = GetComponent<Animator>();
    }

    public void Init(BaseSymbolSO data)
    {
        symbol = data;

        UIDescription tip = GetComponent<UIDescription>();
        if (icon)  icon.sprite = data.symbolSprite;
        tip.info.Name  = data.symbolName;
        tip.info.Description   = data.symbolDesc;
    }

    public void ActiveAnim(string triggerName = "Active")
    {
        if (_anim) _anim.SetTrigger(triggerName);
    }
}