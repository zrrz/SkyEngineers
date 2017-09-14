using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item {

	public int ID;
	public int stackSize;
    public int amount;
	public string itemName;

	public Sprite sprite; 
	public GameObject model; //When on ground

    public enum EquipSlot {
        None = 0,
        Head = 1,
        Chest = 2,
        Legs = 3,
        Boots = 4,
        Gloves = 5,
        Jewelry = 6,
//        Offhand = 6,
    }

    public EquipSlot slot;

	//TODO usable/equipable/consumable

	public bool placeable;
	public BlockData block;

	//TODO recipe

//	void Start () {
//		
//	}
//	
//	void Update () {
//		
//	}

	void PlaceBlock() {

	}
}
