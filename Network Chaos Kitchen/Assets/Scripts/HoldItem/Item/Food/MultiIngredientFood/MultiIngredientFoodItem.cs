
using System.Collections.Generic;

public class MultiIngredientFoodItem : FoodItem {
    
    protected int IngredientCount;
    protected int FoodCode;
    
    public List<HasOpFoodItem> Foods { get; private set; }

    public int GetIngredientCount() {
        return this.IngredientCount;
    }

    public int GetFoodCode() {
        return this.FoodCode;
    }

    public void SetIngredients(int foodCode, int ingredientCount) {
        this.IngredientCount = ingredientCount;
        this.FoodCode = foodCode;
    }

    public void AddIngredient(FoodItem foodItem) {
        this.Foods ??= new List<HasOpFoodItem>();
        switch (foodItem) {
            case PlateFoodItem plateFood:
                plateFood.Foods.ForEach(AddIngredient);
                break;
            case MultiIngredientFoodItem halfFood:
                AddIngredient(halfFood);
                break;
            case HasOpFoodItem opFood:
                AddIngredient(opFood);
                break;
        }
        AddFoodListFromOther(foodItem);
    }
    
    protected virtual void AddIngredient(HasOpFoodItem other) {
        IngredientCount += 1;
        RecipeManager.Instance.UpdateCode(ref this.FoodCode, other.ItemInfo.FoodSort);
        this.Foods.Add(other);
    }

    protected virtual void AddIngredient(MultiIngredientFoodItem other) {
        IngredientCount += other.IngredientCount;
        RecipeManager.Instance.MergeFoodCode(ref this.FoodCode, other.FoodCode);
        this.Foods.AddRange(other.Foods);
    }

    public bool HasOp(CookOP op) {
        return this.Foods.Find(f => !f.HasOp(op)) is null;
    }
}

