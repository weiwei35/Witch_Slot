using UnityEngine;

[System.Serializable]
public class BoosterEffect
{
    public BoosterSymbolSO Data { get; private set; }
    public int RemainingDuration { get; private set; }
    private int intervalCounter = 0;

    public bool IsImmediate => Data.durationType == BoosterDurationType.Immediate;

    public BoosterEffect(BoosterSymbolSO data)
    {
        Data = data;
        RemainingDuration = data.duration;
    }

    /// <summary>
    /// 应用效果到目标（即时或持续初次触发）
    /// </summary>
    public void Apply(CharacterDataSO target)
    {
        switch (Data.effectType)
        {
            case BoosterEffectType.Damage:
                ApplyDamage(target);
                break;

            case BoosterEffectType.AddAttack:
                target.strength += Data.effectValue;
                Debug.Log($"{target.characterName} 攻击力 +{Data.effectValue} ({Data.symbolName})");
                break;

            case BoosterEffectType.AddDefense:
                target.defense += Data.effectValue;
                Debug.Log($"{target.characterName} 防御力 +{Data.effectValue} ({Data.symbolName})");
                break;

            case BoosterEffectType.AddHP:
                target.currentHP = Mathf.Min(target.maxHP, target.currentHP + Data.effectValue);
                Debug.Log($"{target.characterName} 回复 {Data.effectValue} HP");
                break;

            case BoosterEffectType.SubAttack:
                target.strength = Mathf.Max(0, target.strength - Data.effectValue);
                Debug.Log($"{target.characterName} 攻击力 -{Data.effectValue} ({Data.symbolName})");
                break;

            case BoosterEffectType.SubDefense:
                target.defense = Mathf.Max(0, target.defense - Data.effectValue);
                Debug.Log($"{target.characterName} 防御力 -{Data.effectValue} ({Data.symbolName})");
                break;

            case BoosterEffectType.ExtraAttack:
                ApplyExtraAttack();
                break;
        }
    }

    /// <summary>
    /// Booster类型为 Damage 时，智能判断战斗内外目标
    /// </summary>
    private void ApplyDamage(CharacterDataSO target)
    {
        // 若当前处于战斗中，直接对 CharacterFight 实体扣血
        var currentEnemyFight = GameManager.Instance.CurrentEnemyFight;
        if (Data.targetType == BoosterTargetType.CurrentEnemy && currentEnemyFight != null)
        {
            currentEnemyFight.TakeDamage(Data.effectValue);
            AutoScrollLog.instance.AddLog($"{Data.symbolName} 造成 {Data.effectValue} 点伤害（战斗内）");
            return;
        }

        // 否则修改静态数据
        target.currentHP = Mathf.Max(0, target.currentHP - Data.effectValue);
        Debug.Log($"{Data.symbolName} 对 {target.characterName} 造成 {Data.effectValue} 点伤害");
    }

    /// <summary>
    /// 额外攻击逻辑：整合进当前战斗内伤害
    /// </summary>
    public void ApplyExtraAttack()
    {
        var fight = GameManager.Instance.fightSystem;
        if (fight == null || fight.enemy == null)
        {
            Debug.LogWarning($"⚠️ {Data.symbolName} 触发失败：无有效战斗上下文");
            return;
        }

        float extraDamage = Data.effectValue;
        fight.enemy.TakeDamage(extraDamage);

        AutoScrollLog.instance.AddLog($"{Data.symbolName} 触发，造成额外 {extraDamage} 点元素伤害");
    }

    /// <summary>
    /// 移除持续型效果
    /// </summary>
    public void Remove(CharacterDataSO target)
    {
        if (Data.effectType == BoosterEffectType.AddAttack)
            target.strength -= Data.effectValue;
        else if (Data.effectType == BoosterEffectType.AddDefense)
            target.defense -= Data.effectValue;
    }

    /// <summary>
    /// 生命周期推进（回合、攻击、战斗结束）
    /// </summary>
    public bool Tick(CharacterDataSO target, BoosterTriggerTiming timing)
    {
        switch (Data.durationType)
        {
            case BoosterDurationType.Immediate:
                return true;

            case BoosterDurationType.PerAttack:
                if (timing == BoosterTriggerTiming.OnAttack)
                    RemainingDuration--;
                break;

            case BoosterDurationType.PerBattle:
                if (timing == BoosterTriggerTiming.OnVictory)
                    RemainingDuration--;
                break;

            case BoosterDurationType.UntilHit:
                if (timing == BoosterTriggerTiming.OnTakeTrueDamage)
                    RemainingDuration = 0;
                break;

            case BoosterDurationType.IntervalAttack:
                // 不在 Tick 递减次数，由 ShouldTriggerInterval 控制
                break;
        }
        if (RemainingDuration <= 0)
        {
            Remove(target);
            AutoScrollLog.instance.AddLog($"{Data.symbolName} 效果结束");
            return true;
        }

        return false;
    }

    /// <summary>
    /// 判断是否满足间隔攻击触发
    /// </summary>
    public bool ShouldTriggerInterval()
    {
        if (Data.effectType != BoosterEffectType.ExtraAttack ||
            Data.durationType != BoosterDurationType.IntervalAttack)
            return false;

        intervalCounter++;
        if (intervalCounter < Mathf.Max(1, Data.intervalCount))
            return false;

        intervalCounter = 0;
        if (RemainingDuration > 0)
            RemainingDuration--;

        return true;
    }
}
