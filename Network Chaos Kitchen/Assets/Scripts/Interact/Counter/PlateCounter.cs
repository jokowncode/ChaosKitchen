
public abstract class PlateCounter : BaseCounter {

    protected bool IsCleanPlate = true;

    protected override bool ValidateItem(IItem item) {
        return item is PlateItem;
    }
    
    protected override void NetworkInteractOneTime(PlayerInteract player) {
        if (this.Holder.childCount == 0) return;
        
        PlateItem plate = this.Holder.GetChild(this.Holder.childCount - 1).GetComponent<PlateItem>();
        if (player.HasObject && IsCleanPlate) {
            plate.GiveMeObject(player);
        }

        if (!player.HasObject && player.GetObject(plate)) {
            SfxManager.Instance.PlaySound(SFXType.ObjPickup, this.transform.position);
        }
    }
}
