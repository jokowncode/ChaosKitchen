using UnityEngine;

public class ContainerCounter : BaseCounter {

    [Header("Food Container")] 
    [SerializeField] private ContainerFoodSO Food;
    [SerializeField] private SpriteRenderer FoodSpriteRenderer;
    
    private Animator CounterAnimator;
    
    protected override void Awake() {
        base.Awake();
        CounterAnimator = GetComponent<Animator>();
        FoodSpriteRenderer.sprite = Food.FoodSprite;
    }

    protected override void NetworkInteractOneTime(PlayerInteract player) {
        if (player.HasObject) return;
        CounterAnimator.SetTrigger(AnimationParams.OpenClose);
        SfxManager.Instance.PlaySound(SFXType.ObjPickup, this.transform.position);
        FoodItem item = Instantiate(Food.FoodPrefab);
        item.InitialFoodListUI();
        player.GetObject(item);
    }
}
