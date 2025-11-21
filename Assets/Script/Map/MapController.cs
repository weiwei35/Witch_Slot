using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 地图管理：level记录--加载房间
/// </summary>
public class MapController : MonoBehaviour
{
    private int level;
    public MapContentData mapData;
    // public RoomController room2Load;//测试加载房间
    private RoomController currentRoom;

    private void Awake()
    {
        GameStateManager.Instance.OnGameStateChanged += CheckLoadRoom;
    }

    private void OnDestroy()
    {
        GameStateManager.Instance.OnGameStateChanged -= CheckLoadRoom;
    }

    private void CheckLoadRoom(GameState arg1, GameState state)
    {
        if (state == GameState.LoadRoom)
        {
            LoadRoom();
        }
    }

    public void LoadRoom()
    {
        if (currentRoom != null)
        {
            Destroy(currentRoom.gameObject);
        }
        level = GameManager.Instance.GetCurrentLevel();
        //根据等级加载房间
        LevelRoomPrefab levelRoom = null;
        foreach (var room in mapData.rooms)
        {
            if(room.level == level) levelRoom = room;
        }
        //加载房间，设置player初始位置
        if (levelRoom != null)
        {
            currentRoom = Instantiate(levelRoom.room, transform);
            Player player = GameManager.Instance.GetPlayer();
            player.transform.position = currentRoom.InitRoom(levelRoom.data);
        }
    }
}
