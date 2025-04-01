
using System;
using Unity.Netcode;
using UnityEngine.InputSystem;

public class NetworkCounter : NetworkBehaviour {

    private BaseCounter Counter;

    private void Awake() {
        Counter = this.GetComponent<BaseCounter>();
    }

    public void InteractOneTime(PlayerInteract player) {
        InteractOneTimeServerRpc(player.GetNetworkObject());
    }

    [Rpc(SendTo.Server)]
    private void InteractOneTimeServerRpc(NetworkObjectReference reference) {
        InteractOneTimeClientRpc(reference);
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void InteractOneTimeClientRpc(NetworkObjectReference reference) {
        Counter.NetworkInteractOneTime(reference);
    }

    public void InteractOneTimeAlt(PlayerInteract player) {
        InteractOneTimeAltServerRpc(player.GetNetworkObject());
    }
    
    [Rpc(SendTo.Server)]
    private void InteractOneTimeAltServerRpc(NetworkObjectReference reference) {
        InteractOneTimeAltClientRpc(reference);
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void InteractOneTimeAltClientRpc(NetworkObjectReference reference) {
        Counter.NetworkInteractOneTimeAlt(reference);
    }

    public void InteractDur(PlayerInteract player, InputActionPhase phase) {
        InteractDurServerRpc(player.GetNetworkObject(), phase);
    }
    
    [Rpc(SendTo.Server)]
    private void InteractDurServerRpc(NetworkObjectReference reference, InputActionPhase phase) {
        InteractDurClientRpc(reference, phase);
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void InteractDurClientRpc(NetworkObjectReference reference, InputActionPhase phase) {
        Counter.NetworkInteractDur(reference, phase);
    }
}

