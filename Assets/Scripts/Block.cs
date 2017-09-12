using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour {

	public enum HardnessTier {
		Wood = 0,
		Stone = 1,
		Copper = 2,
		Iron = 3,
	}

	public int ID;
	public HardnessTier hardnessTier = HardnessTier.Wood;
	public int brightness = 0;

	//TODO Func
	//	IO (Logic)
	//	Storage 
	//	GUI
	//	Grow

	//TODO sounds
	//TODO Flow (Speed) 0-100 [gravity=10]
	//TODO Heat (Temp) in C or F Adjusted in settings.
	//TODO Power (Volts) 
	//TODO Multiblock Structure...
	//TODO Player Phys

	public float maxHealth = 100f;
	[System.NonSerialized]
	public float currentHealth;

	float healTimer = 0f;


	void Start () {
		currentHealth = maxHealth;
	}
	
	public void BlockUpdate () {
		if(healTimer > 0) {
			healTimer -= Time.deltaTime;
			if(healTimer <= 0f) {
				currentHealth = maxHealth;
			}
			foreach(Renderer rend in GetComponentsInChildren<Renderer>()) {
				rend.material.color = Color.Lerp(Color.red, Color.white, currentHealth/maxHealth);
			}
		}
	}

	/// <summary>
	/// Damages block. Returns true if broken.
	/// </summary>
	/// <param name="damage">Damage.</param>
	public bool Damage(float damage) {
		healTimer = 1f;
		currentHealth -= damage;
		return (currentHealth <= 0f);
	}
}
