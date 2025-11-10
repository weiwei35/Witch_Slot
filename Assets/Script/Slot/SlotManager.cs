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

    private bool isSpinning = false;
    private bool endSlot = false;

    private List<SymbolSO> activeBoosters = new();
    
    List<SymbolSO> finalSymbols = new();


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
            {
                SymbolSystem.Instance.uiLookup.Clear();
                StartCoroutine(StartSpinRoutine());
            }
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

    public void ProcessVisibleSymbols(int reelIndex, List<SymbolSO> visibleSymbols)
    {
        if (visibleSymbols == null || visibleSymbols.Count == 0) return;

        foreach (var s in visibleSymbols)
        {
            finalSymbols.Add(s);
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
        isSpinning = false;
        endSlot = true;

        SymbolSystem.Instance.ApplySlotResult(finalSymbols);

        SymbolSystem.Instance.NotifyEvent(TriggerEvent.OnSlotResolved);

        GameStateManager.Instance.SetState(GameState.Walking);
    }


    private void ResetSlot()
    {
        StopAllCoroutines();
        foreach (var reel in reels)
            reel.ResetSlot();

        activeBoosters.Clear();
        stoppedReelCount = 0;

        isSpinning = false;
        endSlot = false;
    }
}
