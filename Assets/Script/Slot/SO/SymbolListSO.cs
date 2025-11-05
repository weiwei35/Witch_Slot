using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Symbol List", fileName = "SymbolList")]
public class SymbolListSO : ScriptableObject
{
    // 允许列表里混合两种子类（NormalSymbolSO / BoosterSymbolSO）
    public List<BaseSymbolSO> symbols = new List<BaseSymbolSO>();
}