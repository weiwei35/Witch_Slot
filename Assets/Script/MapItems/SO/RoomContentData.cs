using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "RoomContent", menuName = "Map/RoomContent")]
public class RoomContentData : ScriptableObject
{
    public string Level;
    public string RoomName;

    public List<EnemySpawn> Enemies = new();
    public List<ItemSpawn> Items = new();
    public List<ChestSpawn> Chests = new();
}

[System.Serializable]
public class EnemySpawn
{
    public CharacterDataSO Data;
    public float PosIndex;
}

[System.Serializable]
public class ItemSpawn
{
    public MapItemSO Data;
    public float PosIndex;
}

[System.Serializable]
public class ChestSpawn
{
    public MapChestSO Data;
    public float PosIndex;
}