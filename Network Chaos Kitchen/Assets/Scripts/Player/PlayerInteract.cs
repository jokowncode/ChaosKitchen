using System;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteract : ItemHolder {

    [Header("Throw")] 
    [SerializeField] private float MaxThrowForce = 30.0f;
    [SerializeField] private float InitialThrowForce = 10.0f;
    
    private IInteract InteractObject;
    private PlayerMove Move;
    private Player Player;
    private float CurrentThrowForce;

    public NetworkObject GetNetworkObject() {
        return Player.NetworkObject;
    }

    private void Awake() {
        Move = GetComponent<PlayerMove>();
        Player = GetComponent<Player>();
    }
    
    private void Start() {
        this.enabled = false;
        if (!Player.IsOwner) return;
        Move.OnHitObstacle += OnHitObstacle;
        Move.OnAwayObstacle += OnAwayObstacle;
        InputManager.Instance.OnOneTimeInteract += OnOneTimeInteract;
        InputManager.Instance.OnOneTimeInteractAlt += OnOneTimeInteractAlt;
        InputManager.Instance.OnDurInteract += OnDurInteract;
        NetworkGameManager.Instance.OnSceneChange += OnSceneChange;
    }

    private void OnDestroy() {
        if (!Player.IsOwner) return;
        InputManager.Instance.OnOneTimeInteract -= OnOneTimeInteract;
        InputManager.Instance.OnOneTimeInteractAlt -= OnOneTimeInteractAlt;
        InputManager.Instance.OnDurInteract -= OnDurInteract;
        NetworkGameManager.Instance.OnSceneChange -= OnSceneChange;
    }

    private void OnSceneChange() {
        this.CurrentThrowForce = 0.0f;
        this.InteractObject = null;
        if (!this.HasObject) return;
        Destroy(this.HoldItem.GetTransform().gameObject);
        this.HoldItem = null;
    }

    private void Update() {
        CurrentThrowForce = Mathf.Min(MaxThrowForce, CurrentThrowForce + Time.deltaTime * MaxThrowForce);
    }

    private void OnHitObstacle(GameObject obj) {
        if (obj.TryGetComponent(out IInteract interact)) {
            this.InteractObject?.StopInteract();
            this.InteractObject = interact;
            this.InteractObject.StartInteract();
        }
        
        if (obj.TryGetComponent(out IItem item)) {
            Vector3 dir = (item.GetTransform().position - transform.position).normalized;
            item.PhysicsForce(dir);
        }
    }

    private void OnAwayObstacle() {
        if (this.InteractObject is not null) {
            this.InteractObject.StopInteract();
            this.InteractObject = null;
        }
    }

    private void OnOneTimeInteract() {
        if (InteractObject != null && InteractObject.IsAvailable()) {
            InteractObject.InteractOneTime(this);
            return;
        }

        if (InteractObject != null || !this.HasObject ||
            this.HoldItem is not (PhysicsHasOpFoodItem or ToolItem)) return;
        DropHoldItem();
    }

    private void DropHoldItem() {
        Player.DropHoldItemServerRpc();
    }

    public void NetworkDropHoldItem(bool resetHoldItem = true) {
        if (!this.HasObject || this.HoldItem == null) return;
        this.HoldItem.GetTransform().SetParent(null);
        this.HoldItem.SetParentHolder(null);
        this.HoldItem.PhysicsAwake();
        if(resetHoldItem) this.HoldItem = null;
    }

    private void OnOneTimeInteractAlt() {
        if (InteractObject != null && InteractObject.IsAvailable()) {
            InteractObject.InteractOneTimeAlt(this);
        }
    }

    public void NetworkFireExtinguisher(InputActionPhase phase) {
        FireExtinguisherItem fireExtinguisher = this.HoldItem as FireExtinguisherItem;
        if (phase == InputActionPhase.Performed) {
            fireExtinguisher?.StartSmoke();
        }else if (phase == InputActionPhase.Canceled) {
            fireExtinguisher?.StopSmoke();
        }
    }

    public void NetworkThrowItem(float force) {
        NetworkDropHoldItem(false);
        Vector3 dir = (this.transform.forward + Vector3.up).normalized;
        this.HoldItem.PhysicsForce(dir * force);
        this.HoldItem = null;
        this.enabled = false;
    }

    private void OnDurInteract(InputActionPhase phase) {
        if (this.HasObject && this.HoldItem is FireExtinguisherItem) {
            NetworkFireExtinguisher(phase);
            Player.FireExtinguisherOtherRpc(phase);
            return;
        }
        
        if (InteractObject != null && InteractObject.IsAvailable()) {
            InteractObject.InteractDur(this, phase);
            return;
        }

        if (InteractObject != null || !this.HasObject ||
            this.HoldItem is not (PhysicsHasOpFoodItem or ToolItem)) return;
        
        if (phase == InputActionPhase.Performed) {
            this.enabled = true;
            CurrentThrowForce = InitialThrowForce;
        } else if (this.enabled && phase == InputActionPhase.Canceled) {
            NetworkThrowItem(CurrentThrowForce);
            Player.ThrowItemOtherRpc(CurrentThrowForce);
        }
    }
}