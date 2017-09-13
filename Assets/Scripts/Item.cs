using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour {

	public int ID;
	public int amount;
	public string itemName;

	public Sprite sprite; 
	public GameObject model; //When on ground

	//TODO usable/equipable/consumable

	public bool placeable;
	public BlockData block;

	//TODO recipe


	void Start () {
		
	}
	
	void Update () {
		
	}

	void PlaceBlock() {

	}
}
