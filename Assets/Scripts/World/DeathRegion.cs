using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathRegion : MonoBehaviour {

	public float damageAmount = 10f;

	void OnTriggerStay(Collider col) {
		if(col.GetComponent<PlayerData>() != null) {
			col.GetComponent<PlayerData>().ApplyDamage(damageAmount*Time.deltaTime);
		}
	}
}
