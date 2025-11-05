using UnityEngine;

[CreateAssetMenu(menuName = "Character/CharacterData", fileName = "Data")]
public class CharacterDataSO : ScriptableObject
{
    [Header("基础信息")]
    public string characterName;
    public Sprite icon;

    [Header("属性参数")]
    public float strength;
    public float defense;
    public float maxHP;
    public float currentHP;
}