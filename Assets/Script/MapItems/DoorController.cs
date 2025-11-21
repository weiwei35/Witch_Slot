using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : Interactable
{
    public Sprite openDoor;
    public Sprite closedDoor;

    private SpriteRenderer _sprite;
    private BoxCollider2D _collider2D;

    private void Awake()
    {
        _sprite = GetComponent<SpriteRenderer>();
        _collider2D = GetComponent<BoxCollider2D>();
    }

    public void InitDoor()
    {
        transform.localPosition = Vector3.zero;
        _sprite.sprite = closedDoor;
        _collider2D.enabled = true;
    }

    public override void Interact(Player player)
    {
        base.Interact(player);
        
        _sprite.sprite = openDoor;
        _collider2D.enabled = false;
    }
}
