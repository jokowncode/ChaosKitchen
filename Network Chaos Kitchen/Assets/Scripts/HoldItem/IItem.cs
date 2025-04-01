
using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;

public interface IItem {
    public Transform GetTransform();
    public void SetParentHolder(ItemHolder parent);
    public ItemHolder GetParentHolder();

    public void PhysicsSleep();
    public void PhysicsForce(Vector3 force);
    public void PhysicsAwake();
}

public class ToolItem : InteractObject, IItem {

    private ItemHolder ParentHolder;
    private Rigidbody ToolRigidbody;
    private Collider ToolCollider;

    protected virtual void Awake() {
        ToolRigidbody = this.GetComponent<Rigidbody>();
        ToolCollider = this.GetComponent<Collider>();
        if (ToolRigidbody != null && ToolCollider != null) {
            PhysicsSleep();    
        }
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

    public Transform GetTransform() {
        return this.transform;
    }

    public void SetParentHolder(ItemHolder parent) {
        this.ParentHolder = parent;
    }

    public ItemHolder GetParentHolder() {
        return this.ParentHolder;
    }

    public void PhysicsSleep() {
        if (this.ToolRigidbody.isKinematic) return;
        this.ToolRigidbody.rotation = Quaternion.identity;
        this.ToolRigidbody.linearVelocity = Vector3.zero;
        this.ToolRigidbody.angularVelocity = Vector3.zero;
        this.ToolRigidbody.isKinematic = true;
        this.transform.localRotation = Quaternion.identity;
        this.ToolCollider.enabled = false;
    }

    public void PhysicsForce(Vector3 force) {
        if (this.ToolRigidbody.isKinematic) return;
        this.ToolRigidbody.AddForce(force, ForceMode.Impulse);
    }
    
    public void PhysicsAwake() {
        this.ToolRigidbody.isKinematic = false;
        this.ToolCollider.enabled = true;
    }

    public override void InteractOneTime(PlayerInteract player) {
        if (player.HasObject) return;
        player.GetObject(this);
    }
}

public abstract class PlateItem : ToolItem {}
