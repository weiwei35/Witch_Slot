using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugPanel : MonoBehaviour
{
    public List<SymbolSO> symbols = new List<SymbolSO>();
    public DragableSymbol symbolPrefab;
    public SymbolSO emptySymbol;
    public GameObject SymbolListParent;
    public GameObject ReelList_1;
    public GameObject ReelList_2;
    public GameObject ReelList_3;
    public GameObject ReelList_4;
    public GameObject ReelList_5;
    public SymbolListSO reel_1;
    public SymbolListSO reel_2;
    public SymbolListSO reel_3;
    public SymbolListSO reel_4;
    public SymbolListSO reel_5;

    private void OnEnable()
    {
        SetSymbolList();
    }

    public void SetSymbolList()
    {
        foreach (var symbol in symbols)
        {
            var obj = Instantiate(symbolPrefab, SymbolListParent.transform);
            obj.Init(symbol);
        }
    }

    public void SubmitReelList()
    {
        reel_1.symbols.Clear();
        var reelSymbols_1 = ReelList_1.transform.GetChild(1);
        foreach (Transform obj in reelSymbols_1)
        {
            if (obj.childCount > 0)
            {
                var symbolObj = obj.GetChild(0).GetComponent<DragableSymbol>();
                if(symbolObj != null) reel_1.symbols.Add(symbolObj.symbol);
            }
            else reel_1.symbols.Add(emptySymbol);
        }
        reel_2.symbols.Clear();
        var reelSymbols_2 = ReelList_2.transform.GetChild(1);
        foreach (Transform obj in reelSymbols_2)
        {
            if (obj.childCount > 0)
            {
                var symbolObj = obj.GetChild(0).GetComponent<DragableSymbol>();
                if(symbolObj != null) reel_2.symbols.Add(symbolObj.symbol);
            }
            else reel_2.symbols.Add(emptySymbol);
        }
        reel_3.symbols.Clear();
        var reelSymbols_3 = ReelList_3.transform.GetChild(1);
        foreach (Transform obj in reelSymbols_3)
        {
            if (obj.childCount > 0)
            {
                var symbolObj = obj.GetChild(0).GetComponent<DragableSymbol>();
                if(symbolObj != null) reel_3.symbols.Add(symbolObj.symbol);
            }
            else reel_3.symbols.Add(emptySymbol);
        }
        reel_4.symbols.Clear();
        var reelSymbols_4 = ReelList_4.transform.GetChild(1);
        foreach (Transform obj in reelSymbols_4)
        {
            if (obj.childCount > 0)
            {
                var symbolObj = obj.GetChild(0).GetComponent<DragableSymbol>();
                if(symbolObj != null) reel_4.symbols.Add(symbolObj.symbol);
            }
            else reel_4.symbols.Add(emptySymbol);
        }
        reel_5.symbols.Clear();
        var reelSymbols_5 = ReelList_5.transform.GetChild(1);
        foreach (Transform obj in reelSymbols_5)
        {
            if (obj.childCount > 0)
            {
                var symbolObj = obj.GetChild(0).GetComponent<DragableSymbol>();
                if(symbolObj != null) reel_5.symbols.Add(symbolObj.symbol);
            }
            else reel_5.symbols.Add(emptySymbol);
        }
    }
}
