
using UnityEngine;

public class MultiIngredientHalfFoodItem : BaseHalfFoodItem {

    private MultiIngredientHalfFoodVF HalfFoodVf;
    
    private void Awake() {
        this.HalfFoodVf = GetComponent<MultiIngredientHalfFoodVF>();
    }

    protected override void AddIngredient(HasOpFoodItem ingredient) {
        base.AddIngredient(ingredient);
        this.HalfFoodVf?.AddIngredient(ingredient.ItemInfo.FoodSort);
    }

    protected override void AddIngredient(MultiIngredientFoodItem other) {
        base.AddIngredient(other);
        foreach (HasOpFoodItem food in other.Foods) {
            this.HalfFoodVf?.AddIngredient(food.ItemInfo.FoodSort);
        }
    }

    protected override void UpdateCompleteFoodPrefab() {
        RecipeSO recipe = RecipeManager.Instance.GetCompleteRecipe(this.FinalFoodCode);
        if (recipe != null && recipe.RecipeFoods.Length == GetIngredientCount()) {
            this._CompleteFoodPrefab = recipe.FinalFoodPrefab;
        } else {
            this.FoodCode = this.FinalFoodCode;
        }
    }
}

