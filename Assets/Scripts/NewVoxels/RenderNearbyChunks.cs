using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.Jobs;
using Unity.Collections;

public class RenderNearbyChunks : MonoBehaviour
{
    static WorldPos[] chunkPositions = {
                            new WorldPos( 0, 0, 0), new WorldPos( 0,-1, 0), new WorldPos( 0, 1, 0), new WorldPos(-1, 0, 0), new WorldPos( 0, 0,-1),
                            new WorldPos( 0, 0, 1), new WorldPos( 1, 0, 0), new WorldPos( 1, 1, 0), new WorldPos( 0, 1, 1), new WorldPos( 1,-1, 0),
                            new WorldPos( 0,-1, 1), new WorldPos(-1, 1, 0), new WorldPos( 0, 1,-1), new WorldPos(-1,-1, 0), new WorldPos( 0,-1,-1),
                            new WorldPos(-1, 0,-1), new WorldPos(-1, 0, 1), new WorldPos( 1, 0,-1), new WorldPos( 1, 0, 1), new WorldPos(-1,-1,-1),
                            new WorldPos(-1,-1, 1), new WorldPos( 1,-1,-1), new WorldPos( 1,-1, 1), new WorldPos(-1, 1,-1), new WorldPos(-1, 1, 1),
                            new WorldPos( 1, 1,-1), new WorldPos( 1, 1, 1), };

                            //new WorldPos(-2, 0,  0), new WorldPos( 0, 0, -2), new WorldPos( 0, 0,  2), new WorldPos( 2, 0,  0), new WorldPos(-2, 0, -1),
                            //new WorldPos(-2, 0,  1), new WorldPos(-1, 0, -2), new WorldPos(-1, 0,  2), new WorldPos( 1, 0, -2), new WorldPos( 1, 0,  2),
                            //new WorldPos( 2, 0, -1), new WorldPos( 2, 0,  1), new WorldPos(-2, 0, -2), new WorldPos(-2, 0,  2), new WorldPos( 2, 0, -2),
                            //new WorldPos( 2, 0,  2), new WorldPos(-3, 0,  0), new WorldPos( 0, 0, -3), new WorldPos( 0, 0,  3), new WorldPos( 3, 0,  0),
                            //new WorldPos(-3, 0, -1), new WorldPos(-3, 0,  1), new WorldPos(-1, 0, -3), new WorldPos(-1, 0,  3), new WorldPos( 1, 0, -3),
                            //new WorldPos( 1, 0,  3), new WorldPos( 3, 0, -1), new WorldPos( 3, 0,  1), new WorldPos(-3, 0, -2), new WorldPos(-3, 0,  2),
                            //new WorldPos(-2, 0, -3), new WorldPos(-2, 0,  3), new WorldPos( 2, 0, -3), new WorldPos( 2, 0,  3), new WorldPos( 3, 0, -2),
                            //new WorldPos( 3, 0,  2), new WorldPos(-4, 0,  0), new WorldPos( 0, 0, -4), new WorldPos( 0, 0,  4), new WorldPos( 4, 0,  0),
                            //new WorldPos(-4, 0, -1), new WorldPos(-4, 0,  1), new WorldPos(-1, 0, -4), new WorldPos(-1, 0,  4), new WorldPos( 1, 0, -4),
                            //new WorldPos( 1, 0,  4), new WorldPos( 4, 0, -1), new WorldPos( 4, 0,  1), new WorldPos(-3, 0, -3), new WorldPos(-3, 0,  3),
                            //new WorldPos( 3, 0, -3), new WorldPos( 3, 0,  3), new WorldPos(-4, 0, -2), new WorldPos(-4, 0,  2), new WorldPos(-2, 0, -4),
                            //new WorldPos(-2, 0,  4), new WorldPos( 2, 0, -4), new WorldPos( 2, 0,  4), new WorldPos( 4, 0, -2), new WorldPos( 4, 0,  2),
                            //new WorldPos(-5, 0,  0), new WorldPos(-4, 0, -3), new WorldPos(-4, 0,  3), new WorldPos(-3, 0, -4), new WorldPos(-3, 0,  4),
                            //new WorldPos( 0, 0, -5), new WorldPos( 0, 0,  5), new WorldPos( 3, 0, -4), new WorldPos( 3, 0,  4), new WorldPos( 4, 0, -3),
                            //new WorldPos( 4, 0,  3), new WorldPos( 5, 0,  0), new WorldPos(-5, 0, -1), new WorldPos(-5, 0,  1), new WorldPos(-1, 0, -5),
                            //new WorldPos(-1, 0,  5), new WorldPos( 1, 0, -5), new WorldPos( 1, 0,  5), new WorldPos( 5, 0, -1), new WorldPos( 5, 0,  1),
                            //new WorldPos(-5, 0, -2), new WorldPos(-5, 0,  2), new WorldPos(-2, 0, -5), new WorldPos(-2, 0,  5), new WorldPos( 2, 0, -5),
                            //new WorldPos( 2, 0,  5), new WorldPos( 5, 0, -2), new WorldPos( 5, 0,  2), new WorldPos(-4, 0, -4), new WorldPos(-4, 0,  4),
                            //new WorldPos( 4, 0, -4), new WorldPos( 4, 0,  4), new WorldPos(-5, 0, -3), new WorldPos(-5, 0,  3), new WorldPos(-3, 0, -5),
                            //new WorldPos(-3, 0,  5), new WorldPos( 3, 0, -5), new WorldPos( 3, 0,  5), new WorldPos( 5, 0, -3), new WorldPos( 5, 0,  3),
                            //new WorldPos(-6, 0,  0), new WorldPos( 0, 0, -6), new WorldPos( 0, 0,  6), new WorldPos( 6, 0,  0), new WorldPos(-6, 0, -1),
                            //new WorldPos(-6, 0,  1),  new WorldPos(-1, 0, -6), new WorldPos(-1, 0,  6), new WorldPos( 1, 0, -6), new WorldPos( 1, 0,  6),
                            //new WorldPos( 6, 0, -1), new WorldPos( 6, 0,  1), new WorldPos(-6, 0, -2), new WorldPos(-6, 0,  2), new WorldPos(-2, 0, -6),
                            //new WorldPos(-2, 0,  6), new WorldPos( 2, 0, -6), new WorldPos( 2, 0,  6), new WorldPos( 6, 0, -2), new WorldPos( 6, 0,  2),
                            //new WorldPos(-5, 0, -4), new WorldPos(-5, 0,  4), new WorldPos(-4, 0, -5), new WorldPos(-4, 0,  5), new WorldPos( 4, 0, -5),
                            //new WorldPos( 4, 0,  5), new WorldPos( 5, 0, -4), new WorldPos( 5, 0,  4), new WorldPos(-6, 0, -3), new WorldPos(-6, 0,  3),
                            //new WorldPos(-3, 0, -6), new WorldPos(-3, 0,  6), new WorldPos( 3, 0, -6), new WorldPos( 3, 0,  6), new WorldPos( 6, 0, -3),
                            //new WorldPos( 6, 0,  3), new WorldPos(-7, 0,  0), new WorldPos( 0, 0, -7), new WorldPos( 0, 0,  7), new WorldPos( 7, 0,  0),
                            //new WorldPos(-7, 0, -1), new WorldPos(-7, 0,  1), new WorldPos(-5, 0, -5), new WorldPos(-5, 0,  5), new WorldPos(-1, 0, -7),
                            //new WorldPos(-1, 0,  7), new WorldPos( 1, 0, -7), new WorldPos( 1, 0,  7), new WorldPos( 5, 0, -5), new WorldPos( 5, 0,  5),
                            //new WorldPos( 7, 0, -1), new WorldPos( 7, 0,  1), new WorldPos(-6, 0, -4), new WorldPos(-6, 0,  4), new WorldPos(-4, 0, -6),
                            //new WorldPos(-4, 0,  6), new WorldPos( 4, 0, -6), new WorldPos( 4, 0,  6), new WorldPos( 6, 0, -4), new WorldPos( 6, 0,  4),
                            //new WorldPos(-7, 0, -2), new WorldPos(-7, 0,  2), new WorldPos(-2, 0, -7), new WorldPos(-2, 0,  7), new WorldPos( 2, 0, -7),
                            //new WorldPos( 2, 0,  7), new WorldPos( 7, 0, -2), new WorldPos( 7, 0,  2), new WorldPos(-7, 0, -3), new WorldPos(-7, 0,  3),
                            //new WorldPos(-3, 0, -7), new WorldPos(-3, 0,  7), new WorldPos( 3, 0, -7), new WorldPos( 3, 0,  7), new WorldPos( 7, 0, -3),
                            //new WorldPos( 7, 0,  3), new WorldPos(-6, 0, -5), new WorldPos(-6, 0,  5), new WorldPos(-5, 0, -6), new WorldPos(-5, 0,  6),
                            //new WorldPos( 5, 0, -6), new WorldPos( 5, 0,  6), new WorldPos( 6, 0, -5), new WorldPos( 6, 0,  5) };

    public World world;
//
    //List<WorldPos> updateList = new List<WorldPos>();
    List<WorldPos> buildList = new List<WorldPos>();

    int timer = 0;
    readonly Dictionary<WorldPos, ChunkRenderer> chunkRendererMap = new Dictionary<WorldPos, ChunkRenderer>();
    List<WorldPos> chunkRenderers = new List<WorldPos>();

    [SerializeField]
    GameObject chunkPrefab;

    public int renderDistance = 4;
    public int chunkToBuildPerFrame = 2;

    Transform _transform;

    void Start() {
        _transform = transform;
    }

    void Update()
    {
        UnityEngine.Profiling.Profiler.BeginSample("DeleteChunkRenderers");
        if (DeleteChunkRenderers())
            return;
        UnityEngine.Profiling.Profiler.EndSample();

        UnityEngine.Profiling.Profiler.BeginSample("FindChunksToLoad");
        FindChunksToLoad();
        UnityEngine.Profiling.Profiler.EndSample();

        UnityEngine.Profiling.Profiler.BeginSample("LoadAndRenderChunks");
        LoadAndRenderChunks();
        UnityEngine.Profiling.Profiler.EndSample();

        UnityEngine.Profiling.Profiler.BeginSample("updateChunks");
        for (int i = 0, n = chunkRenderers.Count; i < n; i++) {
            ChunkRenderer chunkRenderer = chunkRendererMap[chunkRenderers[i]];
            if(chunkRenderer.chunk.update) {
                chunkRenderer.UpdateChunk();
                //break;
            }
        }
        UnityEngine.Profiling.Profiler.EndSample();
    }

    int viewDistanceIndex = 0;

    void FindChunksToLoad()
    {
        //Get the position of this gameobject to generate around
        WorldPos playerPos = new WorldPos(
            Mathf.FloorToInt(_transform.position.x / ChunkInstance.CHUNK_SIZE) * ChunkInstance.CHUNK_SIZE,
            Mathf.FloorToInt(_transform.position.y / ChunkInstance.CHUNK_SIZE) * ChunkInstance.CHUNK_SIZE,
            Mathf.FloorToInt(_transform.position.z / ChunkInstance.CHUNK_SIZE) * ChunkInstance.CHUNK_SIZE
            );

        if (buildList.Count == 0)
        {

            //if(!buildList.Contains(playerPos))
            //{
            //    If no chunk data available, ignore.
            //    if (world.GetChunk(playerPos.x, playerPos.y, playerPos.z) != null)
            //    {
            //        ChunksLoadedVisualizer.SetChunkLoadedState(newChunkPos, false);
            //        Debug.LogError("No world data at " + newChunkPos.ToString());

            //        ChunkRenderer newChunk;
            //        chunkRendererMap.TryGetValue(playerPos, out newChunk);

            //        If the chunk already exists and it's already
            //        rendered or in queue to be rendered continue
            //        if (newChunk == null)
            //        {
            //            Otherwise let's build it
            //            BuildChunkRenderer(playerPos);
            //        }
            //    }
            //}

            for (int i = 0, n = chunkPositions.Length; i < n; i++)
            {
                WorldPos newChunkPos = new WorldPos(
                        chunkPositions[i].x * ChunkInstance.CHUNK_SIZE + playerPos.x,
                        chunkPositions[i].y * ChunkInstance.CHUNK_SIZE + playerPos.y,
                        chunkPositions[i].z * ChunkInstance.CHUNK_SIZE + playerPos.z
                    );

                if (buildList.Contains(newChunkPos))
                    continue;

                //If no chunk data available, ignore.
                if (world.GetChunk(newChunkPos.x, newChunkPos.y, newChunkPos.z) == null)
                {
                    //ChunksLoadedVisualizer.SetChunkLoadedState(newChunkPos, false);
                    //Debug.LogError("No world data at " + newChunkPos.ToString());
                    continue;
                }

                ChunkRenderer newChunk;
                chunkRendererMap.TryGetValue(newChunkPos, out newChunk);

                //If the chunk already exists and it's already
                //rendered or in queue to be rendered continue
                if (newChunk != null
                    && (newChunk.rendered))
                {
                    continue;
                }

                //Otherwise let's build it
                buildList.Add(newChunkPos);
            }
        }

        //    for (var x = -1; x <= 1; x++)
        //    {
        //        for (var y = -1; y <= 1; y++)
        //        {
        //            for (var z = -1; z <= 1; z++)
        //            {
        //                WorldPos newChunkPos = new WorldPos(
        //                    x * ChunkInstance.CHUNK_SIZE + playerPos.x,
        //                    y * ChunkInstance.CHUNK_SIZE + playerPos.y,
        //                    z * ChunkInstance.CHUNK_SIZE + playerPos.z
        //                );

        //                if (buildList.Contains(newChunkPos))
        //                    continue;

        //                //If no chunk data available, ignore.
        //                if (world.GetChunk(newChunkPos.x, newChunkPos.y, newChunkPos.z) == null)
        //                {
        //                    //ChunksLoadedVisualizer.SetChunkLoadedState(newChunkPos, false);
        //                    //Debug.LogError("No world data at " + newChunkPos.ToString());
        //                    continue;
        //                }

        //                ChunkRenderer newChunk;
        //                chunkRendererMap.TryGetValue(newChunkPos, out newChunk);

        //                //If the chunk already exists and it's already
        //                //rendered or in queue to be rendered continue
        //                if (newChunk != null
        //                    && (newChunk.rendered))
        //                {
        //                    continue;
        //                }

        //                //Otherwise let's build it
        //                buildList.Add(newChunkPos);
        //            }
        //        }
        //    }

        //}
        return;
        if (buildList.Count == 0)
        {
            Vector3 viewDirection = Camera.main.transform.forward;

            WorldPos chunkPosition = playerPos;
            for (int i = 0; i < renderDistance; i++)
            {
                chunkPosition.x += (int)(viewDirection.x * ChunkInstance.CHUNK_SIZE);
                chunkPosition.y += (int)(viewDirection.y * ChunkInstance.CHUNK_SIZE);
                chunkPosition.z += (int)(viewDirection.z * ChunkInstance.CHUNK_SIZE);

                if (buildList.Contains(chunkPosition))
                    continue;

                //If no chunk data available, ignore.
                if (world.GetChunk(chunkPosition.x, chunkPosition.y, chunkPosition.z) == null)
                {
                    //ChunksLoadedVisualizer.SetChunkLoadedState(newChunkPos, false);
                    //Debug.LogError("No world data at " + newChunkPos.ToString());
                    continue;
                }

                ChunkRenderer newChunk;
                chunkRendererMap.TryGetValue(chunkPosition, out newChunk);

                //If the chunk already exists and it's already
                //rendered or in queue to be rendered continue
                if (newChunk != null
                    && (newChunk.rendered))// || updateList.Contains(newChunkPos)))
                {
                    continue;
                }


                //Otherwise let's build it
                buildList.Add(chunkPosition);
            }
        }


        //if (viewDistanceIndex >= renderDistance)
        //    viewDistanceIndex = 0;

        ////TODO Exit out early after iterating a certain amount using viewDistanceIndex

        //int checksPerFrame = 1000;
        //int currentChecks = 0;
        //bool enoughChunksToBuild = false;
        ////If there aren't already chunks to generate
        //if (buildList.Count == 0)
        //{
        //    for (; viewDistanceIndex <= renderDistance && !enoughChunksToBuild; viewDistanceIndex++)
        //    {
        //        for (var x = -viewDistanceIndex; x <= viewDistanceIndex && !enoughChunksToBuild; x++)
        //        {
        //            for (var y = -viewDistanceIndex; y <= viewDistanceIndex && !enoughChunksToBuild; y++)
        //            {
        //                for (var z = -viewDistanceIndex; z <= viewDistanceIndex && !enoughChunksToBuild; z++)
        //                {
        //                    currentChecks++;
        //                    if(currentChecks > checksPerFrame)
        //                    {
        //                        enoughChunksToBuild = true;
        //                    }
        //                    UnityEngine.Profiling.Profiler.BeginSample("FindChunksToLoad Inner");
        //                    UnityEngine.Profiling.Profiler.EndSample();
        //                    //TODO skip these iterations
        //                    if (x > 0 && x < renderDistance && y > 0 && y < renderDistance && z > 0 && z < renderDistance)
        //                        continue;

        //                    WorldPos newChunkPos = new WorldPos(
        //                        x * ChunkInstance.CHUNK_SIZE + playerPos.x,
        //                        y * ChunkInstance.CHUNK_SIZE + playerPos.y,
        //                        z * ChunkInstance.CHUNK_SIZE + playerPos.z
        //                    );

        //                    if (buildList.Contains(newChunkPos))
        //                        continue;

        //                    //If no chunk data available, ignore.
        //                    if (world.GetChunk(newChunkPos.x, newChunkPos.y, newChunkPos.z) == null)
        //                    {
        //                        //ChunksLoadedVisualizer.SetChunkLoadedState(newChunkPos, false);
        //                        //Debug.LogError("No world data at " + newChunkPos.ToString());
        //                        continue;
        //                    }

        //                    ChunkRenderer newChunk;
        //                    chunkRendererMap.TryGetValue(newChunkPos, out newChunk);

        //                    //If the chunk already exists and it's already
        //                    //rendered or in queue to be rendered continue
        //                    if (newChunk != null
        //                        && (newChunk.rendered))// || updateList.Contains(newChunkPos)))
        //                    {
        //                        continue;
        //                    }


        //                    //Otherwise let's build it
        //                    buildList.Add(newChunkPos);
        //                    //updateList.Add(newChunkPos);

        //                    if (buildList.Count > chunkToBuildPerFrame)
        //                    {
        //                        viewDistanceIndex = 0;
        //                        enoughChunksToBuild = true;
        //                    }
        //                }
        //            }
        //        }
        //    }
        //}
    }

    void LoadAndRenderChunks()
    {
        if (buildList.Count != 0)
        {
            for (int i = 0; i < buildList.Count && i < 2; i++)
            {
                BuildChunkRenderer(buildList[0]);
                buildList.RemoveAt(0);
            }
			//If chunks were built return early
			return;

        }

        //if (updateList.Count != 0)
        //{
        //    for (int i = 0; i < updateList.Count && i < 64; i++)
        //    {
        //                 WorldPos pos = updateList[0];
        //                 ChunkRenderer chunkRenderer;
        //                 if (chunkRendererMap.TryGetValue(pos, out chunkRenderer))
        //                 {
        //                     chunkRenderer.chunk.update = true;
        //                 }

        //             	updateList.RemoveAt(0);
        //    }

        //    updateList.Clear();
        //}
    }

    void BuildChunkRenderer(WorldPos pos)
    {
        //Instantiate the chunk at the coordinates using the chunk prefab
        GameObject newChunkObject = Instantiate(
            chunkPrefab, new Vector3(pos.x, pos.y, pos.z),
            Quaternion.Euler(Vector3.zero)
        ) as GameObject;

        ChunkRenderer chunkRenderer = newChunkObject.GetComponent<ChunkRenderer>();
        chunkRenderer.chunk = world.GetChunk(pos.x, pos.y, pos.z);
        if (chunkRenderer.chunk == null)
        {
            Debug.LogError("No chunk at " + pos.x + " " + pos.y + " " + pos.z);
        }
        chunkRendererMap.Add(pos, newChunkObject.GetComponent<ChunkRenderer>());
        chunkRenderers.Add(pos);
        chunkRenderer.rendered = true;
        chunkRenderer.chunk.update = true;
    }

    bool DeleteChunkRenderers()
    {
        if (timer == 20)
        {
            //List<WorldPos> chunkRenderersToDelete = new List<WorldPos>();
            for (int i = 0; i < chunkRenderers.Count; i++)
            {
                WorldPos pos = chunkRenderers[i];
                if(chunkRendererMap[pos].chunk == null) {
                    Destroy(chunkRendererMap[pos].gameObject);
                    chunkRendererMap.Remove(pos);
                    chunkRenderers.RemoveAt(i);
                    i--;
                    continue;
                }

                float distance = Vector3.Distance(
                    new Vector3(pos.x, pos.y, pos.z),
                    new Vector3(_transform.position.x, _transform.position.y, _transform.position.z));

                if (distance > (Mathf.CeilToInt(1.74f * (renderDistance)) + 1) * ChunkInstance.CHUNK_SIZE)
                {
                    Destroy(chunkRendererMap[pos].gameObject);
                    chunkRendererMap.Remove(pos);
                    chunkRenderers.RemoveAt(i);
                    i--;
                    continue;
                }
            }

            //foreach (WorldPos chunkKeys in chunkRenderersToDelete) {
            //    Destroy(chunkRendererMap[chunkKeys].gameObject);
            //    chunkRendererMap.Remove(chunkKeys);
            //}

            timer = 0;
            return true;
        }

        timer++;
        return false;
    }
}