
using UnityEngine;
using UnityEngine.InputSystem;

public class PhysicsHasOpFoodItem : HasOpFoodItem, IInteract {
    [Header("Interact Visual Effect")] 
    [SerializeField] private GameObject Selected;
    
    private Rigidbody FoodRigidbody;
    private Collider FoodCollider;

    protected override void Awake() {
        base.Awake();
        FoodRigidbody = GetComponent<Rigidbody>();
        FoodCollider = GetComponent<Collider>();
        PhysicsSleep();
    }
    
    private void OnCollisionEnter(Collision other) {
        if (this.ParentHolder != null) return;
        if (!other.gameObject.CompareTag("Player") && other.gameObject.TryGetComponent(out ItemHolder holder)
            && MathTool.DotTest(this.transform.position, other.transform.position, Vector3.down)) {
            if (!holder.GetObject(this)) {
                Vector3 dir = (this.transform.position - other.transform.position).normalized;
                this.PhysicsForce(dir);
            }
        }
    }
    
    public override void PhysicsSleep() {
        if (this.FoodRigidbody.isKinematic) return;
        this.FoodRigidbody.rotation = Quaternion.identity;
        this.FoodRigidbody.linearVelocity = Vector3.zero;
        this.FoodRigidbody.angularVelocity = Vector3.zero;
        this.FoodRigidbody.isKinematic = true;
        this.transform.localRotation = Quaternion.identity;
        this.FoodCollider.enabled = false;
    }
    
    public override void PhysicsForce(Vector3 force) {
        if (this.FoodRigidbody.isKinematic) return;
        this.FoodRigidbody.AddForce(force, ForceMode.Impulse);
    }

    public override void PhysicsAwake() {
        this.FoodRigidbody.isKinematic = false;
        this.FoodCollider.enabled = true;
    }

    public void StartInteract() {
        // Show Visual Effect
        Selected?.SetActive(true);
    }

    public void StopInteract() {
        // Cancel visual effect
        Selected?.SetActive(false);
    }

    public bool IsAvailable() {
        return true;
    }
    
    public void InteractOneTime(PlayerInteract player) {
        if (player.HasObject) return;
        player.GetObject(this);
    }

    public void InteractOneTimeAlt(PlayerInteract player) { }
    public void InteractDur(PlayerInteract player, InputActionPhase phase) { }
}

