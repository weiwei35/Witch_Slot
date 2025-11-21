using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapEnemyController : Interactable
{
    public override void Interact(Player player)
    {
        base.Interact(player);
        GameStateManager.Instance.SetState(GameState.Fighting);
        GameManager.Instance.StartBattle(player, GetComponent<Enemy>());
    }
}
