using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextController : Interactable
{
    public override void Interact(Player player)
    {
        base.Interact(player);
        //清空当前角色状态
        GameManager.Instance.GetPlayer().runtimeData.ClearBaseStats();
        //等级+1，加载下层房间
        GameManager.Instance.SetCurrentLevel(GameManager.Instance.GetCurrentLevel() + 1);
        GameStateManager.Instance.SetState(GameState.LoadRoom);
    }
}
