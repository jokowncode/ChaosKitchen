
using UnityEngine;

public abstract class BaseDurCookingCounter : PlaceObjectCounter {
    
    [Header("Dur Cooking Visual Effect")] 
    [SerializeField] private ParticleSystem DurCookingVFX;
    [SerializeField] private GameObject CookingVisualEffect;
    
    protected void ShowVisualEffect() {
        if(DurCookingVFX) DurCookingVFX.gameObject.SetActive(true);
        if(CookingVisualEffect) CookingVisualEffect.SetActive(true);
    }

    protected void HideVisualEffect() {
        if(DurCookingVFX) DurCookingVFX.gameObject.SetActive(false);
        if(CookingVisualEffect) CookingVisualEffect.SetActive(false);
    }
}

public abstract class HoldItemBaseDurCookingCounter : BaseDurCookingCounter {
    protected override bool ValidateItem(IItem item) {
        return item is BaseDurCookingItem;
    }

    protected override void OnGetItem() {
        BaseDurCookingItem durCookingItem = this.HoldItem as BaseDurCookingItem;
        this.HoldItem.GetTransform().localRotation = Quaternion.identity;
        if (durCookingItem != null) {
            durCookingItem.OnStartCooking += ShowVisualEffect;
            durCookingItem.OnEndCooking += HideVisualEffect;
            durCookingItem.OnOverCooked += StartFire;
            durCookingItem.StartCooking();
        }
    }

    protected override void OnRemoveItem() {
        BaseDurCookingItem durCookingItem = this.HoldItem as BaseDurCookingItem;
        if (durCookingItem != null) {
            durCookingItem.EndCooking();
            durCookingItem.OnStartCooking -= ShowVisualEffect;
            durCookingItem.OnEndCooking -= HideVisualEffect;
            durCookingItem.OnOverCooked -= StartFire;
        }
    }
}

public abstract class SelfBaseDurCookingCounter : BaseDurCookingCounter {
    
    [Header("Cooking")]
    [SerializeField] private BaseDurCookingItem CookingItem;

    protected override void Awake() {
        base.Awake();
        CookingItem.OnStartCooking += ShowVisualEffect;
        CookingItem.OnEndCooking += HideVisualEffect;
        CookingItem.OnOverCooked += StartFire;
        CookingItem.SetParentHolder(this);
        this.GetInnerItemHolder = true;
    }

    public override IItem GetExchangeHoldItem() {
        return CookingItem.GetExchangeHoldItem();
    }

    public override bool GetObject(IItem item) {
        return CookingItem.GetObject(item);
    }

    protected override void OnRemoveItem() {
        CookingItem.RemoveItem();
    }
}
