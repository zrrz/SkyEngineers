﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockPlacer : MonoBehaviour {

	public float maxReach = 2f;
	public float damageAmount = 5f;

	public LayerMask layerMask;

	void Start () {
		
	}
	
	void Update () {
		if(Input.GetButtonDown("Fire1")) {
			Debug.DrawRay(Camera.main.ScreenPointToRay(Input.mousePosition).origin, Camera.main.ScreenPointToRay(Input.mousePosition).direction, Color.red);
			RaycastHit hit;
			if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, maxReach, layerMask, QueryTriggerInteraction.Ignore)) {//, int.MaxValue, QueryTriggerInteraction.Ignore)) {
				EditTerrain.SetBlock(hit, BlockLoader.GetBlock(0));
			}
 		}
	}
}
