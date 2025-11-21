using UnityEngine;

public class Enemy : Character
{
    private void CheckDead()
    {
        if (!IsAlive())
        {
            Destroy(gameObject);
        }
    }

    public override void TakeDamage(float dmg, DamageElement element)
    {
        base.TakeDamage(dmg, element);
        CheckDead();
    }

    public void InitEnemy(CharacterDataSO baseData)
    {
        runtimeData = new CharacterRuntimeData(baseData);
        transform.localPosition = Vector3.zero;
        GetComponent<SpriteRenderer>().sprite = runtimeData.Icon;
        GameManager.Instance.RegisterEnemy(this);
        runtimeData.OnValueChanged += CheckDead;
    }

    private void OnDestroy()
    {
        runtimeData.OnValueChanged -= CheckDead;
        GameManager.Instance.UnregisterEnemy(this);
    }
}