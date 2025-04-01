
using System;
using UnityEngine;
using UnityEngine.Pool;

public class PoolManager : MonoBehaviour {

    [Header("Prefab")] 
    [SerializeField] private ScoreChangeTipMove ScoreChangeUIPrefab;
    [SerializeField] private OrderUI OrderUIPrefab;
    [SerializeField] private DurCookingUI CookingUIPrefab;
    [SerializeField] private FoodListUI FoodListUIPrefab;
    [SerializeField] private ProgressBar ProgressBarUIPrefab;
    
    public static PoolManager Instance { get; private set; }
    
    private ObjectPool<OrderUI> OrderUIPool;
    private ObjectPool<ScoreChangeTipMove> ScoreChangeUIPool;
    private ObjectPool<DurCookingUI> CookingUIPool;
    private ObjectPool<FoodListUI> FoodListUIPool;
    private ObjectPool<ProgressBar> ProgressBarUIPool;
    
    private void Awake() {
        if (Instance != null) {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this.gameObject);
        
        OrderUIPool = new ObjectPool<OrderUI>(CreateOrderUI, null, null, null, false);
        ScoreChangeUIPool = new ObjectPool<ScoreChangeTipMove>(CreateScoreChangeUI, OnGetScoreChangeUI,
            OnReleaseScoreChangeUI, null, false);
        CookingUIPool = new ObjectPool<DurCookingUI>(CreateCookingUI, null, 
            null, null, false);
        FoodListUIPool = new ObjectPool<FoodListUI>(CreateFoodListUI, null, 
            null, null, false);
        ProgressBarUIPool = new ObjectPool<ProgressBar>(CreateProgressBarUI, null,
            null, null, false);
    }

    private ProgressBar CreateProgressBarUI() {
        return Instantiate(ProgressBarUIPrefab, CanvasManager.Instance.GetIndicatorCanvasTransform());
    }
    
    private FoodListUI CreateFoodListUI() {
        return Instantiate(FoodListUIPrefab, CanvasManager.Instance.GetIndicatorCanvasTransform());
    }

    private DurCookingUI CreateCookingUI() {
        return Instantiate(CookingUIPrefab, CanvasManager.Instance.GetIndicatorCanvasTransform());
    }

    private void Start() {
        // Prewarm OrderUI
        for (int i = 0; i < 5; i++) {
            OrderUIPool.Get().Hide();
        }
    }

    private void OnReleaseScoreChangeUI(ScoreChangeTipMove obj) {
        obj.gameObject.SetActive(false);
    }

    private void OnGetScoreChangeUI(ScoreChangeTipMove obj) {
        obj.gameObject.SetActive(true);
    }

    private ScoreChangeTipMove CreateScoreChangeUI() {
        return Instantiate(ScoreChangeUIPrefab, GameManager.Instance.GetScoreTipTransform());
    }
    
    private OrderUI CreateOrderUI() {
        return Instantiate(OrderUIPrefab, CanvasManager.Instance.GetOrderCanvasTransform());
    }
    
    public ScoreChangeTipMove GetScoreChangeUI() {
        return ScoreChangeUIPool.Get();
    }

    public void ReleaseScoreChangeUI(ScoreChangeTipMove obj) {
        ScoreChangeUIPool.Release(obj);
    }
    
    public OrderUI GetOrderUI() {
        return OrderUIPool.Get();
    }

    public void ReleaseOrderUI(OrderUI obj) {
        OrderUIPool.Release(obj);
    }
    
    public DurCookingUI GetCookingUI() {
        return CookingUIPool.Get();
    }

    public void ReleaseCookingUI(DurCookingUI obj) {
        CookingUIPool.Release(obj);
    }
    
    public FoodListUI GetFoodListUI() {
        return FoodListUIPool.Get();
    }

    public void ReleaseFoodListUI(FoodListUI obj) {
        FoodListUIPool.Release(obj);
    }
    
    public ProgressBar GetProgressBarUI() {
        return ProgressBarUIPool.Get();
    }

    public void ReleaseProgressBarUI(ProgressBar obj) {
        ProgressBarUIPool.Release(obj);
    }
}
