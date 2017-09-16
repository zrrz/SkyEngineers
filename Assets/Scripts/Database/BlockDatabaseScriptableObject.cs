using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(fileName = "Data", menuName = "Inventory/List", order = 1)]
public class BlockDatabaseScriptableObject : ScriptableObject {
    [SerializeField]
    public List<BlockData> blocks;
}
