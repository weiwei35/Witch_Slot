using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New Booster", menuName = "Booster/Logic/Common")]
public class BoosterLogicSO : ScriptableObject
{
    public float amount;
    public TempType tempType;
    public float keepTime;
    public TopicType topicType;
    public BoosterEffectType boosterEffectType;
    
    protected BoosterSymbolSO boosterSymbol;

    public virtual void BoosterAttack()
    {
        if(boosterEffectType != BoosterEffectType.Now) return;
        
        boosterSymbol.symbol.ActiveAnim();
    }

    public virtual void BoosterInFight()
    {
        if(boosterEffectType != BoosterEffectType.InFight) return;
        boosterSymbol.symbol.ActiveAnim();
    }
    
    public virtual void BoosterAfterWin()
    {
        if(boosterEffectType != BoosterEffectType.AfterWin) return;
        boosterSymbol.symbol.ActiveAnim();
    }

    public void SetSymbol(BoosterSymbolSO symbol)
    {
        boosterSymbol = symbol;
    }
}
