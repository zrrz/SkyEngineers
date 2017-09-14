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
    Inventory inventory;
    Inventory equipment;

	// Use this for initialization
	void Start () {
        equipment.slots = EQUIPMENT_SIZE;
	}
	
//	// Update is called once per frame
//	void Update () {
//		
//	}

    public bool AddItem(int itemID, int amount = 1)
    {
        //Add to the equipment slot if it's empty or stackable

        Item data = ItemLoader.GetItem(itemID);
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
}
