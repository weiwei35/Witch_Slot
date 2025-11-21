using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotMachine : MonoBehaviour
{
    [Header("Data")]
    public List<SymbolSO> symbolLibrary;

    [Header("Reels")]
    private List<SlotReel> reels = new List<SlotReel>();
    public int currentGridCount = 5;
    public List<Transform> reelsParent;
    public SlotReel reelPrefab;

    [Header("Settings")]
    public float staggerDelay = 0.15f; // 每列启动的延迟

    private List<SymbolSO> currentPool = new List<SymbolSO>();
    private List<SymbolSO> resultSymbols = new List<SymbolSO>();
    private bool isSpinning = false;
    private Coroutine checkRoutine;
    private int stoppedReelCount = 0;
    private List<SymbolSO> resultSymbols_endSlot = new List<SymbolSO>();

    void Start()
    {
        reels.Clear();
        for (int i = 0; i < currentGridCount; i++)
        {
            var reelObj = Instantiate(reelPrefab, reelsParent[i/5]);
            reelObj.reelRow = i / 5;
            reelObj.reelColumn = i % 5;
            reelObj.Init(symbolLibrary);
            reelObj.OnReelStopped += ProcessVisibleSymbols;
            reels.Add(reelObj);
        }
    }

    private void OnDestroy()
    {
        foreach (var reel in reels)
            reel.OnReelStopped -= ProcessVisibleSymbols;
    }
    void Update()
    {
        // 按下空格启动滚动
        if (GameStateManager.Instance.Is(GameState.Slot)&& Input.GetKeyDown(KeyCode.Space))
        {
            if (!isSpinning)
            {
                SymbolSystem.Instance.ResetSymbols();
                StartRoll();
            }
        }
    }

    /// <summary>
    /// 启动所有滚轴
    /// </summary>
    public void StartRoll()
    {
        if (symbolLibrary == null || symbolLibrary.Count == 0)
        {
            Debug.LogWarning("⚠️ Symbol Library is empty!");
            return;
        }

        isSpinning = true;

        DrawResults();

        // 为所有滚轴启动滚动，并设置错峰延迟
        for (int i = 0; i < reels.Count; i++)
        {
            float delay = i * staggerDelay;
            reels[i].StartSpin(resultSymbols[i], delay);
        }

        // 检查滚动完成
        if (checkRoutine != null) StopCoroutine(checkRoutine);
        checkRoutine = StartCoroutine(CheckAllReelsFinished());
    }

    /// <summary>
    /// 随机抽取结果（不放回）
    /// </summary>
    void DrawResults()
    {
        currentPool = new List<SymbolSO>(symbolLibrary);
        resultSymbols.Clear();

        for (int i = 0; i < reels.Count; i++)
        {
            if (currentPool.Count == 0)
            {
                // 若 symbol 不够多，允许重复
                currentPool = new List<SymbolSO>(symbolLibrary);
            }

            int rand = Random.Range(0, currentPool.Count);
            resultSymbols.Add(currentPool[rand]);
            currentPool.RemoveAt(rand);
        }
    }

    /// <summary>
    /// 检测所有滚轴是否结束
    /// </summary>
    IEnumerator CheckAllReelsFinished()
    {
        bool allDone = false;
        while (!allDone)
        {
            allDone = true;
            foreach (var reel in reels)
            {
                var field = reel.GetType().GetField("isSpinning", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (field != null && (bool)field.GetValue(reel))
                {
                    allDone = false;
                    break;
                }
            }
            yield return null;
        }

        isSpinning = false;
        checkRoutine = null;
        Debug.Log("🎰 所有滚轴完成停止！");
    }
    public void ProcessVisibleSymbols(SymbolSO resultSymbol)
    {
        resultSymbols_endSlot.Add(resultSymbol);
        stoppedReelCount++;
        if (stoppedReelCount >= reels.Count)
        {
            stoppedReelCount = 0;
            OnAllReelsStopped();
        }
    }

    private void OnAllReelsStopped()
    {
        SymbolSystem.Instance.ApplySlotResult(resultSymbols_endSlot);

        SymbolSystem.Instance.NotifyEvent(TriggerEvent.OnSlotResolved);

        // GameStateManager.Instance.SetState(GameState.Walking);
    }
}
