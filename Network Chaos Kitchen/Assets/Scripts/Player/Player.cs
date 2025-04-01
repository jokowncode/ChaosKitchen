using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : NetworkBehaviour {

    public int OwnerIndex { get; private set; }

    private PlayerInteract Interact;

    private void Awake() {
        DontDestroyOnLoad(this.gameObject);
    }

    public override void OnNetworkSpawn() {
        Interact = this.GetComponent<PlayerInteract>();
        Interact.enabled = true;
        GetComponent<PlayerMove>().enabled = true;
        
        this.OwnerIndex = NetworkGameManager.Instance.GetPlayerIndex(OwnerClientId);
        if (this.OwnerIndex != -1) {
            GetComponent<PlayerVisual>().SetColor(NetworkGameManager.Instance.GetPlayerColor(this.OwnerIndex));
        }
    }

    public override void OnNetworkDespawn() {
        Interact.NetworkDropHoldItem();
    }

    [Rpc(SendTo.Server)]
    public void DropHoldItemServerRpc() {
        DropHoldItemClientRpc();
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void DropHoldItemClientRpc() {
        Interact.NetworkDropHoldItem();
    }

    [Rpc(SendTo.NotOwner)]
    public void FireExtinguisherOtherRpc(InputActionPhase phase) {
        Interact.NetworkFireExtinguisher(phase);
    }

    [Rpc(SendTo.NotOwner)]
    public void ThrowItemOtherRpc(float force) {
        Interact.NetworkThrowItem(force);
    }
}

