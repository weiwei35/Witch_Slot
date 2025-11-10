using System;
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Data/Symbol")]
[Serializable]
public class SymbolSO : ScriptableObject
{
    public string symbolId;
    public string displayName;
    public Sprite icon;
    public SymbolCategory category;

    public List<TriggerEvent> triggers;

    public int interval;
    // public int durationBattles;
    // public int durationAttacks;

    public bool isConsumedAfterTrigger;//booster使用后消耗
    public bool needTrigger;

    public List<SymbolEffectConfig> effects;

    [TextArea]
    public string description;
}