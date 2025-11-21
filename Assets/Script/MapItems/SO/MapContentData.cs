using System;
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "MapContent", menuName = "Map/MapContent")]
public class MapContentData : ScriptableObject
{
    public List<LevelRoomPrefab> rooms = new List<LevelRoomPrefab>();
}
[Serializable]
public class LevelRoomPrefab
{
    public int level;
    public RoomContentData data;
    public RoomController room;
}