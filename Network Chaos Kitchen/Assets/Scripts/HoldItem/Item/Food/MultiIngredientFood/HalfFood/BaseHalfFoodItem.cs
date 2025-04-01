using UnityEngine;

public abstract class BaseHalfFoodItem : MultiIngredientFoodItem {
    
    [Header("Half Food")]
    [SerializeField] private OvercookedFoodItem OvercookedFoodPrefab;

    protected FoodItem _CompleteFoodPrefab;
    private CookOP CookingOperation;
    
    public float TotalCookingTime { get; private set; }
    public bool IsComplete { get; private set; }
    
    private float AlreadyCookedTime;
    private float AlreadyOvercookedTime;

    protected int FinalFoodCode;

    public void SetCookingOP(CookOP op) {
        this.CookingOperation = op;
    }

    public float GetAlreadyCookedTime() {
        return this.AlreadyCookedTime;
    }

    public float GetAlreadyOvercookedTime() {
        return this.AlreadyOvercookedTime;
    }
    
    public void SaveCookedTime(float time) {
        this.AlreadyCookedTime = Mathf.Max(time, this.AlreadyCookedTime);
    }

    public void SaveOvercookedTime(float time) {
        this.AlreadyOvercookedTime = Mathf.Max(time, this.AlreadyOvercookedTime);
    }

    protected abstract void UpdateCompleteFoodPrefab();

    protected override void AddIngredient(HasOpFoodItem ingredient) {
        base.AddIngredient(ingredient);
        IsComplete = false;
        this.AlreadyOvercookedTime = 0.0f;
        BaseDurCookingOperation cookingOperation = ingredient.GetCookOp<BaseDurCookingOperation>(CookingOperation);
        if (cookingOperation is MultiIngredientDurCookingOperation op) {
            RecipeManager.Instance.UpdateCode(ref this.FinalFoodCode, op.CookingResultFoodSort);
        }
        this.TotalCookingTime += cookingOperation.DurCookingTime;
    }

    protected override void AddIngredient(MultiIngredientFoodItem other) {
        base.AddIngredient(other);
        if (other is not BaseHalfFoodItem halfFood) return;
        IsComplete = false;
        this.AlreadyOvercookedTime = halfFood.AlreadyOvercookedTime;
        this.AlreadyCookedTime += halfFood.AlreadyCookedTime;
        this.TotalCookingTime += halfFood.TotalCookingTime;
        RecipeManager.Instance.MergeFoodCode(ref this.FinalFoodCode, halfFood.FinalFoodCode);
    }

    public IItem FoodComplete() {
        IsComplete = true;
        UpdateCompleteFoodPrefab();
        return FoodChange(this._CompleteFoodPrefab);
    }

    public IItem FoodOvercooked() {
        return FoodChange(this.OvercookedFoodPrefab);
    }

    private IItem FoodChange(FoodItem foodPrefab) {
        if (foodPrefab is null) {
            return this;
        }
        FoodItem nextItem = Instantiate(foodPrefab, this.transform.parent);
        nextItem.GetTransform().localPosition = this.transform.localPosition;
        nextItem.SetFoodList(this);
        if (nextItem is MultiIngredientFoodItem food) {
            food.SetIngredients(this.FoodCode, this.IngredientCount);
        }
        Destroy(this.gameObject);
        return nextItem;
    }
}



