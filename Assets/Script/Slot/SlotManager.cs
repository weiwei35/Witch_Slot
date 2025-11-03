using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SlotManager : MonoBehaviour
{
    //按下空格开始滚动，再次按下空格依次停下
    public List<Reel> reels = new List<Reel>();
    private float fightAmount;
    private float protectAmount;

    public ObjectEventSO SetFightAmountEvent;
    public ObjectEventSO SetProtectAmountEvent;
    
    private bool isScrolling = false; // 控制滚动是否开始
    private bool endSlot = false; // 控制是否结束滚动并显示完整节点
    
    private List<BoosterSymbolSO> boosters = new List<BoosterSymbolSO>();
    private void Update()
    {
        if(GameManager.instance.gameState != GameState.Slot) return;
        // 检查空格键的按下状态
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (endSlot)
            {
                // 如果滚动已经结束，进行重置
                foreach (var reel in reels)
                {
                    isScrolling = false;
                    boosters.Clear();
                    reel.ResetSlot();
                }
                isScrolling = true; // 重置后开始滚动
                endSlot = false;
            }
            else if (!isScrolling)
            {
                // 如果没有在滚动，按空格开始滚动
                isScrolling = true;
            }
            else
            {
                // 如果正在滚动，按空格停止滚动并确保最后一个节点完整显示
                StartCoroutine(StopReelOneByOne());
                isScrolling = false;
                endSlot = true; // 标记滚动结束
            }
        }
        // 如果正在滚动
        if (isScrolling)
        {
            // 向上滚动
            foreach (var reel in reels)
            {
                reel.RollUp();
            }
        }
    }

    IEnumerator StopReelOneByOne()
    {
        foreach (var reel in reels)
        {
            yield return new WaitForSeconds(0.2f);
            reel.AdjustContentPositionForCompleteView();
        }
    }

    public void SetSlotResult(int index,float amont)
    {
        if (index == 0)
        {
            fightAmount = amont;
            SetFightAmountEvent.RaiseEvent(fightAmount,null);
        }else if (index == 1)
        {
            protectAmount = amont;
            SetProtectAmountEvent.RaiseEvent(protectAmount,null);
            
            GameManager.instance.gameState = GameState.Walking;
        }
    }

    public void SetBoosterResult(BoosterSymbolSO symbol)
    {
        boosters.Add(symbol);
        foreach (var effect in symbol.effects)
        {
            if (effect.boosterEffectType == BoosterEffectType.Now)
            {
                effect.BoosterAttack();
            }
            else if (effect.boosterEffectType == BoosterEffectType.InFight)
            {
                effect.BoosterInFight();
            }
        }
    }

    public void WinFight()
    {
        foreach (var booster in boosters)
        {
            foreach (var effect in booster.effects)
            {
                if (effect.boosterEffectType == BoosterEffectType.AfterWin)
                {
                    effect.BoosterAfterWin();
                }
            }
        }
    }
}
