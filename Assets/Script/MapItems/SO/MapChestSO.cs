using UnityEngine;

[CreateAssetMenu(fileName = "Chest", menuName = "Map/Chest")]
public class MapChestSO : ScriptableObject
{
    public string ID;
    public string Name;
    public Sprite Icon;
}