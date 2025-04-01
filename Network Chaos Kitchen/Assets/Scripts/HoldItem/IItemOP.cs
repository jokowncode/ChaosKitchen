
using UnityEngine;

public abstract class BaseCookOP : MonoBehaviour {
    public abstract CookOP GetCookOP();

    protected FoodItem GetFoodItemByPrefab(FoodItem foodPrefab) {
        FoodItem nextItem = Instantiate(foodPrefab, this.transform.parent);
        nextItem.transform.localPosition = this.transform.localPosition;
        Destroy(this.gameObject);
        return nextItem;
    }
}

public abstract class NextStatusCookOp : BaseCookOP {
    [Header("Next Status")] 
    [SerializeField] private FoodItem NextStatusPrefab;

    public IItem GetNextStatusItem(FoodItem originItem) {
        FoodItem item = GetFoodItemByPrefab(this.NextStatusPrefab);
        item.SetFoodList(originItem);
        return item;
    }
}

