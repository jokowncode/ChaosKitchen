
using UnityEngine;

public class DeliveryCounter : BaseCounter {

    protected override void NetworkInteractOneTime(PlayerInteract player) {
        if (player.HasObject && player.HoldItem is CleanPlateItem {HasObject : true} plate) {
            player.NetworkDropHoldItem();
            RecipeManager.Instance.PlayerSubmitDish(plate.CurrentCompleteRecipe);
            Destroy(plate.gameObject);
        }
    }
    
}
