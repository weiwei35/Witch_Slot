using UnityEngine;

public class Enemy : Character
{
    public CharacterDataSO baseData;

    private void Awake()
    {
        runtimeData = new CharacterRuntimeData(baseData);
        GameManager.Instance.RegisterEnemy(this);
    }

    private void OnDestroy()
    {
        GameManager.Instance.UnregisterEnemy(this);
    }
}