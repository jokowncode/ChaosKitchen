
using UnityEngine;

public class SingleIngredientHalfFoodItem : BaseHalfFoodItem {
    
    [SerializeField] private FoodItem CompleteFoodPrefab;

    protected override void UpdateCompleteFoodPrefab() {
        this._CompleteFoodPrefab = this.CompleteFoodPrefab;
    }
}

