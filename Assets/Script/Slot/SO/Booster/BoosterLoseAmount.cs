using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

[CreateAssetMenu(fileName = "New Booster", menuName = "Booster/Logic/LoseAmount")]

public class BoosterLoseAmount : BoosterLogicSO
{
    public CharacterStateType stateType;
    public override void BoosterAttack()
    {
        base.BoosterAttack();
        if (topicType == TopicType.All)
        {
            GameObject[] enemys = GameObject.FindGameObjectsWithTag("Enemy");
            switch (stateType)
            {
                case CharacterStateType.Fight:
                    foreach (var enemy in enemys)
                    {
                        Enemy obj = enemy.GetComponent<Enemy>();
                        obj.LoseFight(amount);
                    }
                    break;
                case CharacterStateType.Protect:
                    foreach (var enemy in enemys)
                    {
                        Enemy obj = enemy.GetComponent<Enemy>();
                        obj.LoseProtect(amount);
                    }
                    break;
                case CharacterStateType.HPCurrent:
                    foreach (var enemy in enemys)
                    {
                        Enemy obj = enemy.GetComponent<Enemy>();
                        obj.LoseHP(amount);
                    }
                    break;
            }
        }
    }

    public override void BoosterAfterWin()
    {
        base.BoosterAfterWin();
        Enemy highestEnemy = null;
        GameObject[] enemyObjs = GameObject.FindGameObjectsWithTag("Enemy");
        if (topicType == TopicType.HighestHP)
        {
            float maxHP = float.MinValue;
            foreach (var obj in enemyObjs)
            {
                Enemy enemy = obj.GetComponent<Enemy>();
                if (enemy.currentHP > maxHP)
                {
                    highestEnemy = enemy;
                    maxHP = enemy.currentHP;
                }
            }
        }
        if (topicType == TopicType.HighestFight)
        {
            float maxFight = float.MinValue;
            foreach (var obj in enemyObjs)
            {
                Enemy enemy = obj.GetComponent<Enemy>();
                if (enemy.fightAmount > maxFight)
                {
                    highestEnemy = enemy;
                    maxFight = enemy.fightAmount;
                }
            }
        }
        switch (stateType)
        {
            case CharacterStateType.Fight:
                if (highestEnemy != null) highestEnemy.LoseFight(amount);
                break;
            case CharacterStateType.Protect:
                if (highestEnemy != null) highestEnemy.LoseProtect(amount);
                break;
            case CharacterStateType.HPCurrent:
                if (highestEnemy != null) highestEnemy.LoseHP(amount);
                break;
        }
    }
}
