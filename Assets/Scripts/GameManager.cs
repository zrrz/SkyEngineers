using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

	public static GameManager instance;

	World world;

	void Awake() {
		if(instance != null) {
			Debug.LogError("Already an isntance of GameManager. Disabling me", this);
			this.enabled = false;
		} else {
			instance = this;
            world = GameObject.FindObjectOfType<World>();
			if(world == null) {
				Debug.LogError("No instance of world in scene.");
			}
		}
	}
	
	void Update () {
		
	}

	public void RespawnPlayer(PlayerData player) {
		player.transform.position = world.spawnPoint;
		player.currentHealth = player.maxHealth;
		player.currentStamina = player.maxHealth;
		player.GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonControllerCustom>().ResetVelocity();
	}
}
