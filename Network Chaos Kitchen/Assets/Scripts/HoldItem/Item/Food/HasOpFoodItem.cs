using System.Collections.Generic;

public class HasOpFoodItem : FoodItem {
    
    private Dictionary<CookOP, BaseCookOP> Ops;
    
    protected virtual void Awake() {
        BaseCookOP[] ItemOps = this.GetComponents<BaseCookOP>();
        if (ItemOps.Length == 0) return;
        Ops = new Dictionary<CookOP, BaseCookOP>();
        foreach (BaseCookOP op in ItemOps) {
            Ops.Add(op.GetCookOP(), op);
        }
    }

    public T GetCookOp<T>(CookOP op) where T : BaseCookOP {
        if (Ops.TryGetValue(op, out BaseCookOP itemOp)) {
            return itemOp as T;
        }
        return null;
    }

    public bool HasOp(CookOP op) {
        return Ops.ContainsKey(op);
    }
}
