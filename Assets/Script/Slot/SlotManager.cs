using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotManager : MonoBehaviour
{
    [Header("转盘配置")]
    public List<Reel> reels = new();

    [Header("滚动参数")]
    public float totalSpinTime = 3.0f;
    public float stopDelayBetweenReels = 0.3f;

    [Header("事件引用")]
    public FloatEventSO SetFightAmountEvent;
    public FloatEventSO SetProtectAmountEvent;

    private float fightAmount;
    private float protectAmount;

    private bool isSpinning = false;
    private bool endSlot = false;

    private List<BoosterSymbolSO> activeBoosters = new();

    private int stoppedReelCount = 0;

    private void Start()
    {
        foreach (var reel in reels)
            reel.OnReelStopped += ProcessVisibleSymbols;
    }

    private void OnDestroy()
    {
        foreach (var reel in reels)
            reel.OnReelStopped -= ProcessVisibleSymbols;
    }

    private void Update()
    {
        if (!GameStateManager.Instance.Is(GameState.Slot)) return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!isSpinning)
                StartCoroutine(StartSpinRoutine());
            else if (endSlot)
                ResetSlot();
        }
    }

    private IEnumerator StartSpinRoutine()
    {
        if (isSpinning) yield break;

        isSpinning = true;
        endSlot = false;
        stoppedReelCount = 0;
        activeBoosters.Clear();

        foreach (var reel in reels)
            reel.StartSpin();

        for (int i = 0; i < reels.Count; i++)
        {
            yield return new WaitForSeconds(totalSpinTime + i * stopDelayBetweenReels);
            reels[i].StopSpin();
        }
    }

    public void ProcessVisibleSymbols(int reelIndex, List<BaseSymbolSO> visibleSymbols)
    {
        if (visibleSymbols == null || visibleSymbols.Count == 0) return;

        foreach (var symbol in visibleSymbols)
        {
            if (symbol is BoosterSymbolSO booster)
            {
                activeBoosters.Add(booster);
            }
            else if (symbol is NormalSymbolSO normal)
            {
                if (reelIndex == 0)
                {
                    fightAmount = normal.amount;
                    SetFightAmountEvent.RaiseEvent(fightAmount, null);
                }
                else if (reelIndex == 1)
                {
                    protectAmount = normal.amount;
                    SetProtectAmountEvent.RaiseEvent(protectAmount, null);
                }
            }
        }

        stoppedReelCount++;
        if (stoppedReelCount >= reels.Count)
        {
            stoppedReelCount = 0;
            OnAllReelsStopped();
        }
    }

    private void OnAllReelsStopped()
    {
        // ✅ 1️⃣ 注册所有 Booster（仅一次）
        foreach (var booster in activeBoosters)
        {
            // ✅ 只注册持续型（急速类 Immediate 已即时触发）
            if (booster.durationType != BoosterDurationType.Immediate)
            {
                var effect = new BoosterEffect(booster);

                switch (booster.targetType)
                {
                    case BoosterTargetType.Player:
                    case BoosterTargetType.CurrentEnemy:
                        GameManager.Instance.globalEffectManager.AddEffect(effect, GameManager.Instance.player.GetData());
                        break;

                    case BoosterTargetType.AllEnemies:
                        foreach (var e in GameManager.Instance.ActiveEnemies)
                            GameManager.Instance.globalEffectManager.AddEffect(effect, e.GetData());
                        break;
                }
            }
            // ✅ 如果是征服类（OnVictory），延迟触发，不在此执行
            else
            {
                if (booster.triggerTiming == BoosterTriggerTiming.OnVictory)
                {
                    Debug.Log($"注册征服类 Booster（延迟生效）：{booster.symbolName}");
                }
                else
                {
                    Debug.Log($"注册持续 Booster：{booster.symbolName}");
                }
            }
        }

        // ✅ 2️⃣ 立即触发“急速类”效果（即时执行）
        BoosterTriggerSystem.TriggerBoosters(
            BoosterTriggerTiming.OnSpinEnd,
            activeBoosters,
            GameManager.Instance.player.GetData(),
            GameManager.Instance.ActiveEnemies
        );

        // ✅ 3️⃣ 添加到全局 ActiveBoosters 记录中（防重名）
        foreach (var booster in activeBoosters)
        {
            if (!GameManager.Instance.ActiveBoosters.Contains(booster))
                GameManager.Instance.ActiveBoosters.Add(booster);
        }

        // 状态恢复
        isSpinning = false;
        endSlot = true;
        GameStateManager.Instance.SetState(GameState.Walking);
    }

    private void ResetSlot()
    {
        StopAllCoroutines();
        foreach (var reel in reels)
            reel.ResetSlot();

        fightAmount = 0;
        protectAmount = 0;
        activeBoosters.Clear();
        stoppedReelCount = 0;

        isSpinning = false;
        endSlot = false;
    }
}
