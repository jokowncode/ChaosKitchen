using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "FoodInfo", menuName = "Chaos Kitchen/Kitchen Food")]
public class KitchenFoodSO : ScriptableObject {
    public Sprite FoodSprite;
    public FoodSort FoodSort;
}
