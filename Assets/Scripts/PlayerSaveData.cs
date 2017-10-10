using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class PlayerSaveData {

	[SerializeField]
	public Vector3 position;
	[SerializeField]
	public Item[] inventory;
	[SerializeField]
	public Item[] equipment;

	public PlayerSaveData(PlayerData player) {
		position = player.transform.position;
		inventory = player.GetComponent<PlayerInventory>().inventory.items;
		equipment = player.GetComponent<PlayerInventory>().equipment.items;
	}
}
