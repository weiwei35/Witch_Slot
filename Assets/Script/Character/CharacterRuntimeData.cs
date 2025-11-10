using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CharacterRuntimeData
{
    public string Name;
    public Sprite Icon;

    // 基础属性（永久值）
    private float baseStrength;
    private float baseDefense;
    private float baseMaxHP;

    // 当前状态（可变化）
    public float CurrentHP;

    // Buff 列表（动态加成）
    public List<BuffRuntime> buffs = new();

    public event Action OnValueChanged;

    public CharacterRuntimeData(CharacterDataSO baseData)
    {
        Name = baseData.name;
        baseStrength = baseData.strength;
        baseDefense = baseData.defense;
        baseMaxHP = baseData.maxHP;
        CurrentHP = baseMaxHP;
        Icon = baseData.icon;
    }

    public CharacterRuntimeData Clone()
    {
        var clone = (CharacterRuntimeData)MemberwiseClone();
        clone.buffs = buffs;
        return clone;
    }

    // ✅ 动态属性（= base + buffs）
    public float Strength => baseStrength + GetBuffValue(EffectType.TemporaryAttack);
    public float Defense  => baseDefense + GetBuffValue(EffectType.TemporaryDefense);
    public float MaxHP    => baseMaxHP;
    public float CurrentHPPercent => CurrentHP / MaxHP;

    private float GetBuffValue(EffectType match)
    {
        float total = 0;
        foreach (var b in buffs)
            if (b.effectType == match)
                total += b.value;
        return total;
    }

    // --- 修改基础属性（Normal Symbol 永久改变） ---
    public void AddBaseStrength(float v)
    {
        baseStrength += v;
        OnValueChanged?.Invoke();
    }

    public void AddBaseDefense(float v)
    {
        baseDefense += v;
        OnValueChanged?.Invoke();
    }

    public void AddHP(float v)
    {
        CurrentHP = Mathf.Clamp(CurrentHP + v, 0, MaxHP);
        OnValueChanged?.Invoke();
    }

    public void SetHP(float value)
    {
        CurrentHP = Mathf.Clamp(value, 0, MaxHP);
        OnValueChanged?.Invoke();
    }

    public void SetDefense(float value)
    {
        baseDefense = Mathf.Max(0, value);
        OnValueChanged?.Invoke();
    }

    public void SetStrength(float value)
    {
        baseStrength = value;
        OnValueChanged?.Invoke();
    }

    // ✅ Buff 管理（用于 Booster）
    public void AddBuff(BuffRuntime buff)
    {
        buffs.Add(buff);
        OnValueChanged?.Invoke();
    }

    public void RemoveBuff(BuffRuntime buff)
    {
        buffs.Remove(buff);
        OnValueChanged?.Invoke();
    }

    public void TickBattleEnd()
    {
        for (int i = buffs.Count - 1; i >= 0; i--)
        {
            if(!buffs[i].needCountBattle) continue;
            if (buffs[i].remainBattles > 0)
            {
                buffs[i].remainBattles--;
                if (buffs[i].remainBattles == 0)
                    buffs.RemoveAt(i);
            }
        }
        OnValueChanged?.Invoke();
    }
    public void TickAttackEnd()
    {
        for (int i = buffs.Count - 1; i >= 0; i--)
        {
            if(!buffs[i].needCountAttack) continue;
            if (buffs[i].remainAttacks > 0)
            {
                buffs[i].remainAttacks--;
                if (buffs[i].remainAttacks == 0)
                    buffs.RemoveAt(i);
            }
        }
        OnValueChanged?.Invoke();
    }
    public void TickHittedEnd()
    {
        for (int i = buffs.Count - 1; i >= 0; i--)
        {
            if(!buffs[i].needCountHitted) continue;
            if (buffs[i].remainHitted > 0)
            {
                buffs[i].remainHitted--;
                if (buffs[i].remainAttacks == 0)
                    buffs.RemoveAt(i);
            }
        }
        OnValueChanged?.Invoke();
    }

    public void ClearTriggeredBuffs()
    {
        for (int i = buffs.Count - 1; i >= 0; i--)
        {
            if(!buffs[i].removeAfterTrigger) continue;
            buffs.RemoveAt(i);
        }
        OnValueChanged?.Invoke();
    }
}
