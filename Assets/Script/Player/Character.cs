using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public float maxHP;
    public float currentHP;
    public float fightAmount;
    public float protectAmount;

    public void Die()
    {
        Destroy(gameObject);
    }

    public void LoseFight(float amount)
    {
        fightAmount -= amount;
        if (fightAmount < 0)
        {
            fightAmount = 0;
        }
    }
    public void LoseProtect(float amount)
    {
        protectAmount -= amount;
        if (protectAmount < 0)
        {
            protectAmount = 0;
        }
    }

    public void LoseHP(float amount)
    {
        currentHP -= amount;
        if (currentHP < 0)
        {
            currentHP = 0;
            Die();
        }
    }
}
