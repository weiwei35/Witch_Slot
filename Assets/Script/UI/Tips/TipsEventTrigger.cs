using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 挂载到目标 UI 上，启用 Tips
/// </summary>
[RequireComponent(typeof(UIDescription))]
public class TipsEventTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private UIDescription description;

    private void Awake()
    {
        description = GetComponent<UIDescription>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        TipsManager.Instance.ShowTip(description, transform.position);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        TipsManager.Instance.HideTip();
    }
}