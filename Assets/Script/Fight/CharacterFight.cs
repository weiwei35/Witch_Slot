using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 战斗逻辑层：处理战斗与临时效果（不直接操作 UI）
/// </summary>
public class CharacterFight : MonoBehaviour
{
    [Header("战斗属性")]
    public CharacterStats Stats;

    [Header("临时效果")]
    private TemporaryEffect tempAttackEffect;
    private TemporaryEffect tempDefenseEffect;
    
    [Header("UI 绑定")]
    [SerializeField] private CharacterHUD hud; // ✅ 直接引用 HUD 组件
    public bool IsAlive => Stats != null && Stats.IsAlive;
    
    public EffectManager effectManager = new EffectManager();

    public void Initialize(float strength, float defense, float maxHP, float currentHP, Sprite icon)
    {
        // 初始化数据
        Stats = new CharacterStats(strength, defense, maxHP, currentHP, icon);

        // 通知初始状态
        Stats.NotifyChanged();

        // ✅ 若引用了 HUD，则绑定
        if (hud != null)
        {
            hud.BindStats(Stats);
        }
        else
        {
            Debug.LogWarning($"[{name}] 未绑定 HUD 引用，无法显示角色数据。");
        }
    }

    public float Attack(CharacterFight target)
    {
        float damage = Mathf.Max(0, Stats.Strength - target.Stats.Defense);
        target.TakeDamage(damage);
        return damage;
    }

    public float ExtraAttack(CharacterFight target, float amount)
    {
        float damage = amount + Mathf.Max(0, Stats.Strength - target.Stats.Defense);
        target.TakeDamage(damage);
        return damage;
    }

    public virtual void TakeDamage(float damage)
    {
        Stats.TakeDamage(damage);
    }

    public void Heal(float amount)
    {
        Stats.Heal(amount);
    }

    // === 临时效果部分 ===
    public void ApplyTemporaryAttack(TemporaryEffect effect)
    {
        ClearTemporaryAttack();
        tempAttackEffect = effect;
        Stats.Strength += effect.Amount;
    }

    public void ApplyTemporaryDefense(TemporaryEffect effect)
    {
        ClearTemporaryDefense();
        tempDefenseEffect = effect;
        Stats.Defense += effect.Amount;
    }

    public void ClearTemporaryAttack()
    {
        if (tempAttackEffect == null) return;
        Stats.Strength -= tempAttackEffect.Amount;
        tempAttackEffect = null;
    }

    public void ClearTemporaryDefense()
    {
        if (tempDefenseEffect == null) return;
        Stats.Defense -= tempDefenseEffect.Amount;
        tempDefenseEffect = null;
    }

    public void ClearAllEffects()
    {
        ClearTemporaryAttack();
        ClearTemporaryDefense();
    }
}
