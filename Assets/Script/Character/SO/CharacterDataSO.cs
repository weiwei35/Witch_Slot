using UnityEngine;

[CreateAssetMenu(fileName="CharacterData", menuName="GameData/CharacterData")]
public class CharacterDataSO : ScriptableObject
{
    public string ID;
    public string charactorName;
    public float strength;
    public float defense;
    public float maxHP;
    public Sprite icon;
}