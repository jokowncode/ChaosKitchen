
using UnityEngine;

public class ItemHolder : MonoBehaviour {

    [Header("Item Holder")]
    [SerializeField] protected Transform Holder;

    private bool CanHoldItem => Holder != null;
    
    public bool HasObject => CanHoldItem && Holder.childCount != 0;

    public IItem HoldItem { get; protected set; }
    
    protected virtual bool ValidateItem(IItem item) { return true; }

    protected virtual void OnGetItem() { }

    protected virtual void OnRemoveItem() { }

    public virtual IItem GetExchangeHoldItem() {
        return this.HoldItem;
    }

    public bool GiveMeObject(ItemHolder other) {
        IItem item = other.GetExchangeHoldItem();
        if (item == null) return false;
        if (!GetObject(item)) return false;

        if (this is PlayerInteract) {
            SfxManager.Instance.PlaySound(SFXType.ObjPickup, this.transform.position);
        }

        if (other is PlayerInteract) {
            SfxManager.Instance.PlaySound(SFXType.ObjDrop, this.transform.position);
        }

        other.OnRemoveItem();
        other.HoldItem = null;
        return true;
    }

    public virtual bool GetObject(IItem item) {
        if (!this.CanHoldItem) return false;
        if (this.HasObject && this.HoldItem is ItemHolder holder) {
            return holder.GetObject(item);
        }
        if (this.HasObject) return false;
        if (!ValidateItem(item)) return false;
        GetItem(item);
        return true;
    }

    protected virtual void GetItem(IItem item) {
        Transform itemTrans = item.GetTransform(); 
        itemTrans.parent = this.Holder;
        itemTrans.localPosition = Vector3.zero;
        this.HoldItem = item;
        this.HoldItem.SetParentHolder(this);
        this.HoldItem.PhysicsSleep();
        this.OnGetItem();
    }

    public virtual void DropObject() {
        if (!this.HasObject) return;
        if (this.HoldItem is ItemHolder holder) {
            holder.DropObject();
            return;
        }
        Destroy(this.HoldItem.GetTransform().gameObject);
        this.HoldItem = null;
    }
}
