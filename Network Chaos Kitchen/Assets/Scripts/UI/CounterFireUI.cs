
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CounterFireUI : MonoBehaviour {

    [Header("Fire")] 
    [SerializeField] private ParticleSystem FireParticle;
    [SerializeField] private float SpreadTime = 5.0f;

    public Action OnFireSpread;
    public Action OnFireEnd;
    
    private Collider FireCollider;
    private float CurrentFireScale;
    private float CurrentFireTime;

    private ProgressBar FireProgress;

    private void Awake() {
        FireCollider = GetComponent<Collider>();
        FireCollider.enabled = false;
        this.enabled = false;
        SceneManager.sceneUnloaded += OnSceneUnload;
    }

    private void OnSceneUnload(Scene scene) {
        SceneManager.sceneUnloaded -= OnSceneUnload;
        ReleaseFireProgress();
    }

    private void Update() {
        CurrentFireTime += Time.deltaTime;
        if (CurrentFireTime >= SpreadTime) {
            OnFireSpread?.Invoke();
            this.enabled = false;
        }
    }

    public void StartFire() {
        if (this.enabled) return;
        this.CurrentFireScale = 1.0f;
        FireCollider.enabled = true;
        FireParticle?.Play();
        CurrentFireTime = 0.0f;
        this.enabled = true;
    }

    private void EndFire() {
        FireCollider.enabled = false;
        FireParticle?.Stop();
        this.enabled = false;
        OnFireEnd?.Invoke();
    }

    public void ExtinguishFire() {
        if (this.FireProgress is null) {
            this.FireProgress = PoolManager.Instance.GetProgressBarUI();
            this.FireProgress.Show();
            this.FireProgress.SetWorldPosition(this.transform.position);
        }

        this.CurrentFireScale -= Time.deltaTime;
        this.FireProgress.UpdateProgress(this.CurrentFireScale);
        if (this.CurrentFireScale > 0.0f) return;
        EndFire();
        ReleaseFireProgress();
    }

    private void ReleaseFireProgress() {
        if (this.FireProgress is null) return;
        this.FireProgress.Hide();
        PoolManager.Instance.ReleaseProgressBarUI(this.FireProgress);
        this.FireProgress = null;
    }
}

