using UnityEngine;

public abstract class BaseSymbolSO : ScriptableObject
{
    [Header("通用信息")]
    public string symbolName;
    [TextArea] public string symbolDesc;
    public Sprite symbolSprite;
}