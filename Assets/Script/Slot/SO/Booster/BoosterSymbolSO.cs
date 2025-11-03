using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New Booster", menuName = "Booster/Symbol")]
public class BoosterSymbolSO : SymbolSO
{
    public List<BoosterLogicSO> effects;

    private void OnValidate()
    {
        foreach (var effect in effects)
        {
            if (effect != null) effect.SetSymbol(this);
        }
    }
}
