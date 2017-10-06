using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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
	public const int INVENTORY_SIZE = 40; //Should correspond to slot size
	public const int CRAFTING_SIZE = 4;
    public Inventory inventory;
    public Inventory equipment;

    public int currentActiveSlot = 0;

    public Item CurrentActiveItem {
        get { return inventory.items[currentActiveSlot]; }
    }

    public UnityEvent selectionChangedEvent = new UnityEvent();

	void Awake () {
		equipment = new Inventory();
        equipment.slots = EQUIPMENT_SIZE;
		equipment.items = new Item[EQUIPMENT_SIZE];
		inventory = new Inventory();
		inventory.slots = INVENTORY_SIZE;
		inventory.items = new Item[INVENTORY_SIZE + CRAFTING_SIZE];

        if (grabbedItems == null)
        {
            grabbedItems = new List<ItemPickup>();
        }

		RecipeManager.AddShapedRecipe(new int[] {0, -1, 0, 0}, ItemLoader.GetItemData(1));
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

    public bool ConsumeCurrentItem() {
        return inventory.RemoveItemAt(currentActiveSlot, 1);
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

	public void CraftItem() {
		inventory.RemoveItemAt(40, 1);
		inventory.RemoveItemAt(41, 1);
		inventory.RemoveItemAt(42, 1);
		inventory.RemoveItemAt(43, 1);
	}

	int GetInventoryItemID(int slot) {
		if(inventory.items[slot] != null)
			return inventory.items[slot].ID;
		else
			return -1;
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
			
		Item craftedItem = null;
		if(RecipeManager.CheckRecipe(new int[] {GetInventoryItemID(40), GetInventoryItemID(41), GetInventoryItemID(42), GetInventoryItemID(43)}, out craftedItem)) {
			GameObject.Find("44").GetComponent<UnityEngine.UI.Image>().sprite = craftedItem.sprite;
			GameObject.Find("44").transform.Find("Amount").GetComponent<UnityEngine.UI.Text>().text = craftedItem.amount.ToString();
			GameObject.Find("44").transform.Find("Amount").gameObject.SetActive(true);
		} else {
			GameObject.Find("Canvas/Canvas/Inventory/Bag/Slots/44").GetComponent<UnityEngine.UI.Image>().sprite = null;
			GameObject.Find("44").transform.Find("Amount").gameObject.SetActive(false);
		}

        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            currentActiveSlot--;
            if (currentActiveSlot < 0)
                currentActiveSlot = 9;
//            inventoryDirty = true;
            selectionChangedEvent.Invoke();
        }
        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            currentActiveSlot++;
            if (currentActiveSlot > 9)
                currentActiveSlot = 0;
//            inventoryDirty = true;
            selectionChangedEvent.Invoke();
        }
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            currentActiveSlot = 9;
            selectionChangedEvent.Invoke();
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            currentActiveSlot = 0;
            selectionChangedEvent.Invoke();
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            currentActiveSlot = 1;
            selectionChangedEvent.Invoke();
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            currentActiveSlot = 2;
            selectionChangedEvent.Invoke();
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            currentActiveSlot = 3;
            selectionChangedEvent.Invoke();
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            currentActiveSlot = 4;
            selectionChangedEvent.Invoke();
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            currentActiveSlot = 5;
            selectionChangedEvent.Invoke();
        }
        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            currentActiveSlot = 6;
            selectionChangedEvent.Invoke();
        }
        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            currentActiveSlot = 7;
            selectionChangedEvent.Invoke();
        }
        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            currentActiveSlot = 8;
            selectionChangedEvent.Invoke();
        }
//        
//        if (Input.GetButtonDown("Fire1"))
//        {
//            if(playerInventory.inventory.items[currentActiveSlot] != null && playerInventory.inventory.items[currentActiveSlot].placeable)
//                playerInventory.inventory.items[currentActiveSlot].PlaceBlock();
//        }
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
