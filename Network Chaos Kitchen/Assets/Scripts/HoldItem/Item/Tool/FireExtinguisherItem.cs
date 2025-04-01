
using System;
using UnityEngine;

public class FireExtinguisherItem : ToolItem {

    [Header("Fire Extinguisher")]
    [SerializeField] private ParticleSystem SmokeParticle;
    [SerializeField] private LayerMask FireMask;

    protected override void Awake() {
        base.Awake();
        this.enabled = false;
    }

    public void StartSmoke() {
        this.enabled = true;
        this.transform.localRotation = Quaternion.identity;
        SfxManager.Instance.PlaySound(SFXType.Spray, this.transform.position);
        SmokeParticle.Play();
    }

    public void StopSmoke() {
        this.enabled = false;
        SmokeParticle.Stop();
    }

    private void Update() {
        bool result = Physics.Raycast(this.transform.parent.position + Vector3.up * 0.5f,
            this.transform.parent.forward, 
            out RaycastHit hit, 3.0f, FireMask);
        if (!result) return;
        if (hit.collider.TryGetComponent(out CounterFireUI fire)) {
            fire.ExtinguishFire();
        }
    }
}
