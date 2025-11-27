using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
public enum GameState
{
    System,
    LoadRoom,
    Slot,
    Walking,
    Fighting,
}
public enum SymbolCategory
{
    Normal,     // 固定基础属性
    Instant,    // 一次性加成，不需要生成buff
    Booster     // 拥有 trigger + effects
}

// 触发事件点
public enum TriggerEvent
{
    OnSlotResolved,     // Slot 完成后触发（立即生效）
    OnCombatStart,      // 战斗开始
    OnBeforeAttack,     // 玩家攻击前
    OnAfterAttack,      // 玩家攻击后
    OnReceiveDamage,    // 玩家受击
    OnVictory,          // 战斗胜利
    OnCombatEnd         // 战斗结束
}

// 效果类型
public enum EffectType
{
    ModifyAttack,       // 修改攻击
    ModifyDefense,      // 修改防御
    ModifyHP,           // 修改血量
    ElementDamage,      // 元素伤害
    TemporaryAttack,     // 临时攻击
    TemporaryDefense,     // 临时护甲
    Damage
}

// 元素
public enum DamageElement
{
    None,
    Fire,
    Water,
    Lightning,
    Nature,
}

// 作用对象
public enum TargetType
{
    Player,
    AllEnemies,
    CurrentEnemy,
    HighHpEnemy,
    HighAtkEnemy,
}
// 用于在战斗核心和 SymbolSystem 之间传递战斗数据
public struct AttackContextRuntime
{
    public CharacterRuntimeData attacker;   // runtime 数据的引用（注意：是引用，不是副本）
    public CharacterRuntimeData defender;
    public float baseDamage;
}
public class SymbolInstance
{
    public SymbolSO config;    // 原始 SO
    public int row;
    public int col;

    public SymbolInstance(SymbolSO config, int row, int col)
    {
        this.config = config;
        this.row = row;
        this.col = col;
    }
}
[Serializable]
public class DiceFace {
    public FaceDir dir;      // 方便调试的名字 (如 "Top", "Front")
    public Vector3 localDir; // 面的局部朝向 (如 Vector3.up)
    public int value;        // 这个面对应的数字
    public MeshRenderer mesh;
    public TMP_Text numberText;
}
public enum FaceDir
{
    Top,    //+Y
    Bottom, //-Y
    Front,  //+Z
    Back,   //-Z
    Right,  //+X
    Left,   //-X
}

public enum DiceType
{
    Attack,
    Defence
}