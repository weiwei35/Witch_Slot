using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerFight : CharacterFight
{
    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);
        if (temporaryDefenseEffect != null && temporaryDefenseEffect.Type == TempType.GetDamage)
        {
            ClearTemporaryDefence();
        }
    }
}
