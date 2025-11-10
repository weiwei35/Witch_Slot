using UnityEngine;

public class CombatRuntime
{
    public CharacterRuntimeData Player;
    public CharacterRuntimeData Enemy;

    public CombatRuntime(CharacterRuntimeData p, CharacterRuntimeData e)
    {
        Player = p.Clone();
        Enemy  = e.Clone();
    }
}