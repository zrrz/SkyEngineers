using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour {

    enum Slot {
        None = 0,
        Head = 1,
        Chest = 2,
        Legs = 3,
        Boots = 4,
        Gloves = 5,
        Jewelry = 6,
        //        Offhand = 6,
        //Make sure to change EQUIPMENT_SIZE
    }

    public const int EQUIPMENT_SIZE = 6; //Should correspond to slot size
	public const int INVENTORY_SIZE = 64; //Should correspond to slot size
    public Inventory inventory;
    public Inventory equipment;

	void Awake () {
		equipment = new Inventory();
        equipment.slots = EQUIPMENT_SIZE;
		equipment.items = new Item[EQUIPMENT_SIZE];
		inventory = new Inventory();
		inventory.slots = INVENTORY_SIZE;
		inventory.items = new Item[INVENTORY_SIZE];

        if (grabbedItems == null)
        {
            grabbedItems = new List<ItemPickup>();
        }
	}

    public bool AddItem(int itemID, int amount = 1)
    {
        //Add to the equipment slot if it's empty or stackable

        Item data = ItemLoader.GetItemData(itemID);
        if (data.slot != Item.EquipSlot.None)
        {
            if (equipment.GetItemAt((int)data.slot) == null || equipment.GetItemAt((int)data.slot).ID == itemID)
            {
                if (equipment.AddItemAtIndex(itemID, amount, (int)data.slot) == false)
                {
                    return true;
                }
            }
        }
        return inventory.AddItem(itemID, amount);
    }


    List<ItemPickup> grabbedItems;

    void OnTriggerEnter(Collider col) {
        ItemPickup item = col.gameObject.GetComponent<ItemPickup>();
        if (item != null)
        {
//            if (!grabbedItems.Contains(item))
//            {
            item.GetComponent<Collider>().enabled = false;
            Destroy(item.GetComponent<Rigidbody>());//.enabled = false;
                grabbedItems.Add(item);
//            }
        }
    }

    void Update() {
        for (int i = 0; i < grabbedItems.Count; i++)
        {
            grabbedItems[i].transform.position = Vector3.MoveTowards(grabbedItems[i].transform.position, transform.position, 4f * Time.deltaTime);
            if (Vector3.Distance(grabbedItems[i].transform.position, transform.position) < 0.5f)
            {
                ItemPickup item = grabbedItems[i];
                if (AddItem(item.itemID, item.amount))
                {
                    grabbedItems.RemoveAt(i);
                    i--;
                    Destroy(item.gameObject);
                }
                else
                {
                    Debug.LogError("Can't add to inventory");
                }
            }
        }
    }

//	void OnControllerColliderHit(ControllerColliderHit hit) {
////		Rigidbody body = hit.collider.attachedRigidbody;
////		if (body == null || body.isKinematic)
////			return;
////
////		if (hit.moveDirection.y < -0.3F)
////			return;
////
////		Vector3 pushDir = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z);
////		body.velocity = pushDir * pushPower;
//		//TODO cleanup
//        ItemPickup item = hit.gameObject.GetComponent<ItemPickup>();
//        if(item != null) {
//            AddItem(item.itemID, item.itemID);
//			Destroy(hit.gameObject);
//		}
//	}
}
