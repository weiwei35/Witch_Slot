using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Symbol : MonoBehaviour
{
    [Header("数据")]
    public SymbolSO symbol;   // 由 Reel.Init 注入

    [Header("可选可视化")]
    public Image icon;

    private Animator _anim;
    bool animFinished = false;
    public bool IsAnimationFinished => animFinished;

    private void Awake()
    {
        _anim = GetComponent<Animator>();
    }

    public void Init(SymbolSO data)
    {
        symbol = data;

        UIDescription tip = GetComponent<UIDescription>();
        if (icon) icon.sprite = data.icon;
        tip.AddTip(data.displayName,
            data.description);
    }

    public void SetActiveAnimation(bool active)
    {
        animFinished = false;
        _anim.SetTrigger("active");

        // 在 Animator event 调用：AnimationFinished()
    }

    // Called by AnimationEvent
    public void AnimationFinished()
    {
        animFinished = true;
    }
}