using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO network sync this bitch

public class PlayerData : MonoBehaviour {

	[System.NonSerialized]
	public float currentHealth;
	public float maxHealth = 100f;
	[System.NonSerialized]
	public float currentStamina;
	public float maxStamina = 100f;

	public float healthRegen = 15f;
	public float stamRegen = 25f;

//	[System.NonSerialized]
	public bool usingStam = false;

	public string playerName = "zrrz";

	void Start () {
		currentHealth = maxHealth;
		currentStamina = maxStamina;
	}
	
	void Update () {
		if(currentHealth < maxHealth) {
			currentHealth += healthRegen * Time.deltaTime;
			if(currentHealth > maxHealth)
				currentHealth = maxHealth;
		}

		if(currentStamina < maxStamina && !usingStam) {
			currentStamina += stamRegen * Time.deltaTime;
			if(currentStamina > maxStamina)
				currentStamina = maxStamina;
		}
	}

	public void ApplyDamage(float amount) {
		currentHealth -= amount;
		if(currentHealth <= 0f) {
			Die();
		}
	}

	public void Die() {
		GameManager.instance.RespawnPlayer(this);
	}
}
