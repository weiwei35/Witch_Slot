using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.UI;

public class CharacterFight : MonoBehaviour
{
    public float Strength { get; private set; }
    public float Defense { get; private set; }
    public float MaxHP { get; private set; }
    public float CurrentHP { get; private set; }
    private TemporaryEffect temporaryAttackEffect = null;
    protected TemporaryEffect temporaryDefenseEffect = null;
    public bool IsAlive => CurrentHP > 0;
    public TMP_Text strengthText;
    public TMP_Text defenseText;
    public Scrollbar hpScrollbar;
    public Image iconImage;
    public void Initialize(float strength, float defense, float maxHP, float currentHP,Sprite icon)
    {
        strengthText.text = (Strength = strength).ToString();
        defenseText.text = (Defense = defense).ToString();
        MaxHP = maxHP;
        CurrentHP = currentHP;
        hpScrollbar.size = CurrentHP / MaxHP;
        iconImage.sprite = icon;
    }
    public float Attack(CharacterFight target)
    {
        float damage = Strength - target.Defense;
        if (damage < 0) damage = 0;
        
        target.TakeDamage(damage);
        return damage;
    }
    public float ExtraAttack(CharacterFight target,float amount)
    {
        float damage = amount;
        if (target.Defense > Strength)
            damage = amount + Strength - target.Defense;
        if (damage < 0) damage = 0;
        
        target.TakeDamage(damage);
        return damage;
    }
    public virtual void TakeDamage(float damage)
    {
        CurrentHP = Mathf.Max(0, CurrentHP - damage);
        hpScrollbar.size = (float)CurrentHP / MaxHP;
    }

    public void Heal(int amount)
    {
        CurrentHP = Mathf.Min(MaxHP, CurrentHP + amount);
    }

    public void FightRound()
    {
        if (temporaryAttackEffect != null && temporaryAttackEffect.Type == TempType.Time)
        {
            temporaryAttackEffect.Round--;
            if (temporaryAttackEffect.Round <= 0)
            {
                ClearTemporaryAttack();
            }
        }
    }

    /// <summary>
    /// 应用临时攻击力
    /// </summary>
    public void ApplyTemporaryAttack(object o)
    {
        TemporaryEffect temp = (TemporaryEffect)o;
        temporaryAttackEffect = temp;
        Strength += temporaryAttackEffect.Amount;
        UpdateStrengthText(); // 可选：UI 更新
    }
    public void ApplyTemporaryProtect(object o)
    {
        TemporaryEffect temp = (TemporaryEffect)o;
        temporaryDefenseEffect = temp;
        Defense += temporaryDefenseEffect.Amount;
        UpdateStrengthText(); // 可选：UI 更新
    }

    /// <summary>
    /// 清除当前临时攻击力
    /// </summary>
    private void ClearTemporaryAttack()
    {
        Strength -= temporaryAttackEffect.Amount;
        temporaryAttackEffect = null;
        UpdateStrengthText(); // 可选：UI 更新
    }
    protected void ClearTemporaryDefence()
    {
        Defense -= temporaryDefenseEffect.Amount;
        temporaryDefenseEffect = null;
        UpdateStrengthText(); // 可选：UI 更新
    }

    /// <summary>
    /// 清除所有临时状态（战斗结束时调用）
    /// </summary>
    public void ClearAllTemporaryEffects()
    {
        ClearTemporaryAttack();
        // 可扩展：其他状态清理
    }

    /// <summary>
    /// 更新攻击力显示（可选，仅用于 UI 展示）
    /// </summary>
    private void UpdateStrengthText()
    {
        strengthText.text = Strength.ToString();
        defenseText.text = Defense.ToString();
    }

}
