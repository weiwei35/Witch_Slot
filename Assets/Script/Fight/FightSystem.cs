using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FightSystem : MonoBehaviour
{
    // [SerializeField] private TMP_Text battleLog;
    [SerializeField] private Button startButton;
    public GameObject content;
    public Animator animator;
    public CharacterFight player;
    public CharacterFight enemy;

    private Enemy currentEnemy;

    public ObjectEventSO WinFightEvent;
    public ObjectEventSO FightRoundEvent;
    private void Start()
    {
        startButton.onClick.AddListener(StartBattle);
    }

    public void SetFightState_player(object o)
    {
        Player playerState = (Player)o;
        Sprite icon = playerState.GetComponent<SpriteRenderer>().sprite;
        player.Initialize(playerState.fightAmount,playerState.protectAmount,playerState.maxHP,playerState.currentHP,icon);
    }
    public void SetFightState_enemy(object o)
    {
        Enemy enemyState = (Enemy)o;
        currentEnemy = enemyState;
        Sprite icon = enemyState.GetComponent<SpriteRenderer>().sprite;
        enemy.Initialize(enemyState.fightAmount,enemyState.protectAmount,enemyState.maxHP,enemyState.currentHP,icon);
    }
    public void ApplyTemporaryAttack_player(object o)
    {
        TemporaryEffect temp = (TemporaryEffect)o;
        player.ApplyTemporaryAttack(temp);
    }
    public void ApplyTemporaryProtect_player(object o)
    {
        TemporaryEffect temp = (TemporaryEffect)o;
        player.ApplyTemporaryProtect(temp);
    }
    private void StartBattle()
    {
        startButton.interactable = false;
        AutoScrollLog.instance.AddLog("战斗开始");
        fightCount = 0;
        StartCoroutine(BattleCoroutine());
    }
    private IEnumerator BattleCoroutine()
    {
        while (player.IsAlive && enemy.IsAlive)
        {
            // 玩家攻击
            float playerDamage = player.Attack(enemy);
            AutoScrollLog.instance.AddLog($"玩家造成 {playerDamage} 点伤害，敌人生存 {enemy.CurrentHP}/{enemy.MaxHP}");
            yield return StartCoroutine(ExtraFight());
            yield return new WaitForSeconds(1);
            if (!enemy.IsAlive) break;
            // 敌人攻击
            float enemyDamage = enemy.Attack(player);
            AutoScrollLog.instance.AddLog($"敌人造成 {enemyDamage} 点伤害，玩家生存 {player.CurrentHP}/{player.MaxHP}");
            yield return new WaitForSeconds(1f);
            FightRoundEvent.RaiseEvent(player,this);
        }
        if (player.IsAlive)
        {
            AutoScrollLog.instance.AddLog("战斗胜利!");
            // 触发胜利逻辑
            WinFight();
        }
        else
        {
            AutoScrollLog.instance.AddLog("战斗失败!");
            // 触发失败逻辑
        }
    }

    public void WinFight()
    {
        startButton.interactable = true;
        animator.SetTrigger("close");
        // content.SetActive(false);
        currentEnemy.Die();
        WinFightEvent.RaiseEvent(player,this);
        StopAllCoroutines();
    }

    public List<ExtraFightClass> extraFightList = new List<ExtraFightClass>();
    private int fightCount = 0;

    public void SetExtaList(object o)
    {
        ExtraFightClass extraFightClass = (ExtraFightClass)o;
        extraFightList.Add(extraFightClass);
    }
    public IEnumerator ExtraFight()
    {
        fightCount++;
        bool needReset = false;
        foreach (var extraFight in extraFightList)
        {
            if (extraFight.count == fightCount)
            {
                needReset = true;
                yield return new WaitForSeconds(1f);
                extraFight.symbol.ActiveAnim();
                float playerDamage = player.ExtraAttack(enemy,extraFight.fight);
                AutoScrollLog.instance.AddLog($"玩家额外造成 {playerDamage} 点{extraFight.fightType}类型伤害，敌人生存 {enemy.CurrentHP}/{enemy.MaxHP}");
                
                if (!enemy.IsAlive)
                {
                    AutoScrollLog.instance.AddLog("战斗胜利!");
                    // 触发胜利逻辑
                    WinFight();
                }
            }
        }
        if(needReset) fightCount = 0;
    }
}

public class ExtraFightClass
{
    public int count;
    public float fight;
    public FightType fightType;
    public Symbol symbol;
}