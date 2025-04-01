
using UnityEngine;

public abstract class BaseDurCookingOperation : BaseCookOP {

    [field: SerializeField] public float DurCookingTime { get; private set; } = 20.0f;

    public abstract override CookOP GetCookOP();
}

