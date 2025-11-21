using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 初始化房间：出生位置--房门设置--怪物设置--道具设置
/// </summary>
public class RoomController : MonoBehaviour
{
    public RoomContentData roomData;
    public Transform cameraPos;
    public Transform startPos_player;
    public Transform doorListPos;
    public Transform endPos_player;
    public Transform enemyListPos;
    public Transform itemListPos;
    public Transform chestListPos;

    public DoorController doorPrefab;
    public Enemy enemyPrefab;
    public ItemController itemPrefab;
    public ChestController chestPrefab;
    public NextController nextPrefab;
    public Vector3 InitRoom(RoomContentData data)
    {
        roomData = data;
        Camera.main.transform.position = new Vector3(cameraPos.position.x, cameraPos.position.y, Camera.main.transform.position.z);
        //设置房门
        foreach (Transform doorPos in doorListPos)
        {
            var door = Instantiate(doorPrefab, doorPos);
            door.InitDoor();
        }
        //设置怪物
        foreach (var enemy in roomData.Enemies)
        {
            var enemyObj = Instantiate(enemyPrefab,enemyListPos.GetChild((int)enemy.PosIndex));
            enemyObj.InitEnemy(enemy.Data);
        }
        //设置道具
        foreach (var item in roomData.Items)
        {
            var itemObj = Instantiate(itemPrefab,itemListPos.GetChild((int)item.PosIndex));
            itemObj.Init(item.Data);
        }
        //设置宝箱
        foreach (var chest in roomData.Chests)
        {
            var chestObj = Instantiate(chestPrefab,chestListPos.GetChild((int)chest.PosIndex));
            chestObj.Init(chest.Data);
        }
        //设置楼梯
        //房间加载完毕
        GameStateManager.Instance.SetState(GameState.Slot);
        //返回角色出生点位
        return startPos_player.position;
    }

    private void Start()
    {
        GameManager.Instance.OnEnemyDead += CheckEnemyAllDead;
    }

    private void OnDestroy()
    {
        GameManager.Instance.OnEnemyDead -= CheckEnemyAllDead;
    }

    private void CheckEnemyAllDead()
    {
        if (GameManager.Instance.GetEnemies().Count == 0)
        {
            //加载楼梯
            var next = Instantiate(nextPrefab, endPos_player);
            next.transform.localPosition = Vector3.zero;
        }
    }
}
