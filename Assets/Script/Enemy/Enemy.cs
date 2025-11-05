using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("原始配置文件 (只读模板)")]
    public CharacterDataSO baseData;

    [HideInInspector] public CharacterDataSO runtimeData; // ✅ 独立副本

    private void Start()
    {
        // 在运行时复制 ScriptableObject 数据
        runtimeData = Instantiate(baseData);
        // ✅ 注册到 GameManager
        GameManager.Instance.RegisterEnemy(this);
    }

    public CharacterDataSO GetData()
    {
        return runtimeData;
    }
    
    private void OnDestroy()
    {
        // ✅ 敌人死亡或销毁时从管理器中移除
        GameManager.Instance.UnregisterEnemy(this);
    }
}
