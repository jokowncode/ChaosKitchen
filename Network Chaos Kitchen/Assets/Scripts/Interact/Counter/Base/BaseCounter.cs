using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class BaseCounter : InteractObject {

    [Header("Fire")]
    [SerializeField] private CounterFireUI CounterFire;
    
    private BaseCounter[] NeighborCounters;
    private NetworkCounter NetworkCounter;
    
    protected virtual void Awake() {
        Collider[] colliders = new Collider[4];
        int count = Physics.OverlapBoxNonAlloc(this.transform.position, Vector3.one * 1.5f, colliders, Quaternion.identity, LayerMask.GetMask("Counters"));
        NeighborCounters = new BaseCounter[count];
        for (int i = 0; i < count; i++) {
            NeighborCounters[i] = colliders[i].GetComponent<BaseCounter>();
        }
        CounterFire.OnFireSpread += OnFireSpread;
        CounterFire.OnFireEnd += OnFireEnd;
        NetworkCounter = GetComponent<NetworkCounter>();
    }

    private void OnFireEnd() {
        this.Available = true;
    }

    private void OnFireSpread() {
        foreach (BaseCounter counter in NeighborCounters) {
            counter?.StartFire();
        }
    }

    protected void StartFire() {
        if (!this.Available) return;
        CounterFire.StartFire();
        this.Available = false;
    }

    public override void InteractOneTime(PlayerInteract player) {
        NetworkCounter.InteractOneTime(player);
    }

    public override void InteractOneTimeAlt(PlayerInteract player) {
        NetworkCounter.InteractOneTimeAlt(player);
    }

    public override void InteractDur(PlayerInteract player, InputActionPhase phase) {
        NetworkCounter.InteractDur(player, phase);
    }

    public void NetworkInteractOneTime(NetworkObjectReference reference) {
        if (reference.TryGet(out NetworkObject obj) && obj.TryGetComponent(out PlayerInteract player)) {
            NetworkInteractOneTime(player);
        }
    }

    protected virtual void NetworkInteractOneTime(PlayerInteract player) { }

    public void NetworkInteractOneTimeAlt(NetworkObjectReference reference) {
        if (reference.TryGet(out NetworkObject obj) && obj.TryGetComponent(out PlayerInteract player)) {
            NetworkInteractOneTimeAlt(player);
        }
    }

    protected virtual void NetworkInteractOneTimeAlt(PlayerInteract player) { }

    public void NetworkInteractDur(NetworkObjectReference reference, InputActionPhase phase) {
        if (reference.TryGet(out NetworkObject obj) && obj.TryGetComponent(out PlayerInteract player)) {
            NetworkInteractDur(player, phase);
        }
    }

    protected virtual void NetworkInteractDur(PlayerInteract player, InputActionPhase phase) { }
}