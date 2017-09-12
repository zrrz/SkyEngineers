using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour {

	public PlayerData playerData;

	Transform healthBar;
	Transform staminaBar;

	void Start () {
		healthBar = transform.Find("Stats/Health/Foreground");
		staminaBar = transform.Find("Stats/Stamina/Foreground");
	}
	
	void Update () {
		Vector3 scale = healthBar.transform.localScale;
		scale.x = playerData.currentHealth/playerData.maxHealth;
		healthBar.transform.localScale = scale;

		scale = staminaBar.transform.localScale;
		scale.x = playerData.currentStamina/playerData.maxStamina;
		staminaBar.transform.localScale = scale;
	}
}
