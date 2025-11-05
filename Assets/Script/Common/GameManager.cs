using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("核心引用")]
    public Player player;
    public FightSystem fightSystem;
    public CharacterFight PlayerFight;
    public CharacterFight CurrentEnemyFight;
    public Enemy CurrentEnemy;

    [Header("全局数据")]
    public List<BoosterSymbolSO> ActiveBoosters = new();
    public List<Enemy> ActiveEnemies = new();

    [Header("效果管理器")]
    public EffectManager globalEffectManager = new EffectManager();

    // ======================================================
    // ⚡️ 新增：额外攻击缓存（本轮整合触发）
    // ======================================================
    private float pendingExtraDamage = 0f;
    private string pendingExtraSource = null;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // ======================================================
    // 🧩 敌人注册 / 注销
    // ======================================================
    public void RegisterEnemy(Enemy e)
    {
        if (!ActiveEnemies.Contains(e)) ActiveEnemies.Add(e);
    }

    public void UnregisterEnemy(Enemy e)
    {
        ActiveEnemies.Remove(e);
    }

    // ======================================================
    // 🧠 Booster 注册与触发
    // ======================================================
    public void AddBooster(BoosterSymbolSO booster)
    {
        if (!ActiveBoosters.Contains(booster))
        {
            ActiveBoosters.Add(booster);
            Debug.Log($"新 Booster 加入全局: {booster.symbolName}");
        }
    }

    public void TriggerBooster(BoosterTriggerTiming timing)
    {
        BoosterTriggerSystem.TriggerBoosters(
            timing,
            ActiveBoosters,
            player.GetData(),
            ActiveEnemies
        );
    }

    // ======================================================
    // ⚔️ 战斗流程
    // ======================================================
    public void StartBattle(Player playerComponent, Enemy enemyComponent)
    {
        CurrentEnemy = enemyComponent;

        var pd = playerComponent.GetData();
        PlayerFight.Initialize(pd.strength, pd.defense, pd.maxHP, pd.currentHP, pd.icon);

        var ed = enemyComponent.GetData();
        CurrentEnemyFight.Initialize(ed.strength, ed.defense, ed.maxHP, ed.currentHP, ed.icon);

        // ⚡ 开战前触发“急速类” Booster
        // TriggerBooster(BoosterTriggerTiming.OnSpinEnd);

        // 绑定战斗双方
        fightSystem.player = PlayerFight;
        fightSystem.enemy = CurrentEnemyFight;
        fightSystem.ShowFightPanel();
    }

    public void EndBattle(bool playerWin)
    {
        if (playerWin)
            TriggerBooster(BoosterTriggerTiming.OnVictory);  // ✅ 仅此一处

        // 战斗结束生命周期事件（让 PerBattle/UntilHit 等计时递减）
        OnBattleEndEvent();

        // 清掉一次性临时缓存（若你用了额外伤害缓存）
        pendingExtraDamage = 0;
        pendingExtraSource = null;

        CurrentEnemy = null;
        GameStateManager.Instance.SetState(GameState.Walking);
    }

    // ======================================================
    // 🔁 Booster 生命周期事件入口
    // ======================================================
    public void OnAttackEvent()
    {
        var playerData = player.GetData();
        globalEffectManager.OnAttack(playerData);
        foreach (var e in ActiveEnemies)
            globalEffectManager.OnAttack(e.GetData());
    }

    public void OnTakeTrueDamageEvent()
    {
        var playerData = player.GetData();
        globalEffectManager.OnTakeTrueDamage(playerData);
        foreach (var e in ActiveEnemies)
            globalEffectManager.OnTakeTrueDamage(e.GetData());
    }

    public void OnBattleEndEvent()
    {
        var playerData = player.GetData();
        globalEffectManager.OnBattleEnd(playerData);
        foreach (var e in ActiveEnemies)
            globalEffectManager.OnBattleEnd(e.GetData());
    }

    public void OnTurnEnd()
    {
        var playerData = player.GetData();
    }

    // ======================================================
    // ⚡️ 新增：额外攻击接口
    // ======================================================

    /// <summary>
    /// 注册额外攻击（在 BoosterEffect.Apply 中调用）
    /// </summary>
    public void RegisterExtraAttack(float amount, string source)
    {
        pendingExtraDamage += amount;
        pendingExtraSource = source;
    }

    /// <summary>
    /// 取出并清空当前缓存的额外攻击伤害（在 FightSystem 中使用）
    /// </summary>
    public float ConsumeExtraAttack(out string source)
    {
        float dmg = pendingExtraDamage;
        source = pendingExtraSource;
        pendingExtraDamage = 0f;
        pendingExtraSource = null;
        return dmg;
    }

    // ======================================================
    // 🧹 清理
    // ======================================================
    private void CleanupExpiredEffects()
    {
        var expired = new List<BoosterSymbolSO>();
        foreach (var b in ActiveBoosters)
        {
            if (b.durationType == BoosterDurationType.PerBattle && b.duration <= 0)
                expired.Add(b);
        }

        foreach (var e in expired)
        {
            ActiveBoosters.Remove(e);
            Debug.Log($"🗑️ Booster 已失效并移除：{e.symbolName}");
        }
    }
}
