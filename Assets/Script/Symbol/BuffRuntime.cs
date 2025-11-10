using System;

[Serializable]
public class BuffRuntime
{
    public EffectType effectType;
    public float value;

    public int remainBattles;   // 剩余战斗次数
    public int remainAttacks;   // 剩余攻击次数
    public int remainHitted;    // 剩余受击次数

    public bool removeAfterTrigger;   // 触发一次即消失
    public bool needCountBattle;
    public bool needCountAttack;
    public bool needCountHitted;

    public BuffRuntime(SymbolEffectConfig cfg)
    {
        effectType = cfg.effectType;
        value = cfg.value;

        remainBattles = cfg.durationBattles;
        remainAttacks = cfg.durationAttacks;
        remainHitted = cfg.durationHitted;

        if (remainBattles > 0 || remainAttacks > 0 || remainHitted > 0)
        {
            removeAfterTrigger = false;
            if (remainAttacks > 0)
            {
                needCountBattle  = false;
                needCountAttack = true;
                needCountHitted = false;
            }
            else if(remainBattles > 0)
            {
                needCountBattle  = true;
                needCountAttack = false;
                needCountHitted = false;
            }
            else if (remainHitted > 0)
            {
                needCountBattle  = false;
                needCountAttack = false;
                needCountHitted = true;
            }
        }
        else
        {
            removeAfterTrigger = true;
        }
    }
}