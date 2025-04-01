using UnityEngine;

public class CleanPlatesCounter : PlateCounter {

    [Header("Plates")] 
    [SerializeField] private bool AutoWashPlate = true;
    [SerializeField] private CleanPlateItem CleanPlatePrefab;
    [SerializeField] private int MaxPlateCount = 5;
    [SerializeField] private float SpawnInterval = 1.0f;
    
    private float Timer;
    private int CurrentPlateCount;

    private void Start() {
        if (AutoWashPlate) {
            RecipeManager.Instance.OnPlateReleased += () => {
                this.CurrentPlateCount -= 1;
                this.enabled = this.CurrentPlateCount < this.MaxPlateCount;
            };
        }
        this.IsCleanPlate = true;
        this.CurrentPlateCount = this.Holder.childCount;
        this.enabled = this.CurrentPlateCount < this.MaxPlateCount;
    }

    private void Update() {
        Timer += Time.deltaTime;
        if (Timer < SpawnInterval) return;
        Timer = 0.0f;
        AddNewCleanPlate();
        this.CurrentPlateCount += 1;
        if (this.CurrentPlateCount >= MaxPlateCount) {
            this.enabled = false;
        }
    }

    public void AddNewCleanPlate() {
        CleanPlateItem item = Instantiate(CleanPlatePrefab, this.Holder);
        item.transform.localPosition = Vector3.up * ((this.Holder.childCount - 1) * 0.1f);
    }
}
