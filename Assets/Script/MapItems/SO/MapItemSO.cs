using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Map/Item")]
public class MapItemSO : ScriptableObject
{
    public string ID;
    public string Name;
    public EffectType Type;
    public int Value;
    public Sprite Icon;
}