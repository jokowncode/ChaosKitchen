
using UnityEngine;

public abstract class SingleIngredientDurCookingOperation : BaseDurCookingOperation {
    [field : SerializeField] public SingleIngredientHalfFoodItem NotCompleteFoodPrefab { get; private set; }
    
    public abstract override CookOP GetCookOP();
}
