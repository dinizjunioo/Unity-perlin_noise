
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Progress;

public class Player : MonoBehaviour
{
#pragma warning disable CS0108 // O membro oculta o membro herdado; nova palavra-chave ausente
     Rigidbody rigidbody;
#pragma warning restore CS0108 // O membro oculta o membro herdado; nova palavra-chave ausente
     Vector3 velocity;

     public HUD hud;
     public Inventory inventory;

    private IInventoryItem mItemToPickup = null;
     void Start()
     {
        rigidbody = GetComponent<Rigidbody>();
     }

     void Update()
     {
        velocity = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized * 10;
        
        if(mItemToPickup != null && Input.GetKeyDown(KeyCode.F))
        {
            inventory.AddItem(mItemToPickup);
            mItemToPickup.OnPickup();
            hud.CloseMessagePanel();
        }
    
    }

     void FixedUpdate()
     {
        rigidbody.MovePosition(rigidbody.position + velocity * Time.fixedDeltaTime);
     }

    //private void OnCollisionEnter(Collision collision)
    //{
    //    IInventoryItem item = collision.collider.GetComponent<IInventoryItem>();
        
    //    if (item != null)
    //    {
    //        Debug.Log("collision ->" + item.Name);
    //        inventory.AddItem(item);
    //    }
    //}

    private void OnTriggerEnter(Collider other)
    {
        IInventoryItem item = other.GetComponent<IInventoryItem>();
        if (item != null)
        {
            //Debug.Log("Hit ->" + item.Name);

            //inventory.AddItem(item);
            mItemToPickup = item;
            hud.OpenMessagePanel("");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        IInventoryItem item = other.GetComponent<IInventoryItem>();

        if(item != null)
        {
            hud.CloseMessagePanel();
            mItemToPickup = null;
        }


    }
}
