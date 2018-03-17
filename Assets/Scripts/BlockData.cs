using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[System.Serializable]
public class BlockData {

	public enum HardnessTier {
		Wood = 0,
		Stone = 1,
		Copper = 2,
		Iron = 3,
	}

    public enum Direction { Up, Down, East, West, North, South, };

	[System.Serializable]
    public struct TexturePosition { 
        public TexturePosition(int x, int y) {
            this.x = x; this.y = y;
        }
        public TexturePosition(Vector2Int vec2) {
            this.x = vec2.x; this.y = vec2.y;
        }
        public int x; public int y;
    } //TODO idk rename maybe
    public TexturePosition[] texturePosition;
    public bool[] solid;
//    public bool downSolid;
//    public bool eastSolid;
//    public bool westSolid;
//    public bool frontSolid;
//    public bool backSolid;

	public ushort ID;
    public string name;
	public HardnessTier hardnessTier = HardnessTier.Wood;
	public int brightness = 0;

    public static float TILE_SIZE = 1f / 64f;


    //TODO idk maybe optimize this somehow by not cluttering this objs size. And or exclude from build
    public Texture2D[] textures;
//    public Texture2D westTexture; 
//    public Texture2D topTexture; 
//    public Texture2D bottomTexture; 
//    public Texture2D frontTexture;
//    public Texture2D backTexture; 

	//TODO Func
	//	IO (Logic)
	//	Storage 
	//	GUI
	//	Grow
    //      Timing
    //      Free space

    //These should maybe be inherited or interfaces
	//TODO sounds
    //public AudioClip jumpSound;
    //public AudioClip hitSound;
    //public AudioClip breakSound;
    //public AudioClip interactSound;


	//TODO Flow (Speed) 0-100 [gravity=10]
    //Uuuuuuuuuuh....

	//TODO Heat (Temp) in C or F Adjusted in settings.
    //Is heat transfered by averaging neighbors? Maybe just a heat var on all blocks that heat emitters uses Func to affect. 
    //Kind of like the idea of blocks having a melting point.

	//TODO Power (Volts) 
    //Should all blocks be conductive? Only certain blocks conductive? 
    //Power distro, recieving, and storage should be inheritance or interface? Maybe a part of Func

	//TODO Multiblock Structure...
    //No idea GL zack

	//TODO Player Phys
    //Probably a collection of predefined statuses that player can poll beneath them to recieve. Will keep it cleaner per block

    [System.Serializable]
    public struct DropData
    {
        public int itemID;
        public int amount;
        public float percentChance; //Out of 100f
    }
    public DropData[] drops;

    internal static void WriteToStream(BlockInstanceData blockData, BinaryWriter writer)
    {
        //byte[] bytes;
        //using (var ms = new MemoryStream())
        //{
        //    using (var bw = new BinaryWriter(ms))
        //        try
        //        {
        //            blockData.Serialize(bw);
        //        }
        //        catch (Exception e)
        //        {
        //            Logger.Error("Error during serialization of " + blockData);
        //            Logger.Exception(e);
        //        }

        //    bytes = ms.ToArray();
        //}

        //writer.Write(GameRegistry.GetBlockDataRegistryKey(blockData));
        //writer.Write((ushort)bytes.Length);
        //writer.Write(bytes);
    }

    internal static BlockData ReadFromStream(BinaryReader reader)
    {
        //var blockDataRegistryKey = reader.ReadString();
        //var bytesLength = reader.ReadUInt16();
        //var bytes = reader.ReadBytes(bytesLength);

        //var entry = GameRegistry.BlockDataRegistry[blockDataRegistryKey];
        //var blockData = (BlockData)Activator.CreateInstance(entry.Type);

        //using (var ms = new MemoryStream(bytes))
        //using (var br = new BinaryReader(ms))
        //    blockData.Deserialize(br);

        //return blockData;

        return null;
    }

    //public bool IsSolid(Direction direction)
    //{
    //    return solid[(int)direction];
    //}

    public virtual MeshData GetBlockdata
     (ChunkInstance chunk, int x, int y, int z, MeshData meshData)
    {
        meshData.useRenderDataForCol = true;

        if (!BlockLoader.GetBlock(chunk.GetBlock(x, y + 1, z)).solid[(int)Direction.Down])
        {
            meshData = FaceDataUp(chunk, x, y, z, meshData);
        }

        if (!BlockLoader.GetBlock(chunk.GetBlock(x, y - 1, z)).solid[(int)Direction.Up])
        {
            meshData = FaceDataDown(chunk, x, y, z, meshData);
        }

        if (!BlockLoader.GetBlock(chunk.GetBlock(x, y, z + 1)).solid[(int)Direction.South])
        {
            meshData = FaceDataNorth(chunk, x, y, z, meshData);
        }

        if (!BlockLoader.GetBlock(chunk.GetBlock(x, y, z - 1)).solid[(int)Direction.North])
        {
            meshData = FaceDataSouth(chunk, x, y, z, meshData);
        }

        if (!BlockLoader.GetBlock(chunk.GetBlock(x + 1, y, z)).solid[(int)Direction.West])
        {
            meshData = FaceDataEast(chunk, x, y, z, meshData);
        }

        if (!BlockLoader.GetBlock(chunk.GetBlock(x - 1, y, z)).solid[(int)Direction.East])
        {
            meshData = FaceDataWest(chunk, x, y, z, meshData);
        }

        return meshData;
    }

    protected virtual MeshData FaceDataUp
        (ChunkInstance chunk, int x, int y, int z, MeshData meshData)
    {
        meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z + 0.5f));
        meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z + 0.5f));
        meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z - 0.5f));
        meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z - 0.5f));

        meshData.AddQuadTriangles();
        meshData.AddUVs(FaceUVs(Direction.Up));
        return meshData;
    }

    protected virtual MeshData FaceDataDown
        (ChunkInstance chunk, int x, int y, int z, MeshData meshData)
    {
        meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z - 0.5f));
        meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z - 0.5f));
        meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z + 0.5f));
        meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z + 0.5f));

        meshData.AddQuadTriangles();
        meshData.AddUVs(FaceUVs(Direction.Down));
        return meshData;
    }

    protected virtual MeshData FaceDataNorth
        (ChunkInstance chunk, int x, int y, int z, MeshData meshData)
    {
        meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z + 0.5f));
        meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z + 0.5f));
        meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z + 0.5f));
        meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z + 0.5f));

        meshData.AddQuadTriangles();
        meshData.AddUVs(FaceUVs(Direction.North));
        return meshData;
    }

    protected virtual MeshData FaceDataEast
        (ChunkInstance chunk, int x, int y, int z, MeshData meshData)
    {
        meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z - 0.5f));
        meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z - 0.5f));
        meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z + 0.5f));
        meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z + 0.5f));

        meshData.AddQuadTriangles();
        meshData.AddUVs(FaceUVs(Direction.East));
        return meshData;
    }

    protected virtual MeshData FaceDataSouth
        (ChunkInstance chunk, int x, int y, int z, MeshData meshData)
    {
        meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z - 0.5f));
        meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z - 0.5f));
        meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z - 0.5f));
        meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z - 0.5f));

        meshData.AddQuadTriangles();
        meshData.AddUVs(FaceUVs(Direction.South));
        return meshData;
    }

    protected virtual MeshData FaceDataWest
        (ChunkInstance chunk, int x, int y, int z, MeshData meshData)
    {
        meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z + 0.5f));
        meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z + 0.5f));
        meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z - 0.5f));
        meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z - 0.5f));

        meshData.AddQuadTriangles();
        meshData.AddUVs(FaceUVs(Direction.West));

        return meshData;
    }

    public virtual TexturePosition GetTexturePosition(Direction direction)
    {
        //        Tile tile = new Tile();
        //        tile.x = 0;
        //        tile.y = 0;
        //
        //        return tile;
        //
        //
        if (BlockLoader.GetBlock(ID).texturePosition != null && BlockLoader.GetBlock(ID).texturePosition.Length > (int)direction)
            return BlockLoader.GetBlock(ID).texturePosition[(int)direction];
        else
            return new TexturePosition(0, 0);
        //        return BlockDatabase.GetBlock(ID).texturePosition[(int)direction];
    }

    public virtual Vector2[] FaceUVs(Direction direction)
    {
        Vector2[] UVs = new Vector2[4];
        BlockData.TexturePosition tilePos = GetTexturePosition(direction);

        UVs[0] = new Vector2(TILE_SIZE * tilePos.x + TILE_SIZE,
                             TILE_SIZE * tilePos.y);
        UVs[1] = new Vector2(TILE_SIZE * tilePos.x + TILE_SIZE,
                             TILE_SIZE * tilePos.y + TILE_SIZE);
        UVs[2] = new Vector2(TILE_SIZE * tilePos.x,
                             TILE_SIZE * tilePos.y + TILE_SIZE);
        UVs[3] = new Vector2(TILE_SIZE * tilePos.x,
                             TILE_SIZE * tilePos.y);

        return UVs;
    }

    public void Break(Vector3 pos)
    {
        for (int i = 0; i < drops.Length; i++)
        {
            float roll = Random.Range(0f, 100f);
            if (roll <= drops[i].percentChance) //Is this right?
            {
                for (int j = 0; j < drops[i].amount; j++)
                {
                    Vector2 xyDir = Random.insideUnitCircle.normalized * 0.25f;
                    Vector3 direction = new Vector3(xyDir.x, 0f, xyDir.y);
                    GameObject obj = ItemLoader.CreateModel(drops[i].itemID);
                    obj.transform.position = pos + direction;
                    obj.GetComponent<Rigidbody>().AddForce(direction * Random.Range(40f, 60f) + Vector3.up * Random.Range(80f, 110f));
                }
            }
        }
    }


    //TODO move to chunk.cs
    //	public float maxHealth = 100f;
    //	[System.NonSerialized]
    //	public float currentHealth;
    //
    //	float healTimer = 0f;
    //
    //
    //	void Start () {
    //		currentHealth = maxHealth;
    //	}
    //	
    //	public void BlockUpdate () {
    //		if(healTimer > 0) {
    //			healTimer -= Time.deltaTime;
    //			if(healTimer <= 0f) {
    //				currentHealth = maxHealth;
    //			}
    //			foreach(Renderer rend in GetComponentsInChildren<Renderer>()) {
    //				rend.material.color = Color.Lerp(Color.red, Color.white, currentHealth/maxHealth);
    //			}
    //		}
    //	}
    //
    //	/// <summary>
    //	/// Damages block. Returns true if broken.
    //	/// </summary>
    //	/// <param name="damage">Damage.</param>
    //	public bool Damage(float damage) {
    //		healTimer = 1f;
    //		currentHealth -= damage;
    //		return (currentHealth <= 0f);
    //	}
}
