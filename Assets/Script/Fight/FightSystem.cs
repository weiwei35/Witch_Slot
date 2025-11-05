using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FightSystem : MonoBehaviour
{
    [Header("UI 控制")]
    [SerializeField] private Button startButton;
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject content;
    [SerializeField] private TMP_Text logText;

    [Header("战斗角色引用")]
    public CharacterFight player;
    public CharacterFight enemy;

    [Header("事件引用")]
    public CharacterFightEventSO WinFightEvent;
    public CharacterFightEventSO FightRoundEvent;

    private int roundCount = 0;
    private bool isFighting = false;
    private Enemy currentEnemy;

    public void ShowFightPanel()
    {
        if (startButton != null)
            startButton.onClick.AddListener(StartBattle);
        content.SetActive(true);
    }

    public void Initialize(CharacterFight playerFight, CharacterFight enemyFight, Enemy enemySource)
    {
        player = playerFight;
        enemy = enemyFight;
        currentEnemy = enemySource;
    }

    private void StartBattle()
    {
        if (isFighting) return;

        startButton.interactable = false;
        isFighting = true;
        roundCount = 0;

        AutoScrollLog.instance.AddLog("战斗开始！");
        StartCoroutine(BattleCoroutine());
    }

    private IEnumerator BattleCoroutine()
    {
        while (player.IsAlive && enemy.IsAlive)
        {
            roundCount++;

            // ============================
            // 玩家攻击阶段
            // ============================
            yield return ExecuteAttackPhase(player, enemy, isPlayer: true);
            if (!enemy.IsAlive) break;

            // ============================
            // 敌人攻击阶段
            // ============================
            yield return ExecuteAttackPhase(enemy, player, isPlayer: false);
            if (!player.IsAlive) break;

            // ============================
            // 回合结束：Booster Tick
            // ============================
            GameManager.Instance.OnTurnEnd();
            FightRoundEvent.RaiseEvent(player, this);
        }

        // ============================
        // 战斗结果
        // ============================
        if (player.IsAlive)
        {
            AutoScrollLog.instance.AddLog("战斗胜利！");
            WinFight(); // ✅ 不再这里触发 OnVictory
        }
        else
        {
            AutoScrollLog.instance.AddLog("玩家战败...");
            LoseFight();
        }

        isFighting = false;
    }

    /// <summary>
    /// 单个攻击阶段执行逻辑（玩家或敌人）
    /// </summary>
    private IEnumerator ExecuteAttackPhase(CharacterFight attacker, CharacterFight defender, bool isPlayer)
    {

        float damage = attacker.Attack(defender);

        // ✅ 若为玩家攻击，则额外伤害会自动在 BoosterEffect 中被触发
        // （不再手动计算或扣血，BoosterEffect 内部负责调用 CurrentEnemyFight.TakeDamage）

        string attackerName = isPlayer ? "玩家" : "敌人";
        AutoScrollLog.instance.AddLog($"{attackerName} 造成 {damage} 点伤害，HP：{defender.Stats.CurrentHP}/{defender.Stats.MaxHP}");

        // ✅ 若造成真实伤害，触发受击类 Booster
        if (damage > defender.Stats.Defense)
        {
            GameManager.Instance.TriggerBooster(BoosterTriggerTiming.OnTakeTrueDamage);
            GameManager.Instance.OnTakeTrueDamageEvent();
        }

        yield return new WaitForSeconds(0.8f);
        if(isPlayer)
        {
            GameManager.Instance.OnAttackEvent();
            GameManager.Instance.TriggerBooster(BoosterTriggerTiming.OnAttack);
        }
    }

    private void WinFight()
    {
        startButton.interactable = true;
        animator.SetTrigger("close");

        if (currentEnemy != null)
            Destroy(currentEnemy.gameObject);

        WinFightEvent.RaiseEvent(player, this);
        GameManager.Instance.EndBattle(true);
        StopAllCoroutines();
    }

    private void LoseFight()
    {
        startButton.interactable = true;
        animator.SetTrigger("close");

        GameManager.Instance.EndBattle(false);
        StopAllCoroutines();
    }
}
