using System;
using System.Collections;
using System.Linq;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OrderUI : MonoBehaviour {

    [SerializeField] private Color NormalColor = Color.green;
    [SerializeField] private Color RedColor = Color.red;
    
    [SerializeField] private float MoveSpeed = 1000.0f;
    [SerializeField] private Image Progress;
    [SerializeField] private Image FinalFoodImage;
    [SerializeField] private Image[] IngredientSprite;
    [SerializeField] private Image[] IngredientCookingOpSprite;

    public Action<OrderUI> OnOrderTimeOut;
    
    private RectTransform OrderUIRectTransform;
    private CanvasGroup OrderUICanvasGroup;
    
    private float Timer;
    private float OrderTime;

    public RecipeSO CurrentRecipe { get; private set; }

    public OrderUI PreOrder { get; set; }
    public OrderUI AfterOrder { get; set; }

    private Vector2 TargetPos;
    
    private void Awake() {
        OrderUIRectTransform = GetComponent<RectTransform>();
        OrderUICanvasGroup = GetComponent<CanvasGroup>();
    }

    private Vector2 GetPosition() {
        return OrderUIRectTransform.anchoredPosition;
    }

    public int GetGrade() {
        return Mathf.FloorToInt((this.Timer / this.OrderTime) * 2.0f);
    }

    private void Update() {
        if (PreOrder is not null) {
            this.TargetPos = PreOrder.GetPosition() + Vector2.right * (this.OrderUIRectTransform.rect.width + 20.0f);
        } else{
            this.TargetPos = new Vector2(20.0f, this.OrderUIRectTransform.anchoredPosition.y);
        }
        
        if (this.OrderUIRectTransform.anchoredPosition != this.TargetPos) {
            this.OrderUIRectTransform.anchoredPosition = Vector2.MoveTowards(this.OrderUIRectTransform.anchoredPosition, this.TargetPos, this.MoveSpeed * Time.deltaTime);
        }

        if (!GameManager.Instance.IsPlaying) return;

        this.Timer -= Time.deltaTime;
        this.Progress.rectTransform.localScale = new Vector3(this.Timer / this.OrderTime, 1.0f, 1.0f);
        this.Progress.color = Color.Lerp(this.RedColor, this.NormalColor, this.Timer / this.OrderTime);
        
        if (this.Timer <= 0.0f) {
            OnOrderTimeOut?.Invoke(this);
        }
    }

    public void Hide() {
        this.enabled = false;
        this.OrderUICanvasGroup.alpha = 0.0f;
        PoolManager.Instance.ReleaseOrderUI(this);
    }

    public void UpdateVisual(RecipeSO recipe) {
        this.enabled = true;
        this.Timer = recipe.PrepareTime;
        this.OrderTime = recipe.PrepareTime;
        this.Progress.color = this.NormalColor;
        this.Progress.transform.localScale = Vector3.one;
        this.OrderUICanvasGroup.alpha = 1.0f;
        
        this.CurrentRecipe = recipe;

        this.FinalFoodImage.sprite = CurrentRecipe.FinalFoodPrefab.ItemInfo.FoodSprite;
        for (int i = 0; i < IngredientCookingOpSprite.Length; i++) {
            if (i >= recipe.RecipeFoods.Length) {
                IngredientSprite[i].transform.parent.gameObject.SetActive(false);
                continue;
            }

            IngredientSprite[i].transform.parent.gameObject.SetActive(true);
            RecipeFood recipeFood = recipe.RecipeFoods[i];
            IngredientSprite[i].sprite = recipeFood.OriginFoodSprite;

            RectTransform parentTrans = (RectTransform)IngredientCookingOpSprite[i].transform.parent.transform;
            if (recipeFood.Op == CookOP.None) {
                IngredientCookingOpSprite[i].gameObject.SetActive(false);
                parentTrans.sizeDelta = new Vector2(parentTrans.sizeDelta.x, parentTrans.sizeDelta.x);
            } else {
                IngredientCookingOpSprite[i].gameObject.SetActive(true);
                parentTrans.sizeDelta = new Vector2(parentTrans.sizeDelta.x, parentTrans.sizeDelta.x * 2);
                IngredientCookingOpSprite[i].sprite = SpriteManager.Instance.GetCookOpSprite(recipeFood.Op);
            }
        }

        Vector2 currentPos = this.OrderUIRectTransform.anchoredPosition;
        currentPos.x = ((RectTransform)this.transform.parent.transform).rect.width;
        this.OrderUIRectTransform.anchoredPosition = currentPos;
    }
}
