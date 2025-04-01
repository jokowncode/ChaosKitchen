using Unity.VisualScripting;
using UnityEngine;

public class CleanPlateItem : PlateItem {

    [Header("Plate")]
    [SerializeField] private int MaxPlateFoodCount = 4;

    public RecipeType CurrentCompleteRecipe { get; private set; }
    
    protected override bool ValidateItem(IItem item) {
        if (item is ToolItem) return false;
        if (this.HasObject) {
            if (this.HoldItem is OvercookedFoodItem) return false;
            if (this.HoldItem is BaseHalfFoodItem { IsComplete: false } halfFood && halfFood.GetAlreadyCookedTime() != 0) return false;
        }

        if (item is FinalFoodItem or OvercookedFoodItem) {
            return this.HoldItem is null;
        }

        int currentFoodCount = 0;
        if (this.HasObject) {
            currentFoodCount = this.HoldItem is MultiIngredientFoodItem multiIngredientFood ?
                multiIngredientFood.GetIngredientCount() : 1;
        }
        
        if (item is BaseHalfFoodItem half) {
            switch (this.HoldItem) {
                case BaseHalfFoodItem currentHold
                    when (half.IsComplete != currentHold.IsComplete 
                          || (!half.IsComplete && half.GetAlreadyCookedTime() != 0.0f)):
                case FinalFoodItem when !half.IsComplete:
                    return false;
                default:
                    return half.GetIngredientCount() + currentFoodCount <= MaxPlateFoodCount
                           && (half.IsComplete || half.GetAlreadyCookedTime() == 0.0f || currentFoodCount == 0);
            }
        }

        if (item is HasOpFoodItem hasOpFood) {
            return currentFoodCount < MaxPlateFoodCount && hasOpFood.HasOp(CookOP.Plate);
        }
        return false;
    }

    private void UpdateIngredients(FoodItem food) {
        MultiIngredientFoodItem holdItem = (MultiIngredientFoodItem)this.HoldItem;
        holdItem.AddIngredient(food);
        food.transform.parent = holdItem.transform;
        food.transform.localPosition = Vector3.zero;
    }

    private void CheckLevelRecipe() {
        MultiIngredientFoodItem holdItem = (MultiIngredientFoodItem)this.HoldItem;
        RecipeSO recipe = RecipeManager.Instance.GetCompleteRecipe(holdItem.GetFoodCode());
        if (recipe == null || recipe.RecipeFoods.Length != holdItem.GetIngredientCount()) {
            CurrentCompleteRecipe = RecipeType.None;
        } else {
            FinalFoodItem finalFood = Instantiate(recipe.FinalFoodPrefab, this.Holder);
            CurrentCompleteRecipe = finalFood.Type;
            finalFood.transform.localPosition = Vector3.zero;
            finalFood.SetFoodList(holdItem);
            finalFood.SetIngredients(holdItem.GetFoodCode(), holdItem.GetIngredientCount());
            Destroy(holdItem.gameObject);
            this.HoldItem = finalFood;
        } 
    }

    public override bool GetObject(IItem item) {
        if (!ValidateItem(item)) return false;
        if (this.HoldItem is null) {
            GetItem(item);
            CurrentCompleteRecipe = item is FinalFoodItem finalFood ? finalFood.Type : RecipeType.None;
            return true;
        }

        if (this.HoldItem is not MultiIngredientFoodItem) {
            PlateFoodItem food = new GameObject("PlateItem", typeof(PlateFoodItem)).GetComponent<PlateFoodItem>();
            food.transform.parent = this.Holder;
            food.transform.localPosition = Vector3.zero;
            FoodItem preFood = (FoodItem)this.HoldItem;
            this.HoldItem = food;
            UpdateIngredients(preFood);
        }
        UpdateIngredients((FoodItem)item);
        CheckLevelRecipe();
        return true;
    }
}
