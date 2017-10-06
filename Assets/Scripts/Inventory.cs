using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Inventory {

    public int slots = 64;

    public Item[] items;

    public UnityEvent inventoryChangedEvent = new UnityEvent();

    public virtual bool AddItem(int itemID, int amount = 1) {
		if(items == null) {
			Debug.LogError("Inventory is null");
			return false;
		}

        for (int i = 0; i < slots; i++)
        {
            if (items[i] == null)
                continue;
            if (items[i].ID == itemID)
            {
                if (items[i].amount < items[i].stackSize)
                {
                    items[i].amount++;
                    inventoryChangedEvent.Invoke();
					return true;
                }
//                else
//                {
//                    AddItemAtFirstEmptySlot(itemID, amount);
//					return true;
//                }
            }
        }

        //Otherwise just make a new one
        AddItemAtFirstEmptySlot(itemID, amount);
        inventoryChangedEvent.Invoke();
        return true;
    }

    public bool AddItemAtIndex(int itemID, int amount, int index) {
        if(items[index] == null) {
            items[index] = ItemLoader.CreateItem(itemID);
            items[index].amount = amount;
        } 
        else if (items[index] != null && items[index].ID == itemID && items[index].amount + amount < items[index].stackSize) 
        {
            items[index].amount += amount;
        }
        else
        {
            Debug.LogError("Can't add item because slot not empty, wrong itemID, or already full");
            return false;
        }

        inventoryChangedEvent.Invoke();
        return true;
    }

    //TODO maybe remove the itemID validation
    public bool RemoveItemAt(/*int itemID,*/ int index, int amount) {
        if (items[index] == null)
        {
//            Debug.LogError("Trying to remove item from empty slot");
            return false;
        }
        /*if (items[index].ID != itemID)
        {
            Debug.LogError("Trying to remove item using wrong itemID");
            return false;
        }*/ //Prob don't need for now
        if (items[index].amount > amount)
        {
            items[index].amount -= amount;
        }
        else if (items[index].amount == amount)
        {
            items[index] = null;
        }
        else
        {
            return false;
        }

        inventoryChangedEvent.Invoke();
        return true;
    }

    bool AddItemAtFirstEmptySlot(int itemID, int amount) {
        int slot = FindFirstEmptySlotIndex();
        if (slot == -1)
            return false;

        return AddItemAtIndex(itemID, amount, slot);
//        return true;
    }

    int FindFirstEmptySlotIndex() {
        for (int i = 0; i < slots; i++)
        {
            if (items[i] == null)
            {
                return i;
            }
        }
        return -1;
    }

    public Item GetItemAt(int index) {
        return items[index];
    }
}