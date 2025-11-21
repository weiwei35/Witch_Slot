using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestController : Interactable
{
    public void Init(MapChestSO item)
    {
        GetComponent<SpriteRenderer>().sprite = item.Icon;
    }
    public override void Interact(Player player)
    {
        base.Interact(player);
        //TODO:拾取宝箱
    }
}
