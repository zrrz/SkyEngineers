using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

[System.Serializable]
public struct ChunkComponent : IComponentData {
    //Needs to be native arrays
    public ushort[] blockIDs;
    public ushort[] blockMetaData;
    public ushort[] blockLightData;
}


//public class ChunkComponentComponent : ComponentDataWrapper<ChunkComponent> { }