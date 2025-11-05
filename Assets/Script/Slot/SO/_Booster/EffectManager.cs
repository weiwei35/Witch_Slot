using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 管理所有激活中的 BoosterEffect
/// </summary>
[System.Serializable]
public class EffectManager
{
    private readonly Dictionary<CharacterDataSO, List<BoosterEffect>> activeEffects = new();

    /// <summary>
    /// 添加一个新效果
    /// </summary>
    public void AddEffect(BoosterEffect effect, CharacterDataSO target)
    {
        if (effect == null || target == null) return;

        if (!activeEffects.ContainsKey(target))
            activeEffects[target] = new List<BoosterEffect>();

        // 如果是即时效果，立即执行然后不存入列表
        if (effect.IsImmediate)
        {
            effect.Apply(target);
            return;
        }

        // 持续型效果：立即应用一次（通常为增益类）
        if(effect.Data.triggerTiming != BoosterTriggerTiming.OnVictory)
        {
            if (effect.Data.effectType == BoosterEffectType.AddAttack ||
                effect.Data.effectType == BoosterEffectType.AddDefense)
            {
                effect.Apply(target);
            }
        }

        activeEffects[target].Add(effect);
        Debug.Log($"注册持续效果 {effect.Data.symbolName} 于 {target.characterName}");
    }

    /// <summary>
    /// 通用Tick入口，在 GameManager 中由事件驱动
    /// </summary>
    public void OnEvent(CharacterDataSO target, BoosterTriggerTiming timing)
    {
        if (!activeEffects.ContainsKey(target)) return;

        var list = activeEffects[target];
        for (int i = list.Count - 1; i >= 0; i--)
        {
            bool expired = list[i].Tick(target, timing);
            if (expired)
            {
                list.RemoveAt(i);
            }
        }
    }

    /// <summary>
    /// 战斗结束后调用，用于减少Battle型效果
    /// </summary>
    public void OnBattleEnd(CharacterDataSO target)
    {
        OnEvent(target, BoosterTriggerTiming.OnVictory);
    }

    /// <summary>
    /// 攻击时调用，用于处理Attack型和间隔型
    /// </summary>
    public void OnAttack(CharacterDataSO target)
    {
        OnEvent(target, BoosterTriggerTiming.OnAttack);
        
        // 🔥 检查所有 ExtraAttack（IntervalAttack 类型）
        if (!activeEffects.ContainsKey(target)) return;

        var effects = activeEffects[target];
        foreach (var booster in effects)
        {
            if (booster.Data.effectType == BoosterEffectType.ExtraAttack &&
                booster.Data.durationType == BoosterDurationType.IntervalAttack &&
                booster.ShouldTriggerInterval())
            {
                // 满足条件：执行额外攻击
                booster.ApplyExtraAttack();
            }
        }
    }

    /// <summary>
    /// 当受到真实伤害时调用，用于UntilHit型
    /// </summary>
    public void OnTakeTrueDamage(CharacterDataSO target)
    {
        OnEvent(target, BoosterTriggerTiming.OnTakeTrueDamage);
    }

    /// <summary>
    /// 清空所有效果（例如全局重置）
    /// </summary>
    public void ClearAll()
    {
        foreach (var pair in activeEffects)
        {
            foreach (var eff in pair.Value)
                eff.Remove(pair.Key);
        }
        activeEffects.Clear();
        Debug.Log("已清除所有持续效果");
    }

    /// <summary>
    /// 获取目标所有激活效果
    /// </summary>
    public List<BoosterEffect> GetEffects(CharacterDataSO target)
    {
        if (!activeEffects.ContainsKey(target)) return new List<BoosterEffect>();
        return activeEffects[target];
    }
}
