using UnityEngine;
using UnityEngine.SceneManagement;

public class CuttingCounter : PlaceObjectCounter {

    private Animator CuttingAnimator;
    private ProgressBar CurrentProgress;
    private int CurrentCutTime = 0;
    
    protected override void Awake() {
        base.Awake();
        CuttingAnimator = GetComponent<Animator>();
        SceneManager.sceneUnloaded += OnSceneUnload;
    }

    private void OnSceneUnload(Scene scene) {
        SceneManager.sceneUnloaded -= OnSceneUnload;
        OnRemoveItem();
    }

    protected override void OnGetItem() {
        if (this.HoldItem is not HasOpFoodItem food || !food.HasOp(CookOP.Cut)) return;
        this.CurrentProgress = PoolManager.Instance.GetProgressBarUI();
        this.CurrentProgress.SetWorldPosition(this.transform.position);
        this.CurrentProgress.UpdateProgress(0.0f);
        this.CurrentProgress.Show();
    }

    protected override void OnRemoveItem() {
        if (this.CurrentProgress is null) return;
        this.CurrentProgress.Hide();
        PoolManager.Instance.ReleaseProgressBarUI(this.CurrentProgress);
        this.CurrentProgress = null;
    }

    protected override void NetworkInteractOneTime(PlayerInteract player) {
        if (CurrentCutTime > 0) return;
        base.NetworkInteractOneTime(player);
    }

    protected override void NetworkInteractOneTimeAlt(PlayerInteract player) {
        if (!this.HasObject) return;
        if (player.HasObject) return;
        if (this.HoldItem is not HasOpFoodItem food) return;

        CutOperation cutOp = food.GetCookOp<CutOperation>(CookOP.Cut);
        if (cutOp == null) return;

        CuttingAnimator.SetTrigger(AnimationParams.Cut);
        SfxManager.Instance.PlaySound(SFXType.Chop, this.transform.position);
        
        this.CurrentCutTime += 1;
        float progress = this.CurrentCutTime * 1.0f / cutOp.CutCount;
        this.CurrentProgress.UpdateProgress(progress);
        
        if (CurrentCutTime < cutOp.CutCount) return;
        OnRemoveItem();
        this.HoldItem = cutOp.GetNextStatusItem(food);
        this.CurrentCutTime = 0;
    }
}