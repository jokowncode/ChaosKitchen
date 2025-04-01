
using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class SinkCounter : BaseCounter {

    [Header("Dirty Plate Sink")]
    [SerializeField] private GameObject DirtyPlateVisual;
    [SerializeField] private CleanPlatesCounter CleanPlates;
    [SerializeField] private float WashSpeed = 0.5f;
    
    [Header("Wash Visual Effect")] 
    [SerializeField] private ParticleSystem WashParticle;
    
    private int CurrentDirtyPlateCount;
    private float CurrentWashProgress;

    private ProgressBar CurrentProgress;

    private void Start() {
        this.enabled = false;
    }

    protected override void NetworkInteractOneTime(PlayerInteract player) {
        if (!player.HasObject || player.HoldItem is not DirtyPlateItem plate) return;
        Destroy(plate.gameObject);
        DirtyPlateVisual.SetActive(true);
        if (CurrentDirtyPlateCount == 0) {
            CurrentWashProgress = 0.0f;
            this.CurrentProgress = PoolManager.Instance.GetProgressBarUI();
            this.CurrentProgress.SetWorldPosition(this.transform.position);
            this.CurrentProgress.UpdateProgress(0.0f);
            this.CurrentProgress.Show();
        }
        CurrentDirtyPlateCount += 1;
    }

    protected override void NetworkInteractDur(PlayerInteract player, InputActionPhase phase) {
        if (player.HasObject) return;
        if (CurrentDirtyPlateCount == 0) return;
        if (phase == InputActionPhase.Performed) {
            this.enabled = true;
            WashParticle.Play();
        }

        if (phase == InputActionPhase.Canceled) {
            this.enabled = false;
            WashParticle.Stop();
        }
    }

    private void Update() {
        CurrentWashProgress += WashSpeed * Time.deltaTime;
        if (CurrentWashProgress >= 1.0f) {
            CleanPlates.AddNewCleanPlate();
            CurrentDirtyPlateCount -= 1;
            if (CurrentDirtyPlateCount != 0) {
                this.CurrentProgress.UpdateProgress(0.0f);
                this.CurrentWashProgress = 0.0f;
            } else {
                this.enabled = false;
                WashParticle.Stop();
                DirtyPlateVisual.SetActive(false);
                this.CurrentProgress.Hide();
                PoolManager.Instance.ReleaseProgressBarUI(this.CurrentProgress);
                this.CurrentProgress = null;
            }
        } else {
            this.CurrentProgress.UpdateProgress(CurrentWashProgress);
        }
    }
}
