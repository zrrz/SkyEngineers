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
    Inventory inventory;
    Inventory equipment;

	// Use this for initialization
	void Start () {
		equipment = new Inventory();
        equipment.slots = EQUIPMENT_SIZE;
		inventory = new Inventory();
		inventory.slots = INVENTORY_SIZE;
	}
	
//	// Update is called once per frame
//	void Update () {
//		
//	}

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

	void OnControllerColliderHit(ControllerColliderHit hit) {
		Debug.LogError(hit.gameObject.name);
//		Rigidbody body = hit.collider.attachedRigidbody;
//		if (body == null || body.isKinematic)
//			return;
//
//		if (hit.moveDirection.y < -0.3F)
//			return;
//
//		Vector3 pushDir = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z);
//		body.velocity = pushDir * pushPower;
		//TODO cleanup
		if(hit.gameObject.GetComponent<ItemPickup>()) {
			AddItem(hit.gameObject.GetComponent<ItemPickup>().itemID, hit.gameObject.GetComponent<ItemPickup>().itemID);
			Destroy(hit.gameObject);
		}
	}
}
