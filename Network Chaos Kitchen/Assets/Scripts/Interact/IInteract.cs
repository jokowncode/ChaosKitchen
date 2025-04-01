
using UnityEngine.InputSystem;

public interface IInteract {
    public void StartInteract();
    public void StopInteract();

    public bool IsAvailable();

    public void InteractOneTime(PlayerInteract player);
    public void InteractOneTimeAlt(PlayerInteract player);
    public void InteractDur(PlayerInteract player, InputActionPhase phase);
}