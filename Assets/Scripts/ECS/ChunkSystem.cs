using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Collections;

public class ChunkSystem : ComponentSystem {

    public struct Data
    {
        public int Length;
        public GameObjectArray GameObject;
        public ComponentArray<Rigidbody> Rigidbody;
        //[ReadOnly] public ComponentDataArray<PlayerInput> PlayerInput;
        //public SubtractiveComponent<Dead> Dead;
    }

    [Inject] private Data data;

    protected override void OnUpdate()
    {

    }
}
