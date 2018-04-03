using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Collections;

public class ChunkSystem : ComponentSystem {

    public struct Data
    {
        public ComponentDataArray<ChunkComponent> Chunks;
        public int Length;
        //public GameObjectArray GameObject;
        //public ComponentArray<Rigidbody> Rigidbody;
        //[ReadOnly] public ComponentDataArray<PlayerInput> PlayerInput;
        //public SubtractiveComponent<Dead> Dead;
    }

    //[Inject]
    private Data data;

    protected override void OnUpdate()
    {
        // Iterate over all entities matching the declared ComponentGroup required types
        for (int i = 0; i != data.Length; i++)
        {
            //ChunkComponent chunk = data.Chunks[i];
            //chunk.blockIDs = new ushort[16 * 16 * 16];
            //data.Chunks[i] = chunk;
        }
    }
}
