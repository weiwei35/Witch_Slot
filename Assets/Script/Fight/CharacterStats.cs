using System;
using UnityEngine;

/// <summary>
/// 纯数据层：存储角色基础属性
/// </summary>
[System.Serializable]
public class CharacterStats
{
    public float Strength;
    public float Defense;
    public float MaxHP;
    public float CurrentHP;
    public Sprite Icon;

    /// <summary>
    /// 当任意属性变化时触发（用于通知 UI）
    /// </summary>
    public event Action OnValueChanged;
    public void NotifyChanged() => OnValueChanged?.Invoke();

    public bool IsAlive => CurrentHP > 0;

    public CharacterStats(float strength, float defense, float maxHP, float currentHP, Sprite icon)
    {
        Strength = strength;
        Defense = defense;
        MaxHP = maxHP;
        CurrentHP = currentHP;
        Icon = icon;
    }

    public void TakeDamage(float amount)
    {
        CurrentHP = Mathf.Max(0, CurrentHP - amount);
        OnValueChanged?.Invoke(); // ✅ 通知监听者
    }

    public void Heal(float amount)
    {
        CurrentHP = Mathf.Min(MaxHP, CurrentHP + amount);
        OnValueChanged?.Invoke(); // ✅ 通知监听者
    }

    public void ModifyStrength(float delta)
    {
        Strength += delta;
        OnValueChanged?.Invoke(); // ✅ 通知监听者
    }

    public void ModifyDefense(float delta)
    {
        Defense += delta;
        OnValueChanged?.Invoke(); // ✅ 通知监听者
    }

    public void SetIcon(Sprite newIcon)
    {
        Icon = newIcon;
        OnValueChanged?.Invoke(); // ✅ 通知监听者
    }
}