using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class HUD : MonoBehaviour
{
    public Inventory inventory;
    public GameObject messagePanel;
    void Start()
    {
        inventory.ItemAdded += InventoryScript_ItemAdded;
        inventory.ItemRemoved += InventoryScript_ItemRemoved;
    }
    
    private void InventoryScript_ItemAdded(object sender, InventoryEventArgs e)
    {
        Transform inventoryPanel = transform.Find("InventoryPanel");
        Debug.Log("numero de slots -> " + inventoryPanel.childCount);
        foreach (Transform slot in inventoryPanel)
        {
            // indo até a borda
            //Debug.Log ("name -> " + slot.GetChild(0).GetChild(0).name);
            Transform imageTransform = slot.GetChild(0).GetChild(0);
            Image image = imageTransform.GetComponent<Image>();
            ItemDrag itemDrag = imageTransform.GetComponent<ItemDrag>();
            //aqui vamos procurar um slot vazio 
            if (!image.enabled) 
            {
                Debug.Log("entrei -> " + slot.GetChild(0).GetChild(0).name);
                image.enabled = true;
                image.sprite = e.Item.Image;

                itemDrag.Item = e.Item;

                break;

            }

            // ...
        }
    }

    private void InventoryScript_ItemRemoved(object sender, InventoryEventArgs e)
    {
        Transform inventoryPanel = transform.Find("InventoryPanel");
        Debug.Log("numero de slots -> " + inventoryPanel.childCount);
        foreach (Transform slot in inventoryPanel)
        {

            Transform imageTransform = slot.GetChild(0).GetChild(0);
            Image image = imageTransform.GetComponent<Image>();
            ItemDrag itemDrag = imageTransform.GetComponent<ItemDrag>();

            if (itemDrag != null && itemDrag.Item == e.Item)
            //if (itemDrag.Item.Equals(e.Item))
            {
                Debug.Log("entrei -> " + slot.GetChild(0).GetChild(0).name);
                image.enabled = false;
                image.sprite = null;
                itemDrag.Item = null;

                break;
            }

            // ...
        }
    }
    public void OpenMessagePanel(string text)
    {
        messagePanel.SetActive(true);
    }

    public void CloseMessagePanel()
    {
        messagePanel.SetActive(false);
    }
}
