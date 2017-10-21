using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BeardedManStudios.Forge.Networking.Unity;

public class GameLogic : MonoBehaviour {

    private void Start()
    {
        NetworkManager.Instance.InstantiatePlayer();
    }
}
