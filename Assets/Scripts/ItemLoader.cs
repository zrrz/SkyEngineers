using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemLoader : MonoBehaviour {

    [SerializeField]
    Item[] itemArray;

    Dictionary<int, Item> items;

    static ItemLoader instance;

    void Awake() {
        if(instance != null) {
            Debug.LogError("Already a Item in scene. Disabling", this);
            this.enabled = false;
            return;
        }
        instance = this;

        InitializeItems();
    }

    void InitializeItems() {
        items = new Dictionary<int, Item>();
        for(int i = 0; i < itemArray.Length; i++) {
            if(items.ContainsKey(itemArray[i].ID)) {
                Debug.LogError("DUPLICATE BLOCK ID. SKIPPING ITEM");
            } else {
                items.Add(itemArray[i].ID, itemArray[i]);
            }
        }
    }

    //TODO uuuuuuuuuugh replace this.
    public static Item GetItem(int ID) {
        Item item;
        if(instance.items.TryGetValue(ID, out item)) {
            return Clone(item);
        } else {
            Debug.LogError("Item ID: " + ID + " not found.");
            return null;
        }
    }

    public static Item CreateItem(int ID) {
        Item item;
        if(instance.items.TryGetValue(ID, out item)) {
            return item;
        } else {
            Debug.LogError("Item ID: " + ID + " not found.");
            return null;
        }
    }


    //Man I don't like this
    static Item Clone(Item item) {
        Item newItem = new Item();
        newItem.ID = item.ID;
        newItem.stackSize = item.stackSize;
        newItem.itemName = item.itemName;
        //        item.amount = amount; Done after the clone

        newItem.slot = item.slot;
        newItem.sprite = item.sprite;
        newItem.model = item.model;
        newItem.placeable = item.placeable;
        newItem.block = item.block;
        return item;
    }
}
