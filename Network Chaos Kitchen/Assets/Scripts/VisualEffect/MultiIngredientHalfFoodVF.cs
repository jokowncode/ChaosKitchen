
using System;
using UnityEngine;

public class MultiIngredientHalfFoodVF : MonoBehaviour {

    [SerializeField] private GameObject Tomato;
    [SerializeField] private GameObject Cabbage;
    
    public void AddIngredient(FoodSort foodSort) {
        string sort = Enum.GetName(typeof(FoodSort), foodSort);
        if (sort == null) return;
        if (sort.Contains("Tomato")) {
            Tomato.gameObject.SetActive(true);
        }else if (sort.Contains("Cabbage")) {
            Cabbage.gameObject.SetActive(true);
        }
    }
}


