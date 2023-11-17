using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInventoryItem
{
    string Name { get; }
    Sprite Image { get; }

    //InventorySlot Slot { get; set; }
    void OnPickup();
    void OnDrop();
}
public class InventoryItem : MonoBehaviour
{
}

public class InventoryEventArgs : EventArgs
{
    public InventoryEventArgs(IInventoryItem item)
    {
        Item = item;

    }

    public IInventoryItem Item;
}
