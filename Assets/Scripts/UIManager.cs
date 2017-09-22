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

    bool inventoryDirty = true;

	void Start () {
		healthBar = transform.Find("Stats/Health/Foreground");
		staminaBar = transform.Find("Stats/Stamina/Foreground");

		SetInventoryVisibility(false);

        playerInventory.inventory.inventoryChangedEvent.AddListener(OnInventoryChanged);
        playerInventory.selectionChangedEvent.AddListener(OnSelectionChanged);

        Cursor.lockState = CursorLockMode.Locked;

        int hotbarSize = 10;

        for(int i = 0; i < hotbarSize; i++) {
//            transform.Find("Canvas/Inventory/Hotbar/Slots/" + i).GetComponent<Image>().sprite = playerInventory.inventory.items[i].sprite;
            Button button = transform.Find("Canvas/Inventory/Hotbar/Slots/" + i).GetComponent<Button>();
            button.onClick.AddListener(() => ClickSlot(button, playerInventory.inventory));
        }

        for (int i = 10; i < playerInventory.inventory.slots; i++)
        {
            Button button = transform.Find("Canvas/Inventory/Bag/Slots/" + i).GetComponent<Button>();
            button.onClick.AddListener(() => ClickSlot(button, playerInventory.inventory));
        }
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
            Cursor.lockState = showInventory ? CursorLockMode.Confined : CursorLockMode.Locked;
            playerData.GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonControllerCustom>().inputLocked = showInventory;
			SetInventoryVisibility(showInventory);
		}

        if (inventoryDirty)
        {
            inventoryDirty = false;
            UpdateInventory();
        }
	}

    int selectedSlot = -1;
    Inventory selectedInventory = null;
    Button selectedButton = null;

    public void ClickSlot(Button button, Inventory inventory) {
        if (selectedSlot != -1)
        {
            int newSlot = int.Parse(button.name);
            Item tempItem = inventory.items[newSlot];
            if (inventory.items[newSlot] == selectedInventory.items[selectedSlot])
            {
                //Same button. Deselect
            }
            else
            {
                inventory.items[newSlot] = selectedInventory.items[selectedSlot];
                selectedInventory.items[selectedSlot] = tempItem;
                selectedInventory.inventoryChangedEvent.Invoke();
                if (inventory != selectedInventory)
                    inventory.inventoryChangedEvent.Invoke();
            }
            selectedSlot = -1;
            selectedInventory = null;
            selectedButton.GetComponent<Image>().color = Color.white;
            selectedButton = null;
        } else {
            selectedSlot = int.Parse(button.name);
            if (inventory.items[selectedSlot] != null)
            {
                selectedInventory = inventory;
                button.GetComponent<Image>().color = Color.red;
                selectedButton = button;
            }
            else
            {
                selectedSlot = -1;
            }
        }
    }

    void OnInventoryChanged() {
        inventoryDirty = true;
    }

    void OnSelectionChanged() {
        transform.Find("Canvas/Hotbar/Slots/Selection").transform.position = transform.Find("Canvas/Hotbar/Slots/" + playerInventory.currentActiveSlot).transform.position;
    }

	void SetInventoryVisibility(bool visible) {
		transform.Find("Canvas/Hotbar").gameObject.SetActive(!visible);
        transform.Find("Canvas/Inventory").gameObject.SetActive(visible);
//        inventoryDirty = true;
	}

	void UpdateInventory() {
		int hotbarSize = 10;
		for(int i = 0; i < hotbarSize; i++) {
            if (playerInventory.inventory.items[i] != null)
            {
                transform.Find("Canvas/Inventory/Hotbar/Slots/" + i).GetComponent<Image>().sprite = playerInventory.inventory.items[i].sprite;
                transform.Find("Canvas/Hotbar/Slots/" + i).GetComponent<Image>().sprite = playerInventory.inventory.items[i].sprite;
                if (playerInventory.inventory.items[i].amount == 1)
                {
                    transform.Find("Canvas/Inventory/Hotbar/Slots/" + i + "/Amount").GetComponent<Text>().enabled = false;
                    transform.Find("Canvas/Hotbar/Slots/" + i + "/Amount").GetComponent<Text>().enabled = false;
                }
                else
                {
                    transform.Find("Canvas/Inventory/Hotbar/Slots/" + i + "/Amount").GetComponent<Text>().enabled = true;
                    transform.Find("Canvas/Inventory/Hotbar/Slots/" + i + "/Amount").GetComponent<Text>().text = playerInventory.inventory.items[i].amount.ToString();
                    transform.Find("Canvas/Hotbar/Slots/" + i + "/Amount").GetComponent<Text>().enabled = true;
                    transform.Find("Canvas/Hotbar/Slots/" + i + "/Amount").GetComponent<Text>().text = playerInventory.inventory.items[i].amount.ToString();
                }
            }
            else
            {
                transform.Find("Canvas/Inventory/Hotbar/Slots/" + i).GetComponent<Image>().sprite = null;
                transform.Find("Canvas/Inventory/Hotbar/Slots/" + i + "/Amount").GetComponent<Text>().enabled = false;
                transform.Find("Canvas/Hotbar/Slots/" + i).GetComponent<Image>().sprite = null;
                transform.Find("Canvas/Hotbar/Slots/" + i + "/Amount").GetComponent<Text>().enabled = false;
            }
		}
        for (int i = 10; i < playerInventory.inventory.slots; i++)
        {
            if (playerInventory.inventory.items[i] != null)
            {
                transform.Find("Canvas/Inventory/Bag/Slots/" + i).GetComponent<Image>().sprite = playerInventory.inventory.items[i].sprite;
                if (playerInventory.inventory.items[i].amount == 1)
                {
                    transform.Find("Canvas/Inventory/Bag/Slots/" + i + "/Amount").GetComponent<Text>().enabled = false;
                }
                else
                {
                    transform.Find("Canvas/Inventory/Bag/Slots/" + i + "/Amount").GetComponent<Text>().enabled = true;
                    transform.Find("Canvas/Inventory/Bag/Slots/" + i + "/Amount").GetComponent<Text>().text = playerInventory.inventory.items[i].amount.ToString();
                }
            }
            else
            {
                transform.Find("Canvas/Inventory/Bag/Slots/" + i).GetComponent<Image>().sprite = null;
                transform.Find("Canvas/Inventory/Bag/Slots/" + i + "/Amount").GetComponent<Text>().enabled = false;
            }
        }
	}
}
