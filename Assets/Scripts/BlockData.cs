using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BlockData {

	public enum HardnessTier {
		Wood = 0,
		Stone = 1,
		Copper = 2,
		Iron = 3,
	}

	[System.Serializable]
    public struct TexturePosition { 
        public TexturePosition(int x, int y) {
            this.x = x; this.y = y;
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

	public int ID;
    public string name;
	public HardnessTier hardnessTier = HardnessTier.Wood;
	public int brightness = 0;


    //TODO idk maybe optimize this somehow by not cluttering this objs size. And or exclude from build
    public Texture2D eastTexture;
    public Texture2D westTexture; 
    public Texture2D topTexture; 
    public Texture2D bottomTexture; 
    public Texture2D frontTexture;
    public Texture2D backTexture; 

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
