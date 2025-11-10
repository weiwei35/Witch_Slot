[System.Serializable]
public class SymbolEffectConfig
{
    public EffectType effectType;

    public float value = 0;                 // 数值（+2攻 / +1伤）
    public DamageElement element = DamageElement.None;

    public int durationBattles = 0;         // 持续 N 场战斗
    public int durationAttacks = 0;         // 持续 N 次攻击
    public int durationHitted = 0;         // 持续 N 次受击

    public bool removeAfterTrigger = false; // 是否触发一次就移除
    public TargetType target = TargetType.Player;
}