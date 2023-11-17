using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemClick : MonoBehaviour
{
    public void OnItemClicked()
    {
        ItemDrag itemDrag = gameObject.transform.Find("ItemImage").GetComponent<ItemDrag>();
        IInventoryItem item = itemDrag.Item;
        if(item != null)
            Debug.Log(item.Name);
    }
}
