using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecipeManager : MonoBehaviour {

	static RecipeManager instance;

	Dictionary<string, Item> shapedRecipes;
	Dictionary<string, Item> shapelessRecipes;

	void Awake () {
		if(instance != null) {
			Debug.LogError("Already instance of RecipeManager in scene.");
			this.enabled = false;
			return;
		}
		instance = this;
		shapedRecipes = new Dictionary<string, Item>();
		shapelessRecipes = new Dictionary<string, Item>();
	}

	void Start() {
		InitRecipes();
	}

	void InitRecipes() {
		RecipeManager.AddShapelessRecipe(new int[] {2}, ItemLoader.CreateItem(5), 4); //Log to planks
		RecipeManager.AddShapelessRecipe(new int[] {5}, ItemLoader.CreateItem(6), 4); //Plank to stick
		RecipeManager.AddShapelessRecipe(new int[] {3, 3}, ItemLoader.CreateItem(6), 1); //Twig to stick
//		RecipeManager.AddShapedRecipe(new int[] {0, -1, 0, 0}, ItemLoader.GetItemData(1)); //Dirt to rock
	}

	static string MakeKey(int[] itemIDs) {
		string key = "";
		for(int i = 0; i < itemIDs.Length; i++) {
			string val = itemIDs[i] < 0 ? "X" : itemIDs[i].ToString(); 
			if(i < itemIDs.Length - 1) {
				key += val + "-";
			} else {
				key += val;
			}
		}

		return key;
	}

	public static void AddShapedRecipe(int[] itemIDs, Item item, int amount = 1) {
		if(!(itemIDs.Length == 9 || itemIDs.Length == 4)) {
			Debug.LogError("Must be 4 or 9");
			return;
		}

		string key = MakeKey(itemIDs);

		item.amount = amount;
		instance.shapedRecipes.Add(key, item);
	}

	public static void AddShapelessRecipe(int[] itemIDs, Item item, int amount = 1) {
//		if(!(itemIDs.Length == 9 || itemIDs.Length == 4)) {
//			Debug.LogError("Must be 4 or 9");
//			return;
//		}

		string key = MakeKey(itemIDs);

		item.amount = amount;
		instance.shapelessRecipes.Add(key, item);
	}


	/// <summary>
	/// Checks the recipe. Must be 4 or 9 itemIDs
	/// </summary>
	/// <returns><c>true</c>, if recipe was checked, <c>false</c> otherwise.</returns>
	/// <param name="itemIDs">Item I ds.</param>
	/// <param name="item">Item.</param>
	public static bool CheckRecipe(int[] itemIDs, out Item item) {
		item = null;
		if(!(itemIDs.Length == 9 || itemIDs.Length == 4)) {
			Debug.LogError("Must be 4 or 9");
			return false;
		}

		string key = MakeKey(itemIDs);

		if(instance.shapedRecipes.TryGetValue(key, out item)) {
			return true;
		} 

		key = "";
		for(int i = 0; i < itemIDs.Length; i++) {
			if(itemIDs[i] > 0)
				key += (key.Length > 0 ? "-" : "") + itemIDs[i];
		}

		if(instance.shapelessRecipes.TryGetValue(key, out item)) {
			return true;
		} 

		return false;
	}
}
