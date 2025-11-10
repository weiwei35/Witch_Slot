using UnityEngine;

public abstract class Character : MonoBehaviour
{
    public CharacterRuntimeData runtimeData;

    public virtual void TakeDamage(float dmg, DamageElement element)
    {
        runtimeData.CurrentHP = Mathf.Max(0, runtimeData.CurrentHP - dmg);
        // notify UI, etc
    }

    public bool IsAlive()
    {
        return runtimeData.CurrentHP > 0;
    }
}