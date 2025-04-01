using System;
using UnityEngine;

public abstract class MultiIngredientDurCookingItem : BaseDurCookingItem {

    [Header("Multi Ingredient")] 
    [SerializeField] private int MaxIngredientCount = 4;
    [SerializeField] private MultiIngredientHalfFoodItem NotCompleteFoodPrefab;

    protected override void Awake() {
        base.Awake();
        this._MaxIngredientCount = this.MaxIngredientCount;
        this._NotCompleteFoodPrefab = this.NotCompleteFoodPrefab;
    }
}

public abstract class StoveMultiIngredientCookingItem : MultiIngredientDurCookingItem { }


