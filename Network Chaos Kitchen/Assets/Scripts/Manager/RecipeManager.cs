using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

public class RecipeManager : NetworkBehaviour {
    
    [Header("Order Generate")]
    [SerializeField] private LevelRecipeSO LevelRecipe;
    [SerializeField] private float OrderSpawnInterval = 5.0f;
    [SerializeField] private int MaxOrders = 5;
    [SerializeField] private int StartSpawnCount = 2;
    [SerializeField] private int FoodSortCodeBits = 2;
    
    public static RecipeManager Instance { get; private set; }
    
    private int[] RecipeCodes;
    private Dictionary<FoodSort, byte> FoodCodes;
    
    private float Timer;
    private int CurrentWaitingOrderCount;

    private OrderUI HeadWaitingOrder;
    private OrderUI TailWaitingOrder;
    
    public Action OnPlateReleased;
    
    private void Awake() {
        Instance = this;
        Timer = OrderSpawnInterval;
        InitializeMenus();
    }

    private void GenerateRecipe() {
        ClientGenerateRecipeRpc(Random.Range(0, LevelRecipe.Recipes.Length));
    }
    
    [Rpc(SendTo.ClientsAndHost)]
    private void ClientGenerateRecipeRpc(int recipeIndex) {
        RecipeSO recipe = this.LevelRecipe.Recipes[recipeIndex];
        UpdateOrderUI(recipe);
    }

    public void ClearOrder() {
        OrderUI current = HeadWaitingOrder;
        while (current is not null) {
            current.Hide();
            current = current.AfterOrder;
        }
    }

    private void GenerateStartOrder() {
        for (int i = 0; i < StartSpawnCount; i++) {
            GenerateRecipe();
        }
    }

    private void Update() {
        if (!IsHost) return;
        if (!GameManager.Instance.IsPlaying) return; 
        Timer += Time.deltaTime;
        if (Timer < OrderSpawnInterval) return;
        Timer = 0.0f;
        if (CurrentWaitingOrderCount == 0 && StartSpawnCount != 0) {
            GenerateStartOrder();
        } else if (CurrentWaitingOrderCount < MaxOrders) {
            GenerateRecipe();
        }
    }

    private void UpdateOrderUI(RecipeSO recipe) {
        OrderUI orderUI = PoolManager.Instance.GetOrderUI();
        CurrentWaitingOrderCount += 1;

        if (HeadWaitingOrder is null) {
            HeadWaitingOrder = orderUI;
            HeadWaitingOrder.AfterOrder = null;
            HeadWaitingOrder.PreOrder = null;
        } else {
            TailWaitingOrder.AfterOrder = orderUI;
            orderUI.PreOrder = TailWaitingOrder;
            orderUI.AfterOrder = null;
        }
        TailWaitingOrder = orderUI;
        
        orderUI.UpdateVisual(recipe);
        orderUI.OnOrderTimeOut += OnOrderTimeOut;
    }

    private void OnOrderTimeOut(OrderUI timeOutOrder) {
        if(IsHost) GameManager.Instance.AddScore(-timeOutOrder.CurrentRecipe.BaseFee / 2);
        RemoveOrder(timeOutOrder);
    }

    public void PlayerSubmitDish(RecipeType type) {
        this.OnPlateReleased?.Invoke();

        if (HeadWaitingOrder is null || type == RecipeType.None) {
            if(IsHost) GameManager.Instance.PlayerSubmitErrorDish();
            SfxManager.Instance.PlaySound(SFXType.DeliveryFail, this.transform.position);
            return;
        }
        
        OrderUI current = HeadWaitingOrder;
        while (current != null) {
            if (current.CurrentRecipe.FinalFoodPrefab.Type == type) {
                break;
            }
            current = current.AfterOrder;
        }

        if (current == null) {
            if(IsHost) GameManager.Instance.PlayerSubmitErrorDish();
            SfxManager.Instance.PlaySound(SFXType.DeliveryFail, this.transform.position);
            return;
        }
        
        if(IsHost) GameManager.Instance.OrderCompleteAddScore(current.CurrentRecipe.BaseFee, current.GetGrade());
        SfxManager.Instance.PlaySound(SFXType.DeliverySuccess, this.transform.position);
        RemoveOrder(current);
    }

    private void RemoveOrder(OrderUI orderUI) {
        orderUI.OnOrderTimeOut -= OnOrderTimeOut;

        if (orderUI.PreOrder is not null) {
            orderUI.PreOrder.AfterOrder = orderUI.AfterOrder;
        }

        if (orderUI.AfterOrder is not null) {
            orderUI.AfterOrder.PreOrder = orderUI.PreOrder;
        }

        if (orderUI == HeadWaitingOrder) {
            HeadWaitingOrder = orderUI.AfterOrder;
        }

        if (orderUI == TailWaitingOrder) {
            TailWaitingOrder = orderUI.PreOrder;
        }

        this.CurrentWaitingOrderCount -= 1;
        orderUI.Hide();
    }

    private void InitializeMenus() {
        RecipeSO[] recipes = LevelRecipe.Recipes;
        RecipeCodes = new int[recipes.Length];
        FoodCodes = new Dictionary<FoodSort, byte>();

        byte current = 0;
        for (int i = 0;  i < recipes.Length; i++) {
            RecipeSO recipe = recipes[i];
            int recipeCode = 0;
            foreach (RecipeFood recipeFood in recipe.RecipeFoods) {
                FoodSort sort = recipeFood.CurrentFoodSort;
                if (!FoodCodes.ContainsKey(sort)) {
                    FoodCodes.Add(sort, current++);
                }

                byte foodCode = FoodCodes[sort];
                if (ValidateIngredientRepeat(recipeCode, foodCode)) {
                    recipeCode += 1 << (foodCode * FoodSortCodeBits);
                } else {
                    throw new Exception("Recipe Code can only repeated three times most.");
                }
            }
            RecipeCodes[i] = recipeCode;
        }
        // Debug.Log($"Current Level Has Recipe : {recipes.Length}, Has Food Sorts (In Recipe) : {FoodCodes.Count}");
    }

    private bool ValidateIngredientRepeat(int code, int foodCode) {
        int re = ((3 << (foodCode * FoodSortCodeBits)) & code) >> (foodCode * FoodSortCodeBits);
        return re < 3;
        // Debug.LogWarning("Recipe Ingredient Should Only Repeat Three Times most");
    }

    public void UpdateCode(ref int code, FoodSort foodSort) {
        if (!FoodCodes.TryGetValue(foodSort, out byte foodCode)) return;

        if (ValidateIngredientRepeat(code, foodCode)) {
            code += 1 << (foodCode * FoodSortCodeBits);
        }
    }

    public RecipeSO GetCompleteRecipe(int plateCode) {
        for (int i = 0; i < RecipeCodes.Length; i++) {
            if (RecipeCodes[i] == plateCode) {
                return LevelRecipe.Recipes[i];
            }
        }
        return null;
    }

    public void MergeFoodCode(ref int origin, int other) {
        int result = 0;
        for (byte i = 0; i < 32 / FoodSortCodeBits; i++) {
            int otherCode = ((3 << (i * FoodSortCodeBits)) & other) >> (i * FoodSortCodeBits);
            int originCode = ((3 << (i * FoodSortCodeBits)) & origin) >> (i * FoodSortCodeBits);

            int finalCode = Mathf.Min(otherCode + originCode, (int)(Mathf.Pow(2, FoodSortCodeBits) - 1));
            result += finalCode << (i * FoodSortCodeBits);
        }
        origin = result;
    }
}
