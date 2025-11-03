using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public enum TopicType
{
    None,
    Self,
    One,
    HighestHP,
    HighestFight,
    All
}

public enum FightType
{
    火焰,潮汐,雷电,自然
}
public enum CharacterStateType
{
    Fight,
    Protect,
    HPMax,
    HPCurrent
}
public enum BoosterEffectType
{
    Now,
    InFight,
    AfterWin
}

public enum GameState
{
    Slot,
    Walking,
    Fighting,
}

public enum TempType
{
    Time,
    GetDamage
}
public class TemporaryEffect
{
    public float Amount;
    public TempType Type;
    public int Round;
    public TemporaryEffect(float amount,TempType type, [CanBeNull]int time)
    {
        Amount = amount;
        Type = type;
        Round = time;
    }
}