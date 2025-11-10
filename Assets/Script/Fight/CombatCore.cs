using UnityEngine;

public class CombatCore
{
    public CombatRuntime data;
    private bool playerTurn = true;   // ✅ 记录现在轮到谁攻击

    public CombatCore(CombatRuntime r)
    {
        data = r;
    }

    // 战斗是否结束
    public bool IsOver =>
        data.Player.CurrentHP <= 0 || data.Enemy.CurrentHP <= 0;

    /// <summary>
    /// 单步执行一次攻击
    /// </summary>
    public int DoStep()
    {
        if (IsOver) return 0;

        int count = 0;
        if (playerTurn)
        {
            count = Attack(data.Player, data.Enemy);

            if (data.Enemy.CurrentHP <= 0)
                AutoScrollLog.instance.AddLog("\n敌人阵亡!");
            
            data.Player.TickAttackEnd();
        }
        else
        {
            count = Attack(data.Enemy, data.Player);

            if (data.Player.CurrentHP <= 0)
                AutoScrollLog.instance.AddLog("\n你阵亡!");
            
            data.Player.TickHittedEnd();
        }

        // ✅ 交换轮次
        playerTurn = !playerTurn;
        return count;
    }

    private int Attack(CharacterRuntimeData attacker, CharacterRuntimeData defender)
    {
        string attackerName = attacker.Name;
        string defenderName = defender.Name;

        float atk = attacker.Strength;

        float real = DealDamage(defender, atk);

        if (real <= 0)
        {
            AutoScrollLog.instance.AddLog(
                $"{attackerName} 攻击 {defenderName}\n" +
             $"   {atk} 点伤害被护甲吸收\n" +
             $"   剩余护甲：{defender.Defense}"
                );
        }
        else
        {
            AutoScrollLog.instance.AddLog(
                $"{attackerName} 攻击 {defenderName}\n" +
                $"   HP -{real}   剩余HP：{defender.CurrentHP}"
                );
        }
        
        if (playerTurn && attacker.CurrentHP > 0 && defender.CurrentHP > 0)
        {
            return SymbolSystem.Instance.NotifyAttackWithLog(
                TriggerEvent.OnAfterAttack,
                new AttackContextRuntime
                {
                    attacker = attacker,
                    defender = defender,
                    baseDamage = atk
                }
            );
        }

        return 0;
    }

    /// <summary>
    /// 处理伤害（护甲吸收 + HP 扣减）
    /// 返回最终扣除的生命
    /// </summary>
    public static float DealDamage(CharacterRuntimeData defender, float damage)
    {
        if (damage <= 0) return 0;

        // 护甲足够 → 吸收全部
        if (defender.Defense >= damage)
        {
            defender.SetDefense(defender.Defense - damage);
            return 0f;
        }

        // 护甲不够 → 部分伤害溢出
        float absorbed = defender.Defense;
        float real = damage - absorbed;

        defender.SetDefense(0);
        defender.SetHP(defender.CurrentHP - real);

        return real;
    }
}