using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemController : Interactable
{
    private MapItemSO itemData;
    public void Init(MapItemSO item)
    {
        GetComponent<SpriteRenderer>().sprite = item.Icon;
        itemData = item;
    }
    public override void Interact(Player player)
    {
        base.Interact(player);
        switch (itemData.Type)
        {
            case EffectType.ModifyHP:
                player.AddHP(itemData.Value);
                break;
        }
        
        Destroy(gameObject);
    }
}
