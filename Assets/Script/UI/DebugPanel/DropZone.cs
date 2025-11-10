using UnityEngine;
using UnityEngine.EventSystems;

public class DropZone : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        var obj = eventData.pointerDrag;
        if (obj == null) return;

        // 拿到 SymbolDraggable
        var drag = obj.GetComponent<DragableSymbol>();
        if (drag == null) return;

        // ✅ snap
        drag.canSet = true;
        obj.transform.SetParent(transform, false);
        obj.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
    }
}