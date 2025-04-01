public class StoveCounter : HoldItemBaseDurCookingCounter {
    protected override bool ValidateItem(IItem item) {
        return item is StoveMultiIngredientCookingItem or StoveSingleIngredientCookingItem;
    }
}