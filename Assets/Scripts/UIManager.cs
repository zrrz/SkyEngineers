using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

    //Playing: Player moving around so just shorthand inventory and stats
    //Equipment: Shows equipment and full inventory
    //CraftingInHand: Crafting in hand shows full inventory and 2x2 crafting
    //CraftingInBench: Crifting on bench shows full inventory and 3x3 crafting
    //Options: shows options menu
    enum UIState {Playing, Equipment, CraftingInHand, CraftingOnBench, Options}

    UIState uiState = UIState.Playing;

	public PlayerData playerData;
	public PlayerInventory playerInventory;

	Transform healthBar;
	Transform staminaBar;

	//bool showInventory = false;

    bool inventoryDirty = true;

    public static UIManager instance;

	private void Awake()
	{
        if(instance != null) {
            Debug.LogError("Already an instance of UIManager in scene. Disabling this.", this);
            this.enabled = false;
            return;
        }

        instance = this;
	}

	void Start () {
		healthBar = transform.Find("Canvas - GUI/Stats/Health/Foreground");
		staminaBar = transform.Find("Canvas - GUI/Stats/Stamina/Foreground");

		SetInventoryVisibility(false);

        Cursor.lockState = CursorLockMode.Locked;
	}

    public void InitializePlayerUI(PlayerInventory playerInventory, PlayerData playerData) {
        this.playerData = playerData;

        this.playerInventory = playerInventory;
        playerInventory.inventory.inventoryChangedEvent.AddListener(OnInventoryChanged);
        playerInventory.selectionChangedEvent.AddListener(OnSelectionChanged);

        int hotbarSize = 10;

        for (int i = 0; i < hotbarSize; i++)
        {
            //            transform.Find("Canvas/Inventory/Hotbar/Slots/" + i).GetComponent<Image>().sprite = playerInventory.inventory.items[i].sprite;
            Button button = transform.Find("Canvas - MENU/Inventory/Hotbar/Slots/" + i).GetComponent<Button>();
            button.onClick.AddListener(() => ClickSlot(button, playerInventory.inventory));
        }

        for (int i = 10; i < PlayerInventory.INVENTORY_SIZE + PlayerInventory.CRAFTING_SIZE; i++)
        {
            Button button = transform.Find("Canvas - MENU/Inventory/Bag/Slots/" + i).GetComponent<Button>();
            button.onClick.AddListener(() => ClickSlot(button, playerInventory.inventory));
        }
    }
	
	void Update () {
        if (playerData == null || playerInventory == null)
            return;
        
		Vector3 scale = healthBar.transform.localScale;
		scale.x = playerData.currentHealth/playerData.maxHealth;
		healthBar.transform.localScale = scale;

		scale = staminaBar.transform.localScale;
		scale.x = playerData.currentStamina/playerData.maxStamina;
		staminaBar.transform.localScale = scale;

		if(Input.GetKeyDown(KeyCode.E)) {
            switch (uiState)
            {
                case UIState.Playing:
                    SwitchUIState(UIState.Equipment);
                    break;
                case UIState.CraftingInHand:
                case UIState.CraftingOnBench:
                case UIState.Equipment:
                    SwitchUIState(UIState.Playing);
                    break;
                case UIState.Options:
                    break;
                default:
                    Debug.LogError("State undefinded");
                    break;
            }
		}

        if (inventoryDirty)
        {
            inventoryDirty = false;
            UpdateInventory();
        }
	}

    void SwitchUIState(UIState newState) {
        if (newState == uiState) {
            Debug.LogError("Trying to switch from " + uiState.ToString() + " to " + newState.ToString());
			return;
        }
        
        switch (uiState)
        {
            case UIState.Playing:
                switch (newState)
                {
                    case UIState.CraftingInHand:
                    case UIState.CraftingOnBench:
                    case UIState.Equipment:
                        Cursor.lockState = CursorLockMode.None;
                        playerData.GetComponent<FirstPersonControllerCustom>().inputLocked = true;
                        SetInventoryVisibility(true);
                        break;
                    case UIState.Options:
                        break;
                    default:
                        Debug.LogError("State switch undefinded");
                        break;
                }
                break;
            case UIState.CraftingInHand:
            case UIState.CraftingOnBench:
            case UIState.Equipment:
                switch (newState)
                {
                    case UIState.Playing:
                        Cursor.lockState = CursorLockMode.Locked;
                        playerData.GetComponent<FirstPersonControllerCustom>().inputLocked = false;
                        SetInventoryVisibility(false);
                        //TODO drop items in crafting slot
                        break;
                    case UIState.CraftingInHand:
                    case UIState.CraftingOnBench:
                    case UIState.Equipment:
                        break;
                    case UIState.Options:
                        break;
                    default:
                        Debug.LogError("State switch undefinded");
                        break;
                }
                break;
            case UIState.Options:
                break;
            default:
                Debug.LogError("State undefinded");
                break;
        }

        uiState = newState;
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

	public void GrabCraftedItem() {
		playerInventory.CraftItem();
	}

    void OnInventoryChanged() {
        inventoryDirty = true;
    }

    void OnSelectionChanged() {
        transform.Find("Canvas - MENU/Hotbar/Slots/Selection").transform.position = transform.Find("Canvas - MENU/Hotbar/Slots/" + playerInventory.currentActiveSlot).transform.position;
    }

	void SetInventoryVisibility(bool visible) {
		transform.Find("Canvas - MENU/Hotbar").gameObject.SetActive(!visible);
        transform.Find("Canvas - MENU/Inventory").gameObject.SetActive(visible);
//        inventoryDirty = true;
	}

	void UpdateInventory() {
		int hotbarSize = 10;
		for(int i = 0; i < hotbarSize; i++) {
            if (playerInventory.inventory.items[i] != null)
            {
                transform.Find("Canvas - MENU/Inventory/Hotbar/Slots/" + i).GetComponent<Image>().sprite = playerInventory.inventory.items[i].sprite;
                transform.Find("Canvas - MENU/Hotbar/Slots/" + i).GetComponent<Image>().sprite = playerInventory.inventory.items[i].sprite;
                if (playerInventory.inventory.items[i].amount == 1)
                {
                    transform.Find("Canvas - MENU/Inventory/Hotbar/Slots/" + i + "/Amount").GetComponent<Text>().enabled = false;
                    transform.Find("Canvas - MENU/Hotbar/Slots/" + i + "/Amount").GetComponent<Text>().enabled = false;
                }
                else
                {
                    transform.Find("Canvas - MENU/Inventory/Hotbar/Slots/" + i + "/Amount").GetComponent<Text>().enabled = true;
                    transform.Find("Canvas - MENU/Inventory/Hotbar/Slots/" + i + "/Amount").GetComponent<Text>().text = playerInventory.inventory.items[i].amount.ToString();
                    transform.Find("Canvas - MENU/Hotbar/Slots/" + i + "/Amount").GetComponent<Text>().enabled = true;
                    transform.Find("Canvas - MENU/Hotbar/Slots/" + i + "/Amount").GetComponent<Text>().text = playerInventory.inventory.items[i].amount.ToString();
                }
            }
            else
            {
                transform.Find("Canvas - MENU/Inventory/Hotbar/Slots/" + i).GetComponent<Image>().sprite = null;
                transform.Find("Canvas - MENU/Inventory/Hotbar/Slots/" + i + "/Amount").GetComponent<Text>().enabled = false;
                transform.Find("Canvas - MENU/Hotbar/Slots/" + i).GetComponent<Image>().sprite = null;
                transform.Find("Canvas - MENU/Hotbar/Slots/" + i + "/Amount").GetComponent<Text>().enabled = false;
            }
		}
		for (int i = 10; i < PlayerInventory.INVENTORY_SIZE + PlayerInventory.CRAFTING_SIZE; i++)
        {
            if (playerInventory.inventory.items[i] != null)
            {
                transform.Find("Canvas - MENU/Inventory/Bag/Slots/" + i).GetComponent<Image>().sprite = playerInventory.inventory.items[i].sprite;
                if (playerInventory.inventory.items[i].amount == 1)
                {
                    transform.Find("Canvas - MENU/Inventory/Bag/Slots/" + i + "/Amount").GetComponent<Text>().enabled = false;
                }
                else
                {
                    transform.Find("Canvas - MENU/Inventory/Bag/Slots/" + i + "/Amount").GetComponent<Text>().enabled = true;
                    transform.Find("Canvas - MENU/Inventory/Bag/Slots/" + i + "/Amount").GetComponent<Text>().text = playerInventory.inventory.items[i].amount.ToString();
                }
            }
            else
            {
                transform.Find("Canvas - MENU/Inventory/Bag/Slots/" + i).GetComponent<Image>().sprite = null;
                transform.Find("Canvas - MENU/Inventory/Bag/Slots/" + i + "/Amount").GetComponent<Text>().enabled = false;
            }
        }
	}
}
