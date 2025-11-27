using System;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 挂载到目标 UI 上，启用 Tips
/// </summary>
[RequireComponent(typeof(UIDescription))]
public class TipsEventTrigger3D : MonoBehaviour
{
    private UIDescription description;

    private void Awake()
    {
        description = GetComponent<UIDescription>();
    }

    public void SetTips(SymbolSO symbol)
    {
        description.AddTip(symbol.displayName,
            symbol.description);
    }

    public void ChangeTips(SymbolSO symbol)
    {
        description.RemoveTip(symbol.displayName);
        SetTips(symbol);
    }
    public void ShowTips()
    {
        TipsManager.Instance.ShowTip(description, Input.mousePosition);
    }
}