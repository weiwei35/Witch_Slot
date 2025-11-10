using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Player : Character
{
    public CharacterDataSO baseData;
    
    public TMP_Text fightText;
    public TMP_Text protectText;

    private void Awake()
    {
        runtimeData = new CharacterRuntimeData(baseData);
        
        runtimeData.OnValueChanged += RefreshUI;
        GameManager.Instance.RegisterPlayer(this);
    }

    private void OnDestroy()
    {
        runtimeData.OnValueChanged -= RefreshUI;
    }

    public void AddStrength(float v)
    {
        runtimeData.AddBaseStrength(v);
    }
    public void AddDefense(float v)
    {
        runtimeData.AddBaseDefense(v);
    }
    public void AddHP(float v)
    {
        runtimeData.AddHP(v);
    }

    public void RefreshUI()
    {
        fightText.text = runtimeData.Strength.ToString();
        protectText.text = runtimeData.Defense.ToString();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            GameStateManager.Instance.SetState(GameState.Fighting);
            Enemy enemy = other.GetComponent<Enemy>();
            GameManager.Instance.StartBattle(this, enemy);
        }
    }

    public void OnBattleEnd()
    {
        GameStateManager.Instance.SetState(GameState.Walking);
    }
}