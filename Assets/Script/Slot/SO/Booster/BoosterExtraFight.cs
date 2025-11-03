using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Booster", menuName = "Booster/Logic/ExtraFight")]
public class BoosterExtraFight : BoosterLogicSO
{
    public int fightCount;
    public float extraAmount;
    public FightType fightType;

    public ObjectEventSO ExtraFightAddEvent;
    public override void BoosterInFight()
    {
        base.BoosterInFight();
        ExtraFightClass extra = new ExtraFightClass
        {
            count = fightCount,
            fight = extraAmount,
            fightType = fightType,
            symbol = boosterSymbol.symbol
        };
        ExtraFightAddEvent.RaiseEvent(extra,this);
    }
}
