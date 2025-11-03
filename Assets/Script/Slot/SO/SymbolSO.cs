using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New Symbol", menuName = "Symbol")]
public class SymbolSO : ScriptableObject
{
    public string symbolName;
    public string symbolDesc;
    public Sprite symbolSprite;
    public float amount;
    
    public Symbol symbol;

    public void SetSymbolObj(Symbol symbolObj)
    {
        symbol = symbolObj;
    }
}