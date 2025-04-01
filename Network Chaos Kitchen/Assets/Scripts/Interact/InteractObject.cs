using UnityEngine;
using UnityEngine.InputSystem;

public abstract class InteractObject : ItemHolder, IInteract {

    [Header("Interact Visual Effect")] 
    [SerializeField] private GameObject Selected;

    protected bool Available = true;

    public bool IsAvailable() {
        return Available;
    }
    
    public virtual void StartInteract() {
        // Show Visual Effect
        Selected?.SetActive(true);
    }

    public virtual void StopInteract() {
        // Cancel visual effect
        Selected?.SetActive(false);
    }

    public virtual void InteractOneTime(PlayerInteract player) { }
    public virtual void InteractOneTimeAlt(PlayerInteract player) { }
    public virtual void InteractDur(PlayerInteract player, InputActionPhase phase) { }
}