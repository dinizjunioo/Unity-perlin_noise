using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    private const int SLOTS = 7;
    private List<IInventoryItem> mItens = new List<IInventoryItem>();
    //private IList<InventorySlot> mItens = new List<InventorySlot>();
    public event EventHandler<InventoryEventArgs> ItemAdded;

    public event EventHandler<InventoryEventArgs> ItemRemoved;
    public void AddItem(IInventoryItem item)
    {
        if(mItens.Count < SLOTS)
        {
            Collider collider = (item as MonoBehaviour).GetComponent<Collider>();
            collider.enabled = false;
            mItens.Add(item);
            item.OnPickup();
            if(ItemAdded != null)
            {
                ItemAdded(this, new InventoryEventArgs(item));
            }
        }
    }

    public void RemoveItem(IInventoryItem item)
    {
        if (mItens.Contains(item))
        {
            mItens.Remove(item);
            item.OnDrop();

            Collider collider = (item as MonoBehaviour).GetComponent<Collider>();
            if(collider != null)
                collider.enabled = true;
            if (ItemRemoved != null)
            {
                ItemRemoved(this, new InventoryEventArgs(item));
            }
        }
    }

}
