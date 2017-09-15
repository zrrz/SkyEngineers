using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : MonoBehaviour {

	public int itemID;
	public int amount;

	void OnCollisionEnter(Collision col) {
		if(col.gameObject.GetComponent<PlayerInventory>() != null) {
			Debug.LogError("Col2!");
			col.gameObject.GetComponent<PlayerInventory>().AddItem(itemID, amount);
			Destroy(gameObject);
		}
	}

//	void Start () {
//		
//	}
//	
//	void Update () {
//		
//	}
}
