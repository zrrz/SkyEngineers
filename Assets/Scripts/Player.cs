using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BeardedManStudios.Forge.Networking.Generated;

public class Player : PlayerBehavior {

    Anchor m_anchor;
    public Anchor anchor { get { return m_anchor; } }
    PlayerInventory m_inventory;
    public PlayerInventory inventory { get { return m_inventory; } }
    PlayerData m_playerData;
    public PlayerData playerData { get { return m_playerData; } }

    void Awake() {
        m_inventory = GetComponent<PlayerInventory>();
        if (m_inventory == null)
        {
            Debug.LogError("No PlayerInventory on object", this);
        }
        m_playerData = GetComponent<PlayerData>();
        if (m_playerData == null)
        {
            Debug.LogError("No PlayerData on object", this);
        }
    }

    void Start () {
        m_anchor = new Anchor();
	}
	
	void Update () {
        anchor.position = transform.position;
	}
}
