
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class CanvasManager : MonoBehaviour {
        
    [SerializeField] private CanvasGroup OrderCanvas;
    [SerializeField] private CanvasGroup HUDCanvas;
    [SerializeField] private StaticsUI StaticCanvas;
    [SerializeField] private Animator SceneChangeAnimator;
    [SerializeField] private PauseCanvasUI PauseCanvas;
    [SerializeField] private OptionUI OptionCanvas;
    [SerializeField] private Canvas IndicatorCanvas;
    [SerializeField] private NetworkGameMessageUI NetworkMessage;
    [SerializeField] private NotificationUI Notification;
    
    public static CanvasManager Instance { get; private set; }

    public Action OnHideOption;

    private bool IsPaused;

    private void Awake() {
        if (Instance != null) {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    private void Start() {
        NetworkGameManager.Instance.OnSceneChange += OnSceneChange;
        NetworkGameManager.Instance.OnPausePlayerCountChanged += OnPausePlayerCountChanged;
        InputManager.Instance.OnPauseGame += PauseGame;
    }

    private void OnSceneChange() {
        IsPaused = false;
        ToggleSceneChange(false);
        PauseCanvas.Toggle(false);
        SetGamePlayUIAlpha(0.0f);
        HideNotification();
    }

    private void ShowFullScreenUI() {
        Application.targetFrameRate = 60;
        NetworkGameManager.Instance.MainCamera.cullingMask = 1 << LayerMask.NameToLayer("UI");
        Time.timeScale = 0.0f;
        SetGamePlayUIAlpha(0.0f);
    }

    private void HideFullScreenUI(bool showGamePlayUI) {
        Application.targetFrameRate = -1;
        NetworkGameManager.Instance.MainCamera.cullingMask = ~0;
        Time.timeScale = 1.0f;
        if(showGamePlayUI) SetGamePlayUIAlpha(1.0f);
    }

    public void ShowStaticCanvas(LevelInfoSO levelInfo, int score, int orderComplete) {
        ShowFullScreenUI();
        StaticCanvas.ShowStaticsUI(levelInfo, score, orderComplete);
    }

    public void HideStaticCanvas() {
        HideFullScreenUI(false);
        StaticCanvas.HideStaticsUI();
    }
    
    private void ShowOptionCanvas() {
        float mainMusicVolume = MainMusicManager.Instance.CurrentMainMusicVolume;
        OptionCanvas.ShowOption(mainMusicVolume);
    }

    private void HideOptionCanvas() {
        OptionCanvas.HideOption();
    }

    public void ToggleSceneChange(bool fadeOut) {
        SceneChangeAnimator.SetBool(AnimationParams.Out, fadeOut);
    }

    public void SetGamePlayUIAlpha(float alpha) {
        HUDCanvas.alpha = alpha;
        OrderCanvas.alpha = alpha;
    }

    public Transform GetOrderCanvasTransform() {
        return this.OrderCanvas.transform;
    }

    public RectTransform GetIndicatorCanvasTransform() {
        return (RectTransform)this.IndicatorCanvas.transform;
    }

    private void OnPausePlayerCountChanged(int count) {
        if (count > 0) {
            if (IsPaused) return;
            Time.timeScale = 0.0f;
            Notification.ShowNotification("Other Players are Paused, Please Waiting...");
        } else {
            Notification.HideNotification();
            HideFullScreenUI(true);
        }
    }
    
    private void PauseGame() {
        if (IsPaused) return;
        IsPaused = true;
        Notification.HideNotification();
        ShowFullScreenUI();
        PauseCanvas.Toggle(true);
        NetworkGameManager.Instance.PauseGameRpc();
    }
    
    public void ResumeGameNotifyServer() {
        IsPaused = false;
        PauseCanvas.Toggle(false);
        NetworkGameManager.Instance.MainCamera.cullingMask = ~0;
        SetGamePlayUIAlpha(1.0f);
        NetworkGameManager.Instance.ResumeGameRpc();
    }
    
    public void ShowOption() {
        if (SceneManager.GetActiveScene().buildIndex != (int)Level.MainMenu) {
            PauseCanvas.Toggle(false);
        }
        ShowOptionCanvas();
        InputManager.Instance.TogglePlayerInput(false);
    }

    public void HideOption() {
        HideOptionCanvas();
        if (SceneManager.GetActiveScene().buildIndex != (int)Level.MainMenu) {
            PauseCanvas.Toggle(true);
        }
        InputManager.Instance.TogglePlayerInput(true);
        OnHideOption?.Invoke();
    }

    public void ShowNetworkMessage(string message) {
        ShowFullScreenUI();
        NetworkMessage.Show(message);
    }

    public void HideNetworkMessage() {
        HideFullScreenUI(false);
        NetworkMessage.Hide();
    }

    public void ShowNotification(string message = "Connecting...") {
        Notification.ShowNotification(message);
    }

    public void HideNotification() {
        Notification.HideNotification();
    }
}
