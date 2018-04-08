using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System;
using System.IO.Compression;
using System.Runtime.InteropServices;
//using System.Runtime.Serialization.Formatters.Binary;
//using System.Runtime.Serialization;
//using MessagePack;

public static class Serialization
{
    /* 
         * Region index file format (ri)
         * RegionSizeCubed * (int chunkDataPos, int chunkDataLength)
         * 
         * Region data file format (rd)
         * Compressed chunk data
         */

    private const int ChunksInRegion = 128;
    private const int ChunksInRegionSquared = ChunksInRegion * ChunksInRegion;
    private const int ChunksInRegionCubed = ChunksInRegion * ChunksInRegion * ChunksInRegion;
    private const int RegionSize = ChunksInRegion * ChunkInstance.CHUNK_SIZE;

    private const int IndexFileLength = ChunksInRegionCubed * sizeof(int) * 2;
    private const int IndexFileNull = -1;

    private const int MaxCachedIndexDatas = 6;

    private const string WorldFolder = "World";
    private const string RegionsFolder = "Regions";

    private const string RegionIndexExt = ".ri";
    private const string RegionDataExt = ".rd";

    private static readonly List<Tuple<WorldPos, byte[]>> CachedIndexDatas = new List<Tuple<WorldPos, byte[]>>();

    private static readonly object IndexLockObject = new object();
    private static readonly object DataLockObject = new object();

    public static void SaveChunkInstance(ChunkInstance chunkInstance) {
        if (!chunkInstance.needsSaving) return;
        //TODO Save blockInstanceData to chunkData blockData
        SaveChunk(chunkInstance.chunkData);
		chunkInstance.needsSaving = false;
    }

    public static void SaveChunk(CachedChunk chunkData)
    {
        var region = ChunkToRegion(chunkData.position);
        var regionFilename = Path.Combine(WorldFolder, RegionsFolder, GetRegionFilename(region));
        var indexFile = new FileInfo(regionFilename + RegionIndexExt);
        var dataFile = new FileInfo(regionFilename + RegionDataExt);
        // ReSharper disable once PossibleNullReferenceException
        indexFile.Directory.Create();

        lock (IndexLockObject)
        {
            if (!indexFile.Exists)
                using (var stream = new GZipStream(indexFile.Create(), CompressionMode.Compress))
                {
                    var buffer = new byte[1024];
                    for (var i = 0; i < buffer.Length; i += sizeof(int))
                        BitConverter.GetBytes(IndexFileNull).CopyTo(buffer, i);

                    for (var i = 0; i < IndexFileLength / buffer.Length; i++)
                        stream.Write(buffer, 0, buffer.Length);
                }
        }

        //Compress chunk data
        byte[] compressedChunkData;
        using (var memoryStream = new MemoryStream())
        {
            using (var writer = new BinaryWriter(memoryStream))
            {
                WriteChunk(writer, chunkData);
            }

            compressedChunkData = CompressionHelper.CompressBytes(memoryStream.ToArray());
        }

        var chunkDataPosition = dataFile.Exists ? (int)dataFile.Length : 0;
        var chunkDataLength = compressedChunkData.Length;

        //Append chunk to data file
        lock (DataLockObject)
        {
            using (var stream = dataFile.Open(FileMode.Append, FileAccess.Write))
            {
                stream.Write(compressedChunkData, 0, compressedChunkData.Length);
            }
        }

        //Update chunk index
        var chunkIndexPosition = GetChunkIndexPosition(chunkData.position);

        lock (IndexLockObject)
        {
            var chunkIndexData = GetIndexData(region, indexFile);
            Array.Copy(BitConverter.GetBytes(chunkDataPosition), 0, chunkIndexData, chunkIndexPosition, sizeof(int));
            Array.Copy(BitConverter.GetBytes(chunkDataLength), 0, chunkIndexData, chunkIndexPosition + sizeof(int),
                sizeof(int));
            File.WriteAllBytes(indexFile.FullName, CompressionHelper.CompressBytes(chunkIndexData));
        }

    }

    public static void WriteChunk(BinaryWriter writer, CachedChunk chunkData)
    {
        writer.Write(chunkData.min.x);
        writer.Write(chunkData.min.y);
        writer.Write(chunkData.min.z);

        writer.Write(chunkData.max.x);
        writer.Write(chunkData.max.y);
        writer.Write(chunkData.max.z);

        for (int x = chunkData.min.x, n1 = chunkData.max.x; x <= n1; x++)
            for (int y = chunkData.min.y, n2 = chunkData.max.y; y <= n2; y++)
                for (int z = chunkData.min.z, n3 = chunkData.max.z; z <= n3; z++)
                {
                    int index = x + y * ChunkInstance.CHUNK_SIZE + z * ChunkInstance.CHUNK_SIZE * ChunkInstance.CHUNK_SIZE;
                    writer.Write(chunkData.blockIds[index]);
                    //writer.Write(_lightLevels[x, y, z].Binary);
                }

        //writer.Write(chunkInstance.blockIds.Length);
        //foreach (var data in chunkInstance.blockInstanceData)
        //{
        //    writer.Write(data.Key.Binary);
        //    BlockData.WriteToStream(data.Value, writer);
        //}
    }

    public static CachedChunk LoadChunk(World world, WorldPos chunkPos)
    {
        //var stringBuilder = new System.Text.StringBuilder();
        //stringBuilder. 
        //TODO maybe change to stringbuilder
        WorldPos region = ChunkToRegion(chunkPos);
        string regionFilename = Path.Combine(WorldFolder, RegionsFolder, GetRegionFilename(region));
        FileInfo indexFile = new FileInfo( regionFilename + RegionIndexExt);
        FileInfo dataFile = new FileInfo(regionFilename + RegionDataExt);

        if (!indexFile.Exists || !dataFile.Exists) return null;

        //Get chunk data position and length
        int chunkDataPosition, chunkDataLength;
        var chunkIndexPosition = GetChunkIndexPosition(chunkPos);

        lock (IndexLockObject)
        {
            var chunkIndexData = GetIndexData(region, indexFile);
            chunkDataPosition = BitConverter.ToInt32(chunkIndexData, chunkIndexPosition);
            chunkDataLength = BitConverter.ToInt32(chunkIndexData, chunkIndexPosition + sizeof(int));
        }

        if (chunkDataPosition == IndexFileNull || chunkDataLength == IndexFileNull) return null;
        //Read chunk data
        byte[] chunkData;

        lock (DataLockObject)
        {
            using (var reader = new BinaryReader(dataFile.OpenRead()))
            {
                reader.BaseStream.Seek(chunkDataPosition, SeekOrigin.Begin);
                chunkData = CompressionHelper.DecompressBytes(reader.ReadBytes(chunkDataLength));
            }
        }

        using (var reader = new BinaryReader(new MemoryStream(chunkData)))
        {
            return new CachedChunk(world, chunkPos, reader);
        }
    }

    private static string GetRegionFilename(WorldPos region) => $"{region.x} {region.y} {region.z}";

    private static byte[] GetIndexData(WorldPos region, FileInfo indexFile)
    {
        foreach (var cachedIndexData in CachedIndexDatas)
            if (cachedIndexData.Item1 == region) 
                return cachedIndexData.Item2;

        var data = CompressionHelper.DecompressBytes(File.ReadAllBytes(indexFile.FullName));
        CachedIndexDatas.Add(new Tuple<WorldPos, byte[]>(region, data));

        if (CachedIndexDatas.Count > MaxCachedIndexDatas)
            CachedIndexDatas.RemoveAt(0);

        return data;
    }

    private static int GetChunkIndexPosition(WorldPos chunkPos)
    {
        var chunkInRegion = ChunkInRegion(chunkPos);
        return (ChunksInRegionSquared * chunkInRegion.x + ChunksInRegion * chunkInRegion.y + chunkInRegion.z) *
               sizeof(int) * 2;
    }

    private static WorldPos ChunkToRegion(WorldPos v) => new WorldPos(
        v.x * ChunkInstance.CHUNK_SIZE < 0 ? (v.x * ChunkInstance.CHUNK_SIZE + 1) / RegionSize - 1 : v.x / ChunksInRegion,
        v.y * ChunkInstance.CHUNK_SIZE < 0 ? (v.y * ChunkInstance.CHUNK_SIZE + 1) / RegionSize - 1 : v.y / ChunksInRegion,
        v.z * ChunkInstance.CHUNK_SIZE < 0 ? (v.z * ChunkInstance.CHUNK_SIZE + 1) / RegionSize - 1 : v.z / ChunksInRegion);

    private static WorldPos ChunkInRegion(WorldPos v) => new WorldPos(
        v.x < 0 ? (v.x + 1) % ChunksInRegion + ChunksInRegion - 1 : v.x % ChunksInRegion,
        v.y < 0 ? (v.y + 1) % ChunksInRegion + ChunksInRegion - 1 : v.y % ChunksInRegion,
        v.z < 0 ? (v.z + 1) % ChunksInRegion + ChunksInRegion - 1 : v.z % ChunksInRegion);




//    public static string saveFolderName = "voxelGameSaves";

//    public static string SaveLocation(string worldName)
//    {
//        string saveLocation = saveFolderName + "/" + worldName + "/";

//        if (!Directory.Exists(saveLocation))
//        {
//            Directory.CreateDirectory(saveLocation);
//        }

//        return saveLocation;
//    }

//    public static string FileName(WorldPos chunkLocation)
//    {
//        string fileName = chunkLocation.x + "," + chunkLocation.y + "," + chunkLocation.z + ".bin";
//        return fileName;
//    }

//	public static void SavePlayer(PlayerData player) {
//		PlayerSaveData save = new PlayerSaveData(player);

//		string saveFile = SaveLocation(player.playerName) + ".bin";
////		saveFile += FileName(chunk.pos);

////		IFormatter formatter = new BinaryFormatter();
////		Stream stream = new FileStream(saveFile, FileMode.Create, FileAccess.Write, FileShare.None);
////		formatter.Serialize(stream, save);
////		stream.Close();
//	}

//	public static bool LoadPlayer(PlayerData player) {
//		string saveFile = SaveLocation(player.playerName) + ".bin";

//		if (!File.Exists(saveFile))
//			return false;

////		IFormatter formatter = new BinaryFormatter();
////		FileStream stream = new FileStream(saveFile, FileMode.Open);
////
////		PlayerSaveData save = (PlayerSaveData)formatter.Deserialize(stream);
////
////		player.transform.position = save.position;
////		player.GetComponent<PlayerInventory>().inventory.items = save.inventory;
////		player.GetComponent<PlayerInventory>().equipment.items = save.equipment;
////
////		stream.Close();
//		return true;
//	}

//    public static void SaveChunk(ChunkInstance chunk)
//    {
//        ChunkSaveData save = new ChunkSaveData(chunk);
//        if (save.blocks.Count == 0)
//            return;

//        string saveFile = SaveLocation(chunk.world.worldName);
//        saveFile += FileName(chunk.position);

////        IFormatter formatter = new BinaryFormatter();
//        Stream stream = new FileStream(saveFile, FileMode.Create, FileAccess.Write, FileShare.None);
//        LZ4MessagePackSerializer.Serialize<ChunkSaveData>(stream, save);
//        stream.Close();
//    }

//    public static bool LoadChunk(ChunkInstance chunk)
//    {
//        string saveFile = SaveLocation(chunk.world.worldName);
//        saveFile += FileName(chunk.position);

//        if (!File.Exists(saveFile))
//            return false;

////        IFormatter formatter = new BinaryFormatter();
    //    FileStream stream = new FileStream(saveFile, FileMode.Open);

    //    ChunkSaveData save = LZ4MessagePackSerializer.Deserialize<ChunkSaveData>(stream);
       
    //    foreach (var block in save.blocks)
    //    {
    //        chunk.blocks[block.Key.x, block.Key.y, block.Key.z] = block.Value;
    //    }

    //    stream.Close();
    //    return true;
    //}
}