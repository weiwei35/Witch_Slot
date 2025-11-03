using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Player : Character
{
    public ObjectEventSO StartFightEvent_player;
    public ObjectEventSO StartFightEvent_enemy;
    public ObjectEventSO AddFakeFightAmountEvent_player;
    public ObjectEventSO AddFakeProtectAmountEvent_player;
    
    public TMP_Text fightText;
    public TMP_Text protectText;

    private float fightResetCounter = 0;
    private float fightTemp = 0;
    
    private float protectResetCounter = 0;
    private float protectTemp = 0;
    
    private TemporaryEffect temporaryAttackEffect = null;
    private TemporaryEffect temporaryProtectEffect = null;
    public void SetFightAmount(object o)
    {
        float amount = (float)o;
        fightAmount += amount;
        fightText.text = fightAmount.ToString();
    }

    public void SetProtectAmount(object o)
    {
        float amount = (float)o;
        protectAmount += amount;
        protectText.text = protectAmount.ToString();
    }

    public void AddHPCurrent(float amount)
    {
        currentHP += amount;
        if(currentHP > maxHP) currentHP = maxHP;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            GameManager.instance.gameState = GameState.Fighting;
            StartFightEvent_player.RaiseEvent(this,this);
            if (temporaryAttackEffect != null) AddFakeFightAmountEvent_player.RaiseEvent(temporaryAttackEffect, this);
            if (temporaryProtectEffect != null) AddFakeProtectAmountEvent_player.RaiseEvent(temporaryProtectEffect, this);
            Enemy enemy = other.GetComponent<Enemy>();
            StartFightEvent_enemy.RaiseEvent(enemy,this);
        }
    }

    public void AddFightAmount(float amount, float time)
    {
        fightTemp += amount;
        fightAmount += amount;
        fightResetCounter += time;
        fightText.text = fightAmount.ToString();
    }
    public void AddProtectAmount(float amount, float time)
    {
        protectTemp += amount;
        protectAmount += amount;
        protectResetCounter += time;
        protectText.text = protectAmount.ToString();
    }

    public void AddFightAmountFake(float amount,TempType type, int time)
    {
        temporaryAttackEffect = new TemporaryEffect(amount, type,time);
    }
    public void AddProtectAmountFake(float amount,TempType type, int time)
    {
        temporaryProtectEffect = new TemporaryEffect(amount, type,time);
    }

    public void AfterFight(object o)
    {
        GameManager.instance.gameState = GameState.Walking;
        if(fightResetCounter > 0)
            fightResetCounter--;
        if (fightResetCounter <= 0)
        {
            fightAmount -= fightTemp;
            fightTemp = 0;
            fightText.text = fightAmount.ToString();
            fightResetCounter = 0;
        }
        if(protectResetCounter > 0)
            protectResetCounter--;
        if (protectResetCounter <= 0)
        {
            protectAmount -= protectTemp;
            protectTemp = 0;
            protectText.text = protectAmount.ToString();
            protectResetCounter = 0;
        }
    }

    public void AfterRound(object o)
    {
        PlayerFight playerFight = o as PlayerFight;
        if (playerFight != null) currentHP = playerFight.CurrentHP;
    }
}