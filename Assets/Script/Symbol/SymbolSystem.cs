using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SymbolSystem : MonoBehaviour
{
    public static SymbolSystem Instance { get; private set; }

    /// <summary>可触发的 Booster 运行时记录</summary>
    private List<SymbolRuntime> boosters = new();
    
    public Queue<SymbolInstance> executionQueue = new();

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    /// <summary>
    /// Slot 解析后执行
    /// normals → 永久加成
    /// instant → 即刻执行
    /// boosters → 生成 Buff / 触发器
    /// </summary>
    public void ApplySlotResult(List<SymbolSO> symbols)
    {
        var player = GameManager.Instance.GetPlayer();
        if (player == null) return;

        foreach (var symbolDic in uiLookup)
        {
            if (symbolDic.Key.config.category == SymbolCategory.Normal
                || symbolDic.Key.config.category == SymbolCategory.Instant
                || !symbolDic.Key.config.needTrigger)
            {
                executionQueue.Enqueue(symbolDic.Key);
            }
            else
            {
                ApplySymbol(symbolDic.Key);
            }
        }
        StartCoroutine(ResolveSymbols_instant());
    }

    #region === NORMAL ===
    void ApplyNormal(Player player, SymbolSO symbol)
    {
        foreach (var e in symbol.effects)
        {
            switch (e.effectType)
            {
                case EffectType.ModifyAttack:
                    player.runtimeData.AddBaseStrength(e.value);
                    break;

                case EffectType.ModifyDefense:
                    player.runtimeData.AddBaseDefense(e.value);
                    break;
            }
        }

        Debug.Log($"[Symbol] Normal → {symbol.displayName}");
    }
    #endregion

    #region === INSTANT ===
    void ApplyInstant(SymbolSO symbol)
    {
        foreach (var e in symbol.effects)
            ApplyInstantEffect(e);

        Debug.Log($"[Symbol] Instant → {symbol.displayName}");
    }

    void ApplyInstantEffect(SymbolEffectConfig e)
    {
        switch (e.target)
        {
            case TargetType.Player:
                ApplyInstantToPlayer(e);
                break;

            case TargetType.AllEnemies:
                ApplyInstantToAllEnemies(e);
                break;

            case TargetType.CurrentEnemy:
                ApplyInstantToCurrentEnemy(e);
                break;
        }
    }

    void ApplyInstantToPlayer(SymbolEffectConfig e)
    {
        var player = GameManager.Instance.GetPlayer();
        if (player == null) return;

        switch (e.effectType)
        {
            case EffectType.ModifyAttack:
                player.AddStrength(e.value);
                break;

            case EffectType.ModifyDefense:
                player.AddDefense(e.value);
                break;

            case EffectType.ModifyHP:
                player.AddHP(e.value);
                break;
        }
    }

    void ApplyInstantToAllEnemies(SymbolEffectConfig e)
    {
        foreach (var enemy in GameManager.Instance.GetEnemies())
            ApplyInstantToEnemy(e, enemy);
    }

    void ApplyInstantToCurrentEnemy(SymbolEffectConfig e)
    {
        var enemy = GameManager.Instance.GetCurrentEnemy();//问题：无法获取当前enemy因为在战斗开始前没有这个概念
        if (enemy != null)
            ApplyInstantToEnemy(e, enemy);
    }

    void ApplyInstantToEnemy(SymbolEffectConfig e, Enemy enemy)
    {
        switch (e.effectType)
        {
            case EffectType.ModifyAttack:
                enemy.runtimeData.AddBaseStrength(e.value);
                break;

            case EffectType.ModifyDefense:
                enemy.runtimeData.AddBaseDefense(e.value);
                break;

            case EffectType.ModifyHP:
                enemy.runtimeData.AddHP(e.value);
                break;

            case EffectType.ElementDamage:
            case EffectType.Damage:
                enemy.TakeDamage(e.value, e.element);
                break;
        }
    }
    #endregion

    #region === BOOSTER ===
    void AddBooster(Player player, SymbolInstance symbol)
    {
        if (!symbol.config.needTrigger)//没有前置条件的buff直接添加到player
        {
            foreach (var e in symbol.config.effects)
            {
                BuffRuntime buff = new(e);
                player.runtimeData.AddBuff(buff);
            }
        }
        
        boosters.Add(new(symbol));

        Debug.Log($"[Symbol] Booster Added → {symbol.config.displayName}");
    }

    /// <summary> 战斗结束时调用（减少 remainBattles） </summary>
    public void OnBattleEnd()
    {
        var player = GameManager.Instance.GetPlayer();
        if (player == null) return;

        // 持续战斗类 buff 衰减
        player.runtimeData.TickBattleEnd();
    }
    #endregion

    #region === EVENT ===
    public void NotifyEvent(TriggerEvent evt, object evtParam = null)
    {
        foreach (var rt in boosters.ToArray())
            TryTriggerBooster(rt, evt, evtParam);

        if (evt == TriggerEvent.OnVictory)
        {
            StartCoroutine(ResolveSymbols_victory());
        }
    }

    private void TryTriggerBooster(SymbolRuntime rt, TriggerEvent evt, object evtParam)
    {
        var so = rt.baseData;

        // 检查触发条件
        if (!so.triggers.Contains(evt))
            return;

        if(evt == TriggerEvent.OnVictory)
        {
            executionQueue.Enqueue(rt.inst);
            // VictoryEffects(so);
        }
        // interval
        if (so.interval > 1)
        {
            rt.intervalCounter++;
            if (rt.intervalCounter < so.interval)
                return;
            rt.intervalCounter = 0;
        }

        if (so.isConsumedAfterTrigger)
            boosters.Remove(rt);
    }

    private void VictoryEffects(SymbolSO so)
    {
        foreach (var e in so.effects)
        {
            Character target = SelectTarget(e);
            switch (e.effectType)
            {
                case EffectType.ElementDamage:
                    target.TakeDamage(e.value, e.element);
                    break;
                case EffectType.ModifyHP:
                    target.runtimeData.AddHP(e.value);
                    break;
                case EffectType.TemporaryAttack:
                case EffectType.TemporaryDefense:
                    BuffRuntime buff = new(e);
                    target.runtimeData.AddBuff(buff);
                    break;
                case EffectType.ModifyDefense:
                    target.runtimeData.AddBaseDefense(e.value);
                    break;
                case EffectType.ModifyAttack:
                    target.runtimeData.AddBaseStrength(e.value);
                    break;
            }
            target.runtimeData.ClearTriggeredBuffs();
        }
    }

    private Character SelectTarget(SymbolEffectConfig effect)
    {
        switch (effect.target)
        {
            case TargetType.Player:
                return GameManager.Instance.GetPlayer();
            case TargetType.HighHpEnemy:
                return GameManager.Instance.GetHighHpEnemy();
            case TargetType.HighAtkEnemy:
                return GameManager.Instance.GetHighAtkEnemy();
        }
        return null;
    }
    /// <summary>
    /// 攻击事件（OnAfterAttack）
    /// 通知 Booster 并返回追加日志
    /// </summary>
    public int NotifyAttackWithLog(TriggerEvent evt, AttackContextRuntime ctx)
    {
        int count = 0;
        foreach (var rt in boosters.ToArray())
        {
            var so = rt.baseData;

            // 过滤：是否关注该事件
            if (!so.triggers.Contains(evt))
                continue;

            // interval
            if (so.interval > 1)
            {
                rt.intervalCounter++;
                if (rt.intervalCounter < so.interval)
                    continue;
                rt.intervalCounter = 0;
            }
            if(evt == TriggerEvent.OnAfterAttack)
            {
                count ++;
                executionQueue.Enqueue(rt.inst);
            }
            // ExtraAttackEffectsWithLog(so, ctx);

            // 一次性 symbol → 移除
            if (so.isConsumedAfterTrigger)
                boosters.Remove(rt);
        }
        // 执行逻辑
        if (evt == TriggerEvent.OnAfterAttack)
        {
            StartCoroutine(ResolveSymbols_extraAttack(ctx));
        }

        return count;
    }


    /// <summary>
    /// 执行 symbol 并返回日志
    /// </summary>
    private void ExtraAttackEffectsWithLog(SymbolSO so, AttackContextRuntime ctx)
    {
        foreach (var e in so.effects)
        {
            switch (e.effectType)
            {
                case EffectType.ElementDamage:
                case EffectType.Damage:
                {
                    float final = CombatCore.DealDamage(ctx.defender, e.value);
                    AutoScrollLog.instance.AddLog($"{so.displayName} 触发 → 对 {ctx.defender.Name} 造成 {final} 点额外伤害");
                    break;
                }
            }
        }
    }
    #endregion

    public List<SymbolRuntime> GetBoosters() => boosters;
    
    private IEnumerator ResolveSymbols_instant()
    {
        while (executionQueue.Count > 0)
        {
            var inst = executionQueue.Dequeue();
            yield return PlaySymbolAnimation(inst);
            ApplySymbol(inst);
            // yield return new WaitForSeconds(0.1f);
        }
    }
    private IEnumerator ResolveSymbols_victory()
    {
        while (executionQueue.Count > 0)
        {
            var inst = executionQueue.Dequeue();
            yield return PlaySymbolAnimation(inst);
            VictoryEffects(inst.config);
            // yield return new WaitForSeconds(0.1f);
        }
    }
    private IEnumerator ResolveSymbols_extraAttack(AttackContextRuntime ctx)
    {
        while (executionQueue.Count > 0)
        {
            var inst = executionQueue.Dequeue();
            yield return PlaySymbolAnimation(inst);
            ExtraAttackEffectsWithLog(inst.config, ctx);
            // yield return new WaitForSeconds(0.1f);
        }
    }
    private void ApplySymbol(SymbolInstance so)
    {
        switch (so.config.category)
        {
            case SymbolCategory.Normal:  ApplyNormal(GameManager.Instance.GetPlayer(),so.config); break;
            case SymbolCategory.Instant:  ApplyInstant(so.config); break;
            case SymbolCategory.Booster:  AddBooster(GameManager.Instance.GetPlayer(),so); break;
        }
    }
    public Dictionary<SymbolInstance, Symbol> uiLookup = new();
    IEnumerator PlaySymbolAnimation(SymbolInstance inst)
    {
        if (!uiLookup.TryGetValue(inst, out var ui))
            yield break;
        if(inst.config.symbolId.Contains("EMPTY"))
            yield break;
        ui.SetActiveAnimation(true);

        while (!ui.IsAnimationFinished)
            yield return null;
    }
}
