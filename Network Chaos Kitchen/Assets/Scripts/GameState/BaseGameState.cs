
using Unity.Netcode;
using UnityEngine;

public abstract class BaseGameState : NetworkBehaviour {
    public virtual void Construct() { }
    public virtual void Destruct() { }
    public virtual void Execute() { }
    public virtual void Transition() { }
}


