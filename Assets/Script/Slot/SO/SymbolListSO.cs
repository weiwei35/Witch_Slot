using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "list", menuName = "SymbolList")]
public class SymbolListSO : ScriptableObject
{
    [SerializeField]
    public List<SymbolSO> symbols = new List<SymbolSO>();
}