using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class Reel : MonoBehaviour
{
    [Header("0:fight 1:protect")]
    public int reelIndex;
    public RectTransform content; // Content RectTransform
    public SymbolListSO symbloListSO;
    public Symbol symbol;
    public float scrollSpeed = 100f; // 滚动速度
    public float itemHeight = 100f; // 每个Item的高度
    private int itemCount; // Item数量
    public int visibleCount = 5; // 可见项数量

    private float totalHeight;
    private Symbol[] items; // 保存所有的Items
    private int currentFirstIndex = 0; // 当前第一个Item的索引

    private float moveDistance = 0;

    void Start()
    {
        itemCount = symbloListSO.symbols.Count;
        totalHeight = itemHeight * itemCount;

        // 设置Content的高度
        content.sizeDelta = new Vector2(content.sizeDelta.x, totalHeight);

        // 创建并初始化Item
        items = new Symbol[itemCount];
        if (reelIndex == -1)
        {
            for (int i = 0; i < itemCount; i++)
            {
                Symbol slot = Instantiate(symbol, content);
                if (symbloListSO.symbols != null)
                {
                    BoosterSymbolSO booster = (BoosterSymbolSO)symbloListSO.symbols[i];
                    foreach (var effect in booster.effects)
                    {
                        if (effect.keepTime > 0)
                        {
                            slot.keepTime = effect.keepTime;
                            if(effect.keepTime > 1)slot.isKeepBooster = true;
                        }
                    }
                    slot.Init(booster);
                }

                slot.transform.localPosition = new Vector3(0, -i * itemHeight, 0);
                items[i] = slot;
            }
        }
        else
        {
            for (int i = 0; i < itemCount; i++)
            {
                Symbol slot = Instantiate(symbol, content);
                slot.Init(symbloListSO.symbols[i]);
                slot.transform.localPosition = new Vector3(0, -i * itemHeight, 0);
                items[i] = slot;
            }
        }
    }

    // 重置 Slot 内容
    public void ResetSlot()
    {
        // 在重置之前，停止滚动
        moveDistance = 0;

        // 清除并重新创建所有的 items
        foreach (var item in items)
        {
            Destroy(item.gameObject);
        }

        // 将Content位置恢复到最初
        content.anchoredPosition = new Vector2(0, 0);
        currentFirstIndex = 0;

        // 重新初始化所有的items
        for (int i = 0; i < itemCount; i++)
        {
            Symbol slot = Instantiate(symbol, content);
            slot.Init(symbloListSO.symbols[i]);
            slot.transform.localPosition = new Vector3(0, -i * itemHeight, 0);
            items[i] = slot;
        }
    }


    void Update()
    {
        // // 检查空格键的按下状态
        // if (Input.GetKeyDown(KeyCode.Space))
        // {
        //     if (endSlot)
        //     {
        //         // 如果滚动已经结束，进行重置
        //         ResetSlot();
        //         isScrolling = true;  // 重置后开始滚动
        //         endSlot = false;
        //     }
        //     else if (!isScrolling)
        //     {
        //         // 如果没有在滚动，按空格开始滚动
        //         isScrolling = true;
        //     }
        //     else
        //     {
        //         // 如果正在滚动，按空格停止滚动并确保最后一个节点完整显示
        //         isScrolling = false;
        //         AdjustContentPositionForCompleteView();
        //         endSlot = true;  // 标记滚动结束
        //     }
        // }
        //
        // // 如果正在滚动
        // if (isScrolling)
        // {
        //     // 向上滚动
        //     moveDistance += scrollSpeed * Time.deltaTime;
        //     content.anchoredPosition += new Vector2(0, scrollSpeed * Time.deltaTime);
        //
        //     // 当Content滚动超出可见区域时，进行首尾连接
        //     if (moveDistance >= itemHeight)
        //     {
        //         moveDistance = 0;
        //         RecycleItemAtFirst();
        //     }
        // }
    }

    public void RollUp()
    {
        // 向上滚动
        moveDistance += scrollSpeed * Time.deltaTime;
        content.anchoredPosition += new Vector2(0, scrollSpeed * Time.deltaTime);

        // 当Content滚动超出可见区域时，进行首尾连接
        if (moveDistance >= itemHeight)
        {
            moveDistance = 0;
            RecycleItemAtFirst();
        }
    }
    // 将第一个Item移到最后
    void RecycleItemAtFirst()
    {
        // 获取第一个Item
        Symbol firstItem = items[currentFirstIndex];

        // 将该Item移到队伍尾部的正确位置
        firstItem.transform.localPosition -= new Vector3(0, itemCount * itemHeight, 0);

        // 更新当前第一个Item的索引
        currentFirstIndex = (currentFirstIndex + 1) % itemCount;
    }

    // 调整Content位置，确保最后一个节点完整显示
    public void AdjustContentPositionForCompleteView()
    {
        // 根据当前的位置计算应调整的内容位置，使其显示完整节点
        float targetPosY = Mathf.Floor(content.anchoredPosition.y / itemHeight) * itemHeight;

        // 平滑过渡到目标位置
        StartCoroutine(SmoothMoveToTargetPosition(targetPosY));
    }

    // 使用协程平滑过渡到目标位置
    IEnumerator SmoothMoveToTargetPosition(float targetPosY)
    {
        float startPosY = content.anchoredPosition.y;
        float elapsedTime = 0f;
        float duration = 0.5f; // 过渡时间

        while (elapsedTime < duration)
        {
            content.anchoredPosition = new Vector2(content.anchoredPosition.x, Mathf.Lerp(startPosY, targetPosY, elapsedTime / duration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 确保最后准确对齐
        content.anchoredPosition = new Vector2(content.anchoredPosition.x, targetPosY);
        StartCoroutine(OutputVisibleItems());
    }
    // 计算并输出视野范围内的项
    IEnumerator OutputVisibleItems()
    {
        yield return new WaitForSeconds(0.5f);
        // 计算可见区域的开始和结束索引
        float contentPositionY = content.anchoredPosition.y;
        int firstVisibleIndex = Mathf.FloorToInt(contentPositionY / itemHeight);
        int lastVisibleIndex = Mathf.FloorToInt((contentPositionY + (visibleCount * itemHeight)) / itemHeight);

        // 输出当前可见范围内的项
        // Debug.Log("Visible Items:");
        for (int i = firstVisibleIndex; i < lastVisibleIndex; i++)
        {
            int index = (i + itemCount) % itemCount; // 处理循环
            float amount = items[index].symbol.amount;
            GetComponentInParent<SlotManager>().SetSlotResult(reelIndex, amount);
            if (reelIndex == -1)
            {
                BoosterSymbolSO booster = items[index].symbol as BoosterSymbolSO;
                if (booster != null) GetComponentInParent<SlotManager>().SetBoosterResult(booster);
            }
        }
    }
}
