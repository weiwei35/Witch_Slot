using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    private Player player;
    private List<Enemy> enemies = new List<Enemy>();
    private Enemy currentEnemy = null;

    private void Awake()
    {
        Instance = this;
    }
    public void RegisterPlayer(Player p)
    {
        player = p;
    }
    // ✅ 注册敌人
    public void RegisterEnemy(Enemy enemy)
    {
        if (!enemies.Contains(enemy))
            enemies.Add(enemy);
    }

    // ✅ 取消注册
    public void UnregisterEnemy(Enemy enemy)
    {
        if (enemies.Contains(enemy))
            enemies.Remove(enemy);
    }

    public IReadOnlyList<Enemy> GetEnemies()
    {
        return enemies;
    }

    public Player GetPlayer()
    {
        return player;
    }

    public Enemy GetCurrentEnemy()
    {
        return currentEnemy;
    }

    public Enemy GetHighHpEnemy()
    {
        Enemy e = null;
        foreach (var enemy in enemies)
        {
            if (e == null)
            {
                e = enemy;
            }
            if (enemy.runtimeData.CurrentHP > e.runtimeData.CurrentHP)
            {
                e = enemy;
            }
        }

        return e;
    }
    public Enemy GetHighAtkEnemy()
    {
        Enemy e = null;
        foreach (var enemy in enemies)
        {
            if(!enemy.IsAlive()) continue;
            if (e == null)
            {
                e = enemy;
            }
            if (enemy.runtimeData.Strength > e.runtimeData.Strength)
            {
                e = enemy;
            }
        }

        return e;
    }
    public void StartBattle(Player player, Enemy enemy)
    {
        currentEnemy = enemy;
        CombatSystem.Instance.StartCombat(player, enemy);
    }

    public void EndBattle()
    {
        currentEnemy = null;
    }
}