using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Buffers;

public class MemoryPoolManager : MonoBehaviour {

	public static ArrayPool<Vector2> vector2ArrayPool;
    public static ArrayPool<Vector3> vector3ArrayPool;

	// Use this for initialization
	void Start () {
		vector2ArrayPool = ArrayPool<Vector2>.Create(4, 256);
        vector3ArrayPool = ArrayPool<Vector3>.Create(4, 256);
	}
	
	//// Update is called once per frame
	//void Update () {
 //       UnityEngine.Profiling.Profiler.BeginSample("Use arrayPool");

 //       for (int i = 0; i < 4096/2*6; i++)
 //       {
 //           arrayPool.Return(arrayPool.Rent(4));
 //       }

 //       UnityEngine.Profiling.Profiler.EndSample();

 //       UnityEngine.Profiling.Profiler.BeginSample("Use GC");

 //       for (int i = 0; i < 4096 / 2 * 6; i++)
 //       {
 //           Vector3[] vector3s = new Vector3[4];
 //       }

 //       UnityEngine.Profiling.Profiler.EndSample();
	//}
}
