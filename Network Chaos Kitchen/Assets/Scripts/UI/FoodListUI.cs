using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FoodListUI : MonoBehaviour {

    [SerializeField] private FoodBackground FoodBackgroundPrefab;
    [SerializeField] private Transform FoodList;

    private Transform Target;
    
    public void ResetState() {
        this.Target = null;
        this.ClearFood();
    }

    public void AddFood(Sprite foodSprite) {
        FoodBackground food = Instantiate(FoodBackgroundPrefab, this.FoodList);
        food.SetImage(foodSprite);
    }

    public void SetTarget(Transform target) {
        this.Target = target;
        SetWorldPosition();
    }

    private void SetWorldPosition() {
        if (this.Target) {
            this.transform.SetIndicatorUIPosition(this.Target.position, false);
        }
    }

    private void LateUpdate() {
        SetWorldPosition();
    }

    private void ClearFood() {
        if (!this.FoodList.IsDestroyed()) {
            foreach (Transform child in this.FoodList) {
                Destroy(child.gameObject);
            }
        }
        PoolManager.Instance.ReleaseFoodListUI(this);
    }

    public void AddFromOther(FoodListUI other) {
        while (other.FoodList.childCount > 0) {
            Transform child = other.FoodList.GetChild(0);
            child.SetParent(this.FoodList, false);
            child.localPosition = Vector3.zero;
        }
        other.Target = null;
        PoolManager.Instance.ReleaseFoodListUI(other);
    }
}

