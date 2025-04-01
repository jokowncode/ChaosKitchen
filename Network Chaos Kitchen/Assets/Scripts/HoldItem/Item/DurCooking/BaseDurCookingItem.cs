using System;
using UnityEngine.SceneManagement;

public abstract class BaseDurCookingItem : ToolItem {

    protected int _MaxIngredientCount;
    protected BaseHalfFoodItem _NotCompleteFoodPrefab;
    
    public Action OnStartCooking;
    public Action OnEndCooking;
    public Action OnOverCooked;

    private bool IsComplete;
    private bool IsOvercooked;

    private DurCookingUI CurrentDurCookingUI;

    protected override void Awake() {
        base.Awake();
        SceneManager.sceneUnloaded += OnSceneUnload;
    }

    private void OnCookingEnd() {
        IsComplete = true;
    }

    private void OverCooked() {
        EndCooking();
        IsOvercooked = true;
        BaseHalfFoodItem food = (BaseHalfFoodItem)this.HoldItem;
        this.HoldItem = food.FoodOvercooked();
        OnOverCooked?.Invoke();
    }

    public override IItem GetExchangeHoldItem() {
        if (this.HoldItem is null) {
            return this.HoldItem;
        }
        if (IsOvercooked || !IsComplete) {
            return this.HoldItem;
        }
        BaseHalfFoodItem food = (BaseHalfFoodItem)this.HoldItem;
        return food.FoodComplete();
    }

    public override bool GetObject(IItem item) {
        if (this.Holder is null) return false;
        if (!ValidateItem(item)) return false;
        GetItem(item);
        return true;
    }
    
    protected override void GetItem(IItem item) {
        if (this.HoldItem is null && 
            ((this is SingleIngredientDurCookingItem && item is SingleIngredientHalfFoodItem)
             || (this is MultiIngredientDurCookingItem && item is MultiIngredientHalfFoodItem))) {
            base.GetItem(item);
            return;
        }

        if (this.HoldItem is not BaseHalfFoodItem currentFood) {
            currentFood = Instantiate(this._NotCompleteFoodPrefab, this.Holder);
            currentFood.SetCookingOP(GetItemCookingOp());
            this.HoldItem = currentFood;
            this.HoldItem.SetParentHolder(this);
        }
        
        currentFood.AddIngredient((FoodItem)item);
        Destroy(item.GetTransform().gameObject);
        this.OnGetItem();
    }
    
    public void StartCooking() {
        if (!this.HasObject || this.HoldItem is not BaseHalfFoodItem halfFood) return;
        if (this.GetParentHolder() is not BaseDurCookingCounter) return;
        IsComplete = false;
        IsOvercooked = false;

        if (CurrentDurCookingUI is null) {
            this.CurrentDurCookingUI = PoolManager.Instance.GetCookingUI();
            this.CurrentDurCookingUI.SetWorldPosition(this.GetParentHolder().transform.position);
            this.CurrentDurCookingUI.OnCookingEnd += OnCookingEnd;
            this.CurrentDurCookingUI.OnCookingOvercooked += OverCooked;
        }
        this.CurrentDurCookingUI.StartCooking(halfFood);
        this.OnStartCooking?.Invoke();
    }

    private void OnSceneUnload(Scene scene) {
        SceneManager.sceneUnloaded -= OnSceneUnload;
        if (this.CurrentDurCookingUI is null) return;
        this.CurrentDurCookingUI.OnCookingEnd -= OnCookingEnd;
        this.CurrentDurCookingUI.OnCookingOvercooked -= OverCooked;
        this.CurrentDurCookingUI.EndCooking();
        PoolManager.Instance.ReleaseCookingUI(this.CurrentDurCookingUI);
        this.CurrentDurCookingUI = null;
    }

    public void EndCooking() {
        this.CurrentDurCookingUI?.EndCooking();
        this.OnEndCooking?.Invoke();
    }
    
    protected override void OnGetItem() {
        StartCooking();
    }

    protected override void OnRemoveItem() {
        RemoveItem();
    }

    public void RemoveItem() {
        EndCooking();
        ResetState();
    }

    protected abstract CookOP GetItemCookingOp();
    protected override bool ValidateItem(IItem item) {
        if (item is FinalFoodItem or OvercookedFoodItem or BaseHalfFoodItem { IsComplete: true }) return false;
        
        int currentIngredientCount = 0;
        if (this.HoldItem is BaseHalfFoodItem currentFood) {
            currentIngredientCount = currentFood.GetIngredientCount();
        }
        
        return item switch {
            MultiIngredientFoodItem ingredients => currentIngredientCount + ingredients.GetIngredientCount() <= _MaxIngredientCount 
                                            && ingredients.HasOp(GetItemCookingOp()),
            HasOpFoodItem food => food.HasOp(GetItemCookingOp()),
            _ => false
        };
    }

    private void ResetState() {
        this.IsComplete = false;
        this.IsOvercooked = false;
        this.HoldItem = null;
    }

    public override void DropObject() {
        base.DropObject();
        ResetState();
    }
}



