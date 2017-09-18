using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

	public PlayerData playerData;
	public PlayerInventory playerInventory;

	Transform healthBar;
	Transform staminaBar;

	bool showInventory = false;

	void Start () {
		healthBar = transform.Find("Stats/Health/Foreground");
		staminaBar = transform.Find("Stats/Stamina/Foreground");

		SetInventoryVisibility(false);
	}
	
	void Update () {
		Vector3 scale = healthBar.transform.localScale;
		scale.x = playerData.currentHealth/playerData.maxHealth;
		healthBar.transform.localScale = scale;

		scale = staminaBar.transform.localScale;
		scale.x = playerData.currentStamina/playerData.maxStamina;
		staminaBar.transform.localScale = scale;

		if(Input.GetKeyDown(KeyCode.E)) {
			showInventory = !showInventory;
			SetInventoryVisibility(showInventory);
		}
	}

	void SetInventoryVisibility(bool visible) {
		transform.Find("Hotbar").gameObject.SetActive(!visible);
		transform.Find("Inventory").gameObject.SetActive(visible);
		UpdateInventory();
	}

	void UpdateInventory() {
		int hotbarSize = 10;
		for(int i = 0; i < hotbarSize; i++) {
			if(playerInventory.inventory.items[i] != null) {
				transform.Find("Inventory/Hotbar/Slots/" + i).GetComponent<Image>().sprite = playerInventory.inventory.items[i].sprite;
				transform.Find("Inventory/Hotbar/Slots/" + i + "/Amount").GetComponent<Text>().text = playerInventory.inventory.items[i].amount.ToString();
			}
		}
		for(int i = 0; i < playerInventory.inventory.slots-hotbarSize; i++) {
			if(playerInventory.inventory.items[i+hotbarSize] != null)
				transform.Find("Inventory/Bag/Slots/" + i).GetComponent<Image>().sprite = playerInventory.inventory.items[i+hotbarSize].sprite;
		}
	}
}
