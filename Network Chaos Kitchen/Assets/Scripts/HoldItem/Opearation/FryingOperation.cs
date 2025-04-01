
public class FryingOperation : SingleIngredientDurCookingOperation {
    public override CookOP GetCookOP() {
        return CookOP.Fry;
    }
}
