
using System;
using UnityEngine;

public abstract class FoodItem : MonoBehaviour, IItem {
    
    [Header("Item Info")] 
    [field : SerializeField] public KitchenFoodSO ItemInfo { get; private set; }
    
    protected ItemHolder ParentHolder;

    private FoodListUI CurrentFoodListUI;
    private bool HasFoodList => CurrentFoodListUI != null;

    private void OnDestroy() {
        this.CurrentFoodListUI?.ResetState();
        this.CurrentFoodListUI = null;
    }
    
    public void SetFoodList(FoodItem other) {
        SetFoodList(other.CurrentFoodListUI);
        other.CurrentFoodListUI = null;
    }

    private void SetFoodList(FoodListUI other) {
        this.CurrentFoodListUI = other;
        this.CurrentFoodListUI.SetTarget(this.transform);
    }

    protected void AddFoodListFromOther(FoodItem other) {
        if (!other.HasFoodList) {
            return;
        }

        if (!this.HasFoodList) {
            SetFoodList(other);
            return;
        }
        this.CurrentFoodListUI.AddFromOther(other.CurrentFoodListUI);
        other.CurrentFoodListUI = null;
    }

    public void InitialFoodListUI() {
        if (HasFoodList) return;
        this.CurrentFoodListUI = PoolManager.Instance.GetFoodListUI();
        this.CurrentFoodListUI.SetTarget(this.transform);
        if (this.ItemInfo is not null) {
            this.CurrentFoodListUI.AddFood(this.ItemInfo.FoodSprite);
        }
    }

    public Transform GetTransform() {
        return this.transform;
    }

    public void SetParentHolder(ItemHolder parent) {
        this.ParentHolder = parent;
    }

    public ItemHolder GetParentHolder() {
        return this.ParentHolder;
    }

    public virtual void PhysicsSleep() { }

    public virtual void PhysicsForce(Vector3 force) { }

    public virtual void PhysicsAwake() { }
}


