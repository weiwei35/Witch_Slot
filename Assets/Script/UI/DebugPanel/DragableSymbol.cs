using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragableSymbol : MonoBehaviour,IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("数据")]
    public SymbolSO symbol;   // 由 Reel.Init 注入
    public Image icon;
    
    public void Init(SymbolSO data)
    {
        symbol = data;

        UIDescription tip = GetComponent<UIDescription>();
        if (icon)  icon.sprite = data.icon;
        tip.AddTip(data.displayName,
            data.description);
    }
    
    public Canvas canvas;
    private CanvasGroup group;
    private RectTransform rt;
    private Transform originalParent;

    public bool canSet = false;
    void Awake()
    {
        rt = GetComponent<RectTransform>();
        group = GetComponent<CanvasGroup>();
        if (!canvas)
            canvas = FindObjectOfType<Canvas>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalParent = transform.parent;
        transform.SetParent(canvas.transform, true);
        group.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        rt.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if(!canSet)
            // 如果没有 drop 到 valid zone → 回原位
            transform.SetParent(originalParent, true);
        
        group.blocksRaycasts = true;
    }
}
