using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("原始配置文件 (只读模板)")]
    public CharacterDataSO baseData;

    [HideInInspector] public CharacterDataSO runtimeData; // ✅ 独立副本

    private void Awake()
    {
        // 在运行时复制 ScriptableObject 数据
        runtimeData = Instantiate(baseData);
    }

    public CharacterDataSO GetData()
    {
        return runtimeData;
    }
    public PlayerEventSO StartFightEvent_player;
    public EnemyEventSO StartFightEvent_enemy;
    public TemporaryEffectEventSO AddFakeFightAmountEvent_player;
    public TemporaryEffectEventSO AddFakeProtectAmountEvent_player;
    
    public TMP_Text fightText;
    public TMP_Text protectText;

    private float fightResetCounter = 0;
    private float fightTemp = 0;
    
    private float protectResetCounter = 0;
    private float protectTemp = 0;
    
    private TemporaryEffect temporaryAttackEffect = null;
    private TemporaryEffect temporaryProtectEffect = null;
    public void _SetFightAmount(float o)
    {
        GetData().strength += o;
        fightText.text = GetData().strength.ToString();
    }
    public void _SetProtectAmount(float o)
    {
        GetData().defense += o;
        protectText.text = GetData().defense.ToString();
    }

    private void Update()
    {
        fightText.text = GetData().strength.ToString();
        protectText.text = GetData().defense.ToString();
    }

    public void AddHPCurrent(float amount)
    {
        GetData().currentHP += amount;
        if(GetData().currentHP > GetData().maxHP) GetData().currentHP = GetData().maxHP;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            GameStateManager.Instance.SetState(GameState.Fighting);
            // StartFightEvent_player.RaiseEvent(this,this);
            if (temporaryAttackEffect != null) AddFakeFightAmountEvent_player.RaiseEvent(temporaryAttackEffect, this);
            if (temporaryProtectEffect != null) AddFakeProtectAmountEvent_player.RaiseEvent(temporaryProtectEffect, this);
            Enemy enemy = other.GetComponent<Enemy>();
            StartFightEvent_enemy.RaiseEvent(enemy,this);
            GameManager.Instance.StartBattle(this, enemy);
        }
    }

    public void AddFightAmount(float amount, float time)
    {
        fightTemp += amount;
        GetData().strength += amount;
        fightResetCounter += time;
        fightText.text = GetData().strength.ToString();
    }
    public void AddProtectAmount(float amount, float time)
    {
        protectTemp += amount;
        GetData().defense += amount;
        protectResetCounter += time;
        protectText.text = GetData().defense.ToString();
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
        GameStateManager.Instance.SetState(GameState.Walking);
        if(fightResetCounter > 0)
            fightResetCounter--;
        if (fightResetCounter <= 0)
        {
            GetData().strength -= fightTemp;
            fightTemp = 0;
            fightText.text = GetData().strength.ToString();
            fightResetCounter = 0;
        }
        if(protectResetCounter > 0)
            protectResetCounter--;
        if (protectResetCounter <= 0)
        {
            GetData().defense -= protectTemp;
            protectTemp = 0;
            protectText.text = GetData().defense.ToString();
            protectResetCounter = 0;
        }
    }
    public void _AfterRound(CharacterFight o)
    {
        if (o != null) GetData().currentHP = o.Stats.CurrentHP;
    }
}