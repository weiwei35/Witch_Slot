using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// 用于前置设置reel中symbol list的洗牌
/// </summary>
public class ReelManager : MonoBehaviour
{
    [Header("转盘配置")]
    public List<Reel> reels = new();
    public List<SymbolListSO> symbolList_reel = new();
    public List<SymbolPos> symbolList = new();

    private void OnEnable()
    {
        Init();
    }

    public void Init()
    {
        symbolList_reel.Clear();
        foreach (var reel in reels)
        {
            symbolList_reel.Add(reel.symbolListSO);
        }

        for (int i = 0; i < symbolList_reel.Count; i++)
        {
            for (int j = 0; j < symbolList_reel[i].symbols.Count; j++)
            {
                symbolList.Add(new SymbolPos(i,j,symbolList_reel[i].symbols[j]));
            }
        }

        ShuffleSymbolList(symbolList, 5);

        foreach (var symbol in symbolList)
        {
            for (int i = 0; i < symbolList_reel.Count; i++)
            {
                if (symbol.x == i)
                {
                    symbolList_reel[i].symbols.Add(symbol.data);
                }
            }
        }
        ShuffleSymbolList(symbolList, 5);

        foreach (var symbol in symbolList)
        {
            for (int i = 0; i < symbolList_reel.Count; i++)
            {
                if (symbol.x == i)
                {
                    symbolList_reel[i].symbols.Add(symbol.data);
                }
            }
        }

        foreach (var reel in reels)
        {
            reel.InitializeSymbols();
        }
    }
    public void ShuffleSymbolList(List<SymbolPos> list, int width)
    {
        // 1. 使用 Knuth 洗牌算法打乱顺序
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1); // Unity.Random.Range 包含上界
            (list[i], list[j]) = (list[j], list[i]);
        }
        // 2. 重新设置每个 SymbolPos 的坐标
        for (int index = 0; index < list.Count; index++)
        {
            // 获取原始数据
            var current = list[index];
            SymbolSO originalData = current.data;
            // 重新计算坐标
            int newX = index % width;           // X = 列索引
            int newY = index / width;           // Y = 行索引
            // 重新赋值（假设 SymbolPos 为 struct 或具有构造函数）
            list[index] = new SymbolPos(newX, newY, originalData);
        }
    }
}
public struct SymbolPos
{
    public int x;
    public int y;
    public SymbolSO data;

    public SymbolPos(int i, int j, SymbolSO symbol)
    {
        x = i;
        y = j;
        data = symbol;
    }
}