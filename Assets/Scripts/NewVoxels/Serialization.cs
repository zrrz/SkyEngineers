using UnityEngine;
using System.Collections;
using System.IO;
using System;
//using System.Runtime.Serialization.Formatters.Binary;
//using System.Runtime.Serialization;
using MessagePack;

public static class Serialization
{
    public static string saveFolderName = "voxelGameSaves";

    public static string SaveLocation(string worldName)
    {
        string saveLocation = saveFolderName + "/" + worldName + "/";

        if (!Directory.Exists(saveLocation))
        {
            Directory.CreateDirectory(saveLocation);
        }

        return saveLocation;
    }

    public static string FileName(WorldPos chunkLocation)
    {
        string fileName = chunkLocation.x + "," + chunkLocation.y + "," + chunkLocation.z + ".bin";
        return fileName;
    }

	public static void SavePlayer(PlayerData player) {
		PlayerSaveData save = new PlayerSaveData(player);

		string saveFile = SaveLocation(player.playerName) + ".bin";
//		saveFile += FileName(chunk.pos);

//		IFormatter formatter = new BinaryFormatter();
//		Stream stream = new FileStream(saveFile, FileMode.Create, FileAccess.Write, FileShare.None);
//		formatter.Serialize(stream, save);
//		stream.Close();
	}

	public static bool LoadPlayer(PlayerData player) {
		string saveFile = SaveLocation(player.playerName) + ".bin";

		if (!File.Exists(saveFile))
			return false;

//		IFormatter formatter = new BinaryFormatter();
//		FileStream stream = new FileStream(saveFile, FileMode.Open);
//
//		PlayerSaveData save = (PlayerSaveData)formatter.Deserialize(stream);
//
//		player.transform.position = save.position;
//		player.GetComponent<PlayerInventory>().inventory.items = save.inventory;
//		player.GetComponent<PlayerInventory>().equipment.items = save.equipment;
//
//		stream.Close();
		return true;
	}

    public static void SaveChunk(ChunkInstance chunk)
    {
        ChunkSaveData save = new ChunkSaveData(chunk);
        if (save.blocks.Count == 0)
            return;

        string saveFile = SaveLocation(chunk.world.worldName);
        saveFile += FileName(chunk.position);

//        IFormatter formatter = new BinaryFormatter();
        Stream stream = new FileStream(saveFile, FileMode.Create, FileAccess.Write, FileShare.None);
        LZ4MessagePackSerializer.Serialize<ChunkSaveData>(stream, save);
        stream.Close();
    }

    public static bool LoadChunk(ChunkInstance chunk)
    {
        string saveFile = SaveLocation(chunk.world.worldName);
        saveFile += FileName(chunk.position);

        if (!File.Exists(saveFile))
            return false;

//        IFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(saveFile, FileMode.Open);

        ChunkSaveData save = LZ4MessagePackSerializer.Deserialize<ChunkSaveData>(stream);
       
        foreach (var block in save.blocks)
        {
            chunk.blocks[block.Key.x, block.Key.y, block.Key.z] = block.Value;
        }

        stream.Close();
        return true;
    }
}