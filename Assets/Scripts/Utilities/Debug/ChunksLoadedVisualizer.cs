using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunksLoadedVisualizer : MonoBehaviour {

    class DebugChunkVisualizerObject {
        public Renderer objRenderer;
        public bool loaded = false;
    }

    Dictionary<WorldPos, DebugChunkVisualizerObject> debugMap = new Dictionary<WorldPos, DebugChunkVisualizerObject>();

    int chunkDistance = 20;

    static ChunksLoadedVisualizer instance;

    Transform parent;

    class DelayedChunkLoadedState
    {
        public WorldPos pos;
        public bool loaded;
        public Transform newParent;
        public DelayedChunkLoadedState(WorldPos pos, bool loaded, Transform newParent)
        {
            this.pos = pos;
            this.loaded = loaded;
            this.newParent = newParent;
        }
    }

    Queue<DelayedChunkLoadedState> delayedChunkLoadedStates = new Queue<DelayedChunkLoadedState>();

	void Start () {
        if(instance != null) {
            Debug.LogError("ChunksLoadedVisualizer instance already filled");
            this.enabled = false;
            return;
        }
        instance = this;
        parent = new GameObject().transform;

        //for (int x = -chunkDistance; x < chunkDistance; x++) {
        //    for (int y = -chunkDistance; y < chunkDistance; y++) {
        //        for (int z = -chunkDistance; z < chunkDistance; z++) {
        //            CreateDebugVisualizer(new WorldPos(x * 16, y * 16, z * 16));
        //        }
        //    } 
        //}
	}

    private void Update()
    {
        if(delayedChunkLoadedStates.Count > 0)
        {
            DelayedChunkLoadedState delayedChunkLoadedState = delayedChunkLoadedStates.Dequeue();
            if(delayedChunkLoadedState != null)
            {
                SetChunkLoadedState(delayedChunkLoadedState.pos, delayedChunkLoadedState.loaded, newParent: delayedChunkLoadedState.newParent);
            }
        }
    }

    public static void SetChunkLoadedState(WorldPos pos, bool loaded, bool delayToMainThread = false, Transform newParent = null) {
        if (delayToMainThread)
        {
            instance.delayedChunkLoadedStates.Enqueue(new DelayedChunkLoadedState(pos, loaded, newParent));
        }
        else
        {
            DebugChunkVisualizerObject visualizerObj = instance.GetOrCreateDebugVisualizerObject(pos);
            if (visualizerObj.loaded != loaded)
            {
                visualizerObj.objRenderer.material.color = loaded ? Color.black : Color.red;
            }
            if (newParent != null)
            {
                visualizerObj.objRenderer.transform.parent = newParent;
            }
            else
            {
                visualizerObj.objRenderer.transform.parent = instance.parent;
            }
        }
    }

    DebugChunkVisualizerObject GetOrCreateDebugVisualizerObject(WorldPos pos) {
        DebugChunkVisualizerObject visualizerObj = null;
        if(debugMap.TryGetValue(pos, out visualizerObj)) {
            return visualizerObj;
        } else {
            return CreateDebugVisualizer(pos);
        }
    }

    DebugChunkVisualizerObject CreateDebugVisualizer(WorldPos pos) {
        DebugChunkVisualizerObject visualizerObj = new DebugChunkVisualizerObject();
        visualizerObj.objRenderer = GameObject.CreatePrimitive(PrimitiveType.Sphere).GetComponent<Renderer>();
        visualizerObj.objRenderer.transform.position = pos.ToVector3();
        visualizerObj.objRenderer.material.color = Color.red;
        visualizerObj.objRenderer.transform.parent = parent;
        debugMap.Add(pos, visualizerObj);
        return visualizerObj;
    }
}
