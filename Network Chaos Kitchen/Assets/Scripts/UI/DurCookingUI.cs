
using System;
using UnityEngine;

public class DurCookingUI : MonoBehaviour {
    
    [Header("UI")]
    [SerializeField] private AudioSource Audio;
    [SerializeField] private GameObject WarningIcon;
    [SerializeField] private ProgressBar Progress;
    [SerializeField] private Animator Complete;
    
    [Header("Overcooked")]
    [SerializeField] private float OvercookedTime = 2.0f;
    [SerializeField] private float WarningThreshold = 0.7f;

    public Action OnCookingEnd;
    public Action OnCookingOvercooked;

    private BaseHalfFoodItem CurrentCookingFood;
    
    private float CurrentCookingTime;
    private float CurrentOvercookedTime;
    
    private bool IsCooked;
    private bool IsComplete;

    private void Awake() {
        WarningIcon.gameObject.SetActive(false);
        this.enabled = false;
    }

    public void SetWorldPosition(Vector3 worldPos) {
        this.transform.SetIndicatorUIPosition(worldPos);
    }

    public void StartCooking(BaseHalfFoodItem food) {
        this.CurrentCookingFood = food;
        
        if (!IsCooked) {
            SfxManager.Instance.PlayOneShot(this.Audio, SFXType.PanSizzle);
            CurrentCookingTime = food.GetAlreadyCookedTime();
        }
        
        IsComplete = CurrentCookingTime >= food.TotalCookingTime;
        if (!IsComplete) {
            CurrentOvercookedTime = 0.0f;
            Progress.UpdateProgress(CurrentCookingTime / food.TotalCookingTime);
            Progress.Show();
        } else {
            CurrentOvercookedTime = food.GetAlreadyOvercookedTime();
            Progress.Hide();
            OnCookingEnd?.Invoke();
        }
        IsCooked = true;
        this.enabled = true;
    }

    private void SaveCookingTime() {
        if (IsComplete) {
            CurrentCookingFood?.SaveOvercookedTime(this.CurrentOvercookedTime);
        } else {
            CurrentCookingFood?.SaveCookedTime(this.CurrentCookingTime);
        }
    }

    public void EndCooking() {
        Progress?.Hide();
        SaveCookingTime();
        CurrentCookingFood = null;
        
        this.WarningIcon?.gameObject.SetActive(false);
        if (this.Audio) {
            this.Audio.Stop();
            this.Audio.clip = null;
        }

        IsCooked = false;
        IsComplete = false;
        this.enabled = false;
    }
    
    private void Update() {
        float ratio;
        if (IsComplete) {
            CurrentOvercookedTime += Time.deltaTime;
            ratio = CurrentOvercookedTime / OvercookedTime;
            if (ratio >= WarningThreshold) {
                Warning();
            }
        } else {
            CurrentCookingTime += Time.deltaTime;
            ratio = CurrentCookingTime / CurrentCookingFood.TotalCookingTime;
            Progress.UpdateProgress(ratio);
        }

        if (ratio < 1.0f) return;
        SaveCookingTime();
        if (!IsComplete) {
            IsComplete = true;
            Progress.Hide();
            Complete.SetTrigger(AnimationParams.Show);
            SfxManager.Instance.PlayOneShot(this.Audio, SFXType.DurCookingComplete);
            OnCookingEnd?.Invoke();
        } else {
            OnCookingOvercooked?.Invoke();
        }
    }
    
    private void Warning() {
        this.WarningIcon.gameObject.SetActive(true);
        if (Audio.clip) return;
        SfxManager.Instance.PlaySound(this.Audio, SFXType.Warning);
    }
}
