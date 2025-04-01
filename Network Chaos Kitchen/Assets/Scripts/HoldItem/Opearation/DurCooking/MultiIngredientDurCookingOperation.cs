
using UnityEngine;

public abstract class MultiIngredientDurCookingOperation : BaseDurCookingOperation {

    [field : SerializeField] public FoodSort CookingResultFoodSort { get; private set; }

    public abstract override CookOP GetCookOP();
}
