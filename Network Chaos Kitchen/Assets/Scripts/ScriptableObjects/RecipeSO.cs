
using UnityEngine;

[System.Serializable]
public struct RecipeFood {
    public Sprite OriginFoodSprite;
    public FoodSort CurrentFoodSort;
    public CookOP Op;
}

[CreateAssetMenu(fileName = "Recipe", menuName = "Chaos Kitchen/Recipe")]
public class RecipeSO : ScriptableObject {
    public FinalFoodItem FinalFoodPrefab;
    public RecipeFood[] RecipeFoods;
    public int BaseFee;
    public float PrepareTime;
}
