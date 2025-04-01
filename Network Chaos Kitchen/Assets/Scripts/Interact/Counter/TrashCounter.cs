
using UnityEngine;

public class TrashCounter : BaseCounter {
    protected override void NetworkInteractOneTime(PlayerInteract player) {
        if (player.HasObject) {
            SfxManager.Instance.PlaySound(SFXType.Trash, this.transform.position);
            player.DropObject();
        }
    }

    public override bool GetObject(IItem item) {
        if (item is ToolItem) return false;
        Destroy(item.GetTransform().gameObject);
        return true;
    }
}
