using System;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Collections;
using Random = UnityEngine.Random;

public class SlotReel : MonoBehaviour
{
    [Header("References")]
    public RectTransform symbolHolder; // 单个 symbol 容器
    public GameObject symbolItemPrefab;
    public int reelRow;
    public int reelColumn;

    [Header("Settings")]
    public float spinDuration = 0.15f; // 每次翻页动画时间
    public float spinInterval = 0.2f; // 连续翻页间隔
    public float totalSpinTime; // 滚动总时长范围

    private Symbol currentSymbol;
    private List<SymbolSO> symbolsPool = new List<SymbolSO>();
    private SymbolSO resultSymbol;
    private bool isSpinning = false;
    private Tween spinTween;
    
    public event Action<SymbolSO> OnReelStopped;

    public void Init(List<SymbolSO> library)
    {
        symbolsPool = new List<SymbolSO>(library);

        // 初始化第一个符号
        if (currentSymbol == null)
        {
            var go = Instantiate(symbolItemPrefab, symbolHolder);
            currentSymbol = go.GetComponent<Symbol>();
            currentSymbol.Init(symbolsPool[Random.Range(0, symbolsPool.Count)]);
        }
    }

    public void StartSpin(SymbolSO target, float startDelay = 0f)
    {
        if (isSpinning) return;
        resultSymbol = target;
        StopAllCoroutines();
    
        // 强制初始化容器位置，防止上一次残留位置导致第一次缓慢移动
        if (symbolHolder != null)
            symbolHolder.anchoredPosition = Vector2.zero;
    
        // 保证间隔至少等于动画时长的 1.05 倍，避免动画被过快覆盖
        if (spinInterval < spinDuration * 1.05f)
            spinInterval = spinDuration * 1.05f;
    
        StartCoroutine(StartWithDelay(startDelay));
    }
    
    private IEnumerator StartWithDelay(float delay)
    {
        if (delay > 0f) yield return new WaitForSeconds(delay);
        yield return StartCoroutine(SpinRoutine());
    }
    
    private IEnumerator SpinRoutine()
    {
        isSpinning = true;

        float elapsed = 0f;
        float duration = totalSpinTime;

        // 基础参数
        float baseInterval = spinInterval;
        float minInterval = baseInterval * 0.5f; // 加速阶段最短间隔
        float maxInterval = baseInterval * 2.0f; // 减速阶段最长间隔

        // 我们使用 DOTween 的缓动函数来模拟“速度变化曲线”
        // t=0（开始） => 加速， t=1（结束） => 减速
        Ease easing = Ease.OutQuad;

        while (elapsed < duration)
        {
            float t = Mathf.Clamp01(elapsed / duration);

            // Ease.OutQuad 曲线映射：快 → 慢
            // EvaluateEase() 是 DOTween 的内部工具函数（我们用 DOGetter 模拟）
            float eased = DOVirtual.EasedValue(0, 1, t, easing);

            // 反向映射到间隔（开始快 → 结束慢）
            float currentInterval = Mathf.Lerp(minInterval, maxInterval, eased);

            PlayOneSpin(symbolsPool[Random.Range(0, symbolsPool.Count)]);
            yield return new WaitForSeconds(currentInterval);

            elapsed += currentInterval;
        }

        // 最终结果动画
        PlayOneSpin(resultSymbol);
        yield return new WaitForSeconds(spinInterval);

        isSpinning = false;
        
        var inst = new SymbolInstance(resultSymbol, reelRow, reelColumn);
        SymbolSystem.Instance.uiLookup.Add(inst,currentSymbol);
        OnReelStopped?.Invoke(resultSymbol);
    }
    
    private void PlayOneSpin(SymbolSO nextSymbol)
    {
        // 终止上一次序列
        if (spinTween != null && spinTween.IsActive()) spinTween.Kill();
    
        // 使用 Sequence 明确出场 -> 更新内容 -> 入场 的子步骤，避免中间空白
        var seq = DOTween.Sequence();
    
        // 确定单侧时长（出/入各半）
        float half = spinDuration * 0.5f;
    
        // 出场：把容器向下移动到 -100 （相对目标值），使用 From = current -> to = -100
        seq.Append(symbolHolder.DOAnchorPosY(-100f, half).SetEase(Ease.InQuad));
    
        // 更新内容在中点（通过 OnComplete 的方式保证时序），然后立刻把容器跳到 +100 并做入场动画
        seq.AppendCallback(() =>
        {
            // 更新 symbol 内容
            if (currentSymbol != null && nextSymbol != null)
                currentSymbol.Init(nextSymbol);
    
            // 直接把容器搬到上方准备入场（不使用即时视觉跳帧）
            symbolHolder.anchoredPosition = new Vector2(symbolHolder.anchoredPosition.x, 100f);
        });
    
        // 入场：从 +100 -> 0
        seq.Append(symbolHolder.DOAnchorPosY(0f, half).SetEase(Ease.OutQuad));
    
        // 记录引用（以便下次能 Kill）
        spinTween = seq;
        seq.Play();
    }
}
