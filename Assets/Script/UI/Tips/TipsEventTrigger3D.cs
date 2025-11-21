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

    public void ShowTips()
    {
        description.info.Name = GetComponent<DiceController>().runtimeSymbolData.displayName;
        description.info.Description = GetComponent<DiceController>().runtimeSymbolData.description;
        TipsManager.Instance.ShowTip(description, Input.mousePosition);
    }

    private void HideTips()
    {
        TipsManager.Instance.HideTip();
    }
}