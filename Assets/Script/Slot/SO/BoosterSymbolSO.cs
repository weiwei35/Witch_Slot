using UnityEngine;

[CreateAssetMenu(menuName = "Game/Symbol/Booster Symbol", fileName = "BoosterSymbol")]
public class BoosterSymbolSO : BaseSymbolSO
{
    [Header("Booster 逻辑属性")]
    public BoosterTriggerTiming triggerTiming;
    public BoosterTargetType targetType;
    public BoosterEffectType effectType;
    public float effectValue = 1f;   // 数值大小
    public int duration = 1;         // 持续场次 / 触发次数
    
    [Header("生命周期逻辑")]
    public BoosterDurationType durationType = BoosterDurationType.Immediate;
    public int intervalCount = 1; // 仅当 IntervalAttack 时有效
}
public enum BoosterTriggerTiming
{
    OnSpinEnd,
    OnAttack,
    OnVictory,
    OnTakeTrueDamage
}

public enum BoosterTargetType
{
    Player,
    AllEnemies,
    CurrentEnemy,
    HighestHPEnemy,
    HighestAttackEnemy
}

public enum BoosterEffectType
{
    Damage,
    AddAttack,
    AddDefense,
    AddHP,
    SubAttack,
    SubDefense,
    ExtraAttack
}
public enum BoosterDurationType
{
    Immediate,          // 即时触发一次（不注册）
    PerAttack,          // 每次攻击递减持续次数
    PerBattle,          // 每场战斗递减持续次数
    UntilHit,           // 受真实伤害后移除
    IntervalAttack      // 每X次攻击触发一次（不递减）
}