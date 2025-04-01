
using UnityEngine;
using UnityEngine.UI;

public class FoodBackground : MonoBehaviour {

    [SerializeField] private Image FoodImage;

    public void SetImage(Sprite foodSprite) {
        FoodImage.sprite = foodSprite;
    }
}

