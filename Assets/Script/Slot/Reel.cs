using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Reel : MonoBehaviour
{
    [Header("0:fight 1:protect")]
    public int reelIndex;
    public RectTransform content;
    public SymbolListSO symbolListSO;
    public Symbol symbolPrefab;

    [Header("滚动设置")]
    public float maxScrollSpeed = 100f;     // 最大滚动速度
    public float accelTime = 0.5f;          // 加速时间
    public float decelTime = 1.0f;          // 减速时间
    public int visibleCount = 5;            // 可见格数
    public float itemHeight = 100f;         // 每格高度

    private float currentSpeed = 0f;        // 当前速度
    private bool isSpinning = false;        // 是否在滚动
    private bool isStopping = false;        // 是否正在减速

    private int itemCount;
    private Symbol[] items;
    private int currentFirstIndex = 0;
    private float moveDistance = 0f;
    private float totalHeight;

    public event Action<int, List<SymbolSO>> OnReelStopped;

    public void InitializeSymbols()
    {
        itemCount = symbolListSO.symbols.Count;
        totalHeight = itemHeight * itemCount;
        content.sizeDelta = new Vector2(content.sizeDelta.x, totalHeight);

        items = new Symbol[itemCount];

        for (int i = 0; i < symbolListSO.symbols.Count; i++)
        {
            Symbol slot = Instantiate(symbolPrefab, content);
            slot.Init(symbolListSO.symbols[i]);
            slot.transform.localPosition = new Vector3(0, -i * itemHeight, 0);
            items[i] = slot;
        }
        
    }

    public void ResetSlot()
    {
        StopAllCoroutines();
        isSpinning = false;
        isStopping = false;
        currentSpeed = 0f;

        foreach (var item in items)
            Destroy(item.gameObject);

        content.anchoredPosition = Vector2.zero;
        currentFirstIndex = 0;
        InitializeSymbols();
    }

    // 🟢 开始滚动动画（加速）
    public void StartSpin()
    {
        if (isSpinning) return;
        StopAllCoroutines();
        StartCoroutine(SpinRoutine());
    }

    private IEnumerator SpinRoutine()
    {
        isSpinning = true;
        isStopping = false;
        currentSpeed = 0f;

        // --- 加速阶段 ---
        float elapsed = 0f;
        while (elapsed < accelTime)
        {
            currentSpeed = Mathf.Lerp(0f, maxScrollSpeed, elapsed / accelTime);
            RollUp();
            elapsed += Time.deltaTime;
            yield return null;
        }
        currentSpeed = maxScrollSpeed;

        // --- 保持滚动（直到停止命令） ---
        while (isSpinning && !isStopping)
        {
            RollUp();
            yield return null;
        }

        // --- 减速阶段 ---
        float decelElapsed = 0f;
        float startSpeed = currentSpeed;
        while (decelElapsed < decelTime)
        {
            currentSpeed = Mathf.Lerp(startSpeed, 0f, decelElapsed / decelTime);
            RollUp();
            decelElapsed += Time.deltaTime;
            yield return null;
        }

        currentSpeed = 0f;
        isSpinning = false;
        AdjustContentPositionForCompleteView();
    }

    // 🟢 外部调用：触发停止动画
    public void StopSpin()
    {
        if (!isSpinning) return;
        isStopping = true;
    }

    private void RollUp()
    {
        float move = currentSpeed * Time.deltaTime;
        moveDistance += move;
        content.anchoredPosition += new Vector2(0, move);

        if (moveDistance >= itemHeight)
        {
            moveDistance = 0;
            RecycleItemAtFirst();
        }
    }

    private void RecycleItemAtFirst()
    {
        Symbol first = items[currentFirstIndex];
        first.transform.localPosition -= new Vector3(0, itemCount * itemHeight, 0);
        currentFirstIndex = (currentFirstIndex + 1) % itemCount;
    }

    private void AdjustContentPositionForCompleteView()
    {
        float targetPosY = Mathf.Floor(content.anchoredPosition.y / itemHeight) * itemHeight;
        StartCoroutine(SmoothMoveToTargetPosition(targetPosY));
    }
    
    private IEnumerator SmoothMoveToTargetPosition(float targetPosY)
    {
        float startPosY = content.anchoredPosition.y;
        float elapsed = 0f;
        float duration = 0.3f;
    
        while (elapsed < duration)
        {
            content.anchoredPosition = new Vector2(
                content.anchoredPosition.x,
                Mathf.Lerp(startPosY, targetPosY, elapsed / duration)
            );
            elapsed += Time.deltaTime;
            yield return null;
        }
    
        content.anchoredPosition = new Vector2(content.anchoredPosition.x, targetPosY);
        NotifyVisibleSymbols();
    }
    private void NotifyVisibleSymbols()
    {
        float contentY = content.anchoredPosition.y;
        int firstVisibleIndex = Mathf.FloorToInt(contentY / itemHeight);
        int lastVisibleIndex = Mathf.FloorToInt((contentY + visibleCount * itemHeight) / itemHeight);

        List<SymbolSO> visibleSymbols = new List<SymbolSO>();
        for (int i = firstVisibleIndex; i < lastVisibleIndex; i++)
        {
            int index = (i + itemCount) % itemCount;
            visibleSymbols.Add(items[index].symbol);
            // var inst = new SymbolInstance(items[index].symbol, index, reelIndex);
            // SymbolSystem.Instance.uiLookup.Add(inst,items[index]);
        }

        OnReelStopped?.Invoke(reelIndex, visibleSymbols);
    }
}
