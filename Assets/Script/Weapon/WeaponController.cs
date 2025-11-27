using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 装备系统
/// 装备基础属性：
/// 1.骰子槽位-给骰子附加攻击或护甲属性
/// 2.骰子数量（特殊骰子）
/// 装备特殊属性：待定
/// </summary>
public class WeaponController : MonoBehaviour
{
    public List<WeaponDice> diceList = new List<WeaponDice>();
}
[Serializable]
public class WeaponDice
{
    public DiceController dice;
    public DiceData diceData;
    public DiceType diceType;
    public SymbolSO symbol;
}