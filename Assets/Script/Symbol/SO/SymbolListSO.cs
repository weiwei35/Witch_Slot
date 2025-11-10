using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/SymbolList")]
public class SymbolListSO : ScriptableObject
{
    public List<SymbolSO> symbols = new List<SymbolSO>();
}
