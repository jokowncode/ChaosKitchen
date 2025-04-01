
using UnityEngine;

public class PlaceObjectCounter : BaseCounter {

    protected bool GetInnerItemHolder = false;
    
    protected override void Awake() {
        base.Awake();
        if (this.HasObject && this.HoldItem == null && this.Holder.GetChild(0).TryGetComponent(out IItem item)) {
            this.HoldItem = item;
            this.HoldItem.SetParentHolder(this);
            OnGetItem();
        }
    }
    
    protected override void NetworkInteractOneTime(PlayerInteract player) {
        if (player.HasObject && player.HoldItem is ItemHolder playerHolder 
         && this.HasObject && this.HoldItem is ItemHolder myHolder) {
            if (playerHolder.HasObject) {
                if (!myHolder.GiveMeObject(playerHolder)) {
                    player.GiveMeObject(myHolder);
                }
            } else {
                if (!playerHolder.GiveMeObject(myHolder)) {
                    myHolder.GiveMeObject(player);
                }
            }
            return;
        }
        
        if (player.HasObject && (!this.HasObject || this.HoldItem is ItemHolder)) {
            ItemHolder holder = player;
            if (GetInnerItemHolder) {
                holder = player.HoldItem as ItemHolder ?? player;
            }
            this.GiveMeObject(holder);
        }else if (this.HasObject && (!player.HasObject || player.HoldItem is ItemHolder)) {
            player.GiveMeObject(this);
        }
    }
}