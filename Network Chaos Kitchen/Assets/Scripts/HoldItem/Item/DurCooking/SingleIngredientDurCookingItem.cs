
using UnityEngine;

public abstract class SingleIngredientDurCookingItem : BaseDurCookingItem {

    protected override void Awake() {
        base.Awake();
        this._MaxIngredientCount = 1;
    }

    protected override void GetItem(IItem item) {
        if (item is HasOpFoodItem food) {
            SingleIngredientDurCookingOperation currentOperation = food.GetCookOp<SingleIngredientDurCookingOperation>(GetItemCookingOp());
            this._NotCompleteFoodPrefab = currentOperation.NotCompleteFoodPrefab;
        }
        base.GetItem(item);
    }
}

public abstract class StoveSingleIngredientCookingItem : SingleIngredientDurCookingItem { }
