using UnityEngine;

public class DirtyPlateCounter : PlateCounter {

    [Header("Plate")] 
    [SerializeField] private DirtyPlateItem DirtyPlatePrefab;
    [SerializeField] private float Interval = 5.0f;

    private void Start() {
        RecipeManager.Instance.OnPlateReleased += () => {
            Invoke(nameof(GenerateDirtyPlate), Interval);
        };
        this.IsCleanPlate = false;
    }

    private void GenerateDirtyPlate() {
        DirtyPlateItem item = Instantiate(DirtyPlatePrefab, this.Holder);
        item.transform.localPosition = Vector3.up * ((this.Holder.childCount - 1) * 0.1f);
    }
}
