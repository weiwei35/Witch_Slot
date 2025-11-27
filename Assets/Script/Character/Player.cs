using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Player : Character
{
    public CharacterDataSO baseData;
    
    public TMP_Text fightText;
    public TMP_Text protectText;
    public TMP_Text hpText;
    public Scrollbar hpSlider;

    private void Awake()
    {
        runtimeData = new CharacterRuntimeData(baseData);
        
        runtimeData.OnValueChanged += RefreshUI;
        GameManager.Instance.RegisterPlayer(this);

        RefreshUI();
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
        hpText.text = runtimeData.CurrentHP + " / " + runtimeData.MaxHP;
        hpSlider.size= runtimeData.CurrentHP/runtimeData.MaxHP;
    }

    public void OnBattleEnd()
    {
        GameStateManager.Instance.SetState(GameState.Walking);
    }
}