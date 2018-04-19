using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockPlacer : MonoBehaviour {

	public float maxReach = 2f;
	public float damageAmount = 5f;

	public LayerMask layerMask;
    public Transform blockSelector;

	void Start () {
        blockSelector.transform.parent = null;
        blockSelector.transform.rotation = Quaternion.identity;
	}
	
    //TODO clean this up to reduce raycasts
	void Update () {
        if (GetComponent<FirstPersonControllerCustom>().inputLocked)
            return;
        
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0f)), out hit, maxReach, layerMask, QueryTriggerInteraction.Ignore))
        {
            if (!blockSelector.gameObject.activeSelf)
                blockSelector.gameObject.SetActive(true);
            hit.point += (-hit.normal*0.1f); //Smudging in a bit to fix edge case
            WorldPos pos = EditTerrain.GetBlockPos(hit);
            blockSelector.position = pos.ToVector3();
        }
        else
        {
            if (blockSelector.gameObject.activeSelf)
                blockSelector.gameObject.SetActive(false);
        }

		if(Input.GetButtonDown("Fire1")) {
            if(Physics.Raycast(Camera.main.ScreenPointToRay(new Vector3(Screen.width/2f, Screen.height/2f, 0f)), out hit, maxReach, layerMask, QueryTriggerInteraction.Ignore)) {//, int.MaxValue, QueryTriggerInteraction.Ignore)) {
				EditTerrain.BreakBlock(hit);
			}
 		}
        if(Input.GetButton("Fire2")) {
//            RaycastHit hit;
            if(Physics.Raycast(Camera.main.ScreenPointToRay(new Vector3(Screen.width/2f, Screen.height/2f, 0f)), out hit, maxReach, layerMask, QueryTriggerInteraction.Ignore)) {//, int.MaxValue, QueryTriggerInteraction.Ignore)) {
                if (GetComponent<PlayerInventory>().CurrentActiveItem != null && GetComponent<PlayerInventory>().CurrentActiveItem.placeable)
                {
                    hit.point += hit.normal;
                    WorldPos pos = EditTerrain.GetBlockPos(hit);
                    Collider[] cols = Physics.OverlapBox(pos.ToVector3(), Vector3.one * 0.45f, Quaternion.identity, LayerMask.GetMask("Default", "Blocks"),QueryTriggerInteraction.Ignore);
                    if (cols.Length > 0)
                    {
                        //Something in way
                        foreach (Collider col in cols)
                        {
                            if(col.GetComponentInChildren<Renderer>() != null)
                                StartCoroutine(TempFlashRed(col.GetComponentInChildren<Renderer>()));
                        }
                    }
                    else
                    {
                        //                    RaycastHit hit2;
                        //                    if (Physics.BoxCast(pos.ToVector3(), Vector3.one * 0.35f, Vector3.up * 0.001f, out hit2))
                        //                    {
                        //                        Debug.LogError(pos.ToVector3() + " - " + hit2.collider.gameObject.name + " - " + hit2.distance);
                        //                        ExtDebug.DrawBoxCastOnHit(pos.ToVector3(), Vector3.one * 0.35f, Quaternion.identity, Vector3.up * 0.001f, 0f, Color.red);
                        //                    }

                        //TODO fix
                        EditTerrain.PlaceBlock(hit, FindObjectOfType<PlayerInventory>().CurrentActiveItem.blockID);
                        GetComponent<PlayerInventory>().ConsumeCurrentItem();
                    }
                }
            }
        }
	}

    //TODO fix. Crazy inneficient super temp. Essentially a memory leak the more I use it on blocks making new mats
    IEnumerator TempFlashRed(Renderer rend) {
        for(float t = 0; t < 1f; t += Time.deltaTime) {
            if (rend == null)
                break;
            rend.material.color = Color.Lerp(Color.red, Color.white, t);
            yield return null;
        }
        if(rend)
            rend.material.color = Color.Lerp(Color.red, Color.white, 1f);
    }
}
