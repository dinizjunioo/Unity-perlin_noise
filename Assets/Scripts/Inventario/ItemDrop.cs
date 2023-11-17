using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemDrop : MonoBehaviour, IDropHandler
{
    public Inventory inventory;
    public void OnDrop(PointerEventData eventData)
    {
        RectTransform invPanel = transform as RectTransform;

        if (!RectTransformUtility.RectangleContainsScreenPoint(invPanel, Input.mousePosition))
        {
            Debug.Log("primeira parte -> drop!");
            IInventoryItem item = eventData.pointerDrag.gameObject.GetComponent<ItemDrag>().Item;
            if(item != null)
            {
                inventory.RemoveItem(item);           
                item.OnDrop();
            }
        }
    }

}
