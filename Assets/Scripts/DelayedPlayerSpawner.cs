using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Debug until we get proper loading screens and player de/serialization
/// </summary>
public class DelayedPlayerSpawner : MonoBehaviour {

    public float delay = 2f;

    [SerializeField]
    GameObject playerPrefab;

    IEnumerator Start () {
        yield return new WaitForSeconds(delay);

        GameObject playerObj = Instantiate(playerPrefab, transform.position, Quaternion.identity);

        playerObj.GetComponent<RenderNearbyChunks>().world = World.instance;
	}

	private void OnDrawGizmos()
	{
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 1f);
	}
}
