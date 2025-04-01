
using UnityEngine;

[CreateAssetMenu(fileName = "Container", menuName = "Chaos Kitchen/Container Food")]
public class ContainerFoodSO : ScriptableObject {
    public FoodItem FoodPrefab;
    public Sprite FoodSprite;
}