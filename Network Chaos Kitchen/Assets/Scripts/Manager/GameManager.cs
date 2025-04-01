using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : NetworkBehaviour {

    [Header("Level Info")]
    [field : SerializeField] public LevelInfoSO[] LevelInfos { get; private set; }
    
    [Header("Score")] 
    [SerializeField] private TextMeshProUGUI ScoreText;
    [SerializeField] private int ErrorDishPunish = 10;
    [SerializeField] private int[] Fee;
    
    public static GameManager Instance { get; private set; }

    private const Level GameLevel = Level.Level1_1;
    private Level CurrentLevel = Level.StartGameLoading;

    private NetworkVariable<int> PlayerScore;
    private NetworkVariable<int> OrderComplete;
    
    private float Timer;
    private BaseGameState CurrentGameState;
    
    private int CurrentUnlockedLevel;
    
    private BaseGameState CountDownState;
    public bool IsPlaying;

    public void Restart() {
        this.CurrentGameState?.Destruct();
        GoToNextLevelClientRpc(this.CurrentLevel);
    }

    public void GoBackToMainMenu() {
        ClearGameState();
        CanvasManager.Instance.HideNetworkMessage();
        GoToLevel(Level.MainMenu);
        NetworkGameManager.Instance.DisConnectServer();
    }
    
    public void GoToNextLevel() {
        GoToNextLevelClientRpc(this.LevelInfos[GetCurrentLevelInfoIndex()].NextLevel);
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void GoToNextLevelClientRpc(Level level) {
        ClearGameState();
        GoToLevel(level);
    }

    private void ClearGameState() {
        CanvasManager.Instance.HideStaticCanvas();
        RecipeManager.Instance?.ClearOrder();
        ChangeState(null);
    }

    public void GoToLevel(Level level) {
        this.CurrentLevel = level;
        SetTargetScene(level);
    }
    
    private void SetTargetScene(Level level) {
        if (level == Level.StartGameLoading) return;
        CanvasManager.Instance.ToggleSceneChange(true);
        MainMusicManager.Instance.MainMusicFadeOut();
    }

    private int GetCurrentLevelInfoIndex() {
        return this.CurrentLevel - GameLevel;
    }

    public void GoToTargetScene() {
        if (this.CurrentLevel >= GameLevel) {
            if (!IsHost) return;
            NetworkManager.SceneManager.LoadScene(this.CurrentLevel.ToString(), LoadSceneMode.Single);
        } else {
            SceneManager.LoadScene((int)this.CurrentLevel);
        }
    }

    public void ResetRound() {
        CanvasManager.Instance.SetGamePlayUIAlpha(1.0f);
        ChangeState(this.CountDownState);
        if (!IsHost) return;
        this.PlayerScore.Value = 0;
        this.OrderComplete.Value = 0;
    }

    private void Awake() {
        if (Instance != null) {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this.gameObject);
        
        if (Fee == null || Fee.Length < 3) {
            Fee = new[] { 12, 20, 32 };
        }
        this.CountDownState = GetComponent<CountDownGameState>();
        this.CurrentGameState = null;

        PlayerScore = new NetworkVariable<int>();
        OrderComplete = new NetworkVariable<int>();
        PlayerScore.OnValueChanged += OnPlayerScoreChanged;
    }

    private void OnPlayerScoreChanged(int previousValue, int newValue) {
        this.ScoreText.text = this.PlayerScore.Value.ToString();
        if (!IsPlaying) return;
        ScoreChangeTipMove tip = PoolManager.Instance.GetScoreChangeUI();
        tip.UpdateScore(newValue - previousValue);
    }

    private void Start() {
        CanvasManager.Instance.SetGamePlayUIAlpha(0.0f);
        CanvasManager.Instance.OnHideOption += OnHideOption;
        ReadPlayerData();
    }
    
    private void OnHideOption() {
        // Save Audio Setting
        PlayerPrefs.SetFloat(PlayerPrefStringParams.MainMusicVolume, MainMusicManager.Instance.CurrentMainMusicVolume);
        PlayerPrefs.SetFloat(PlayerPrefStringParams.SfxMusicVolume, SfxManager.Instance.VolumeMultiplier);
        
        // Save Input Setting
        PlayerPrefs.SetString(PlayerPrefStringParams.PlayerCustomInput, InputManager.Instance.GetRebindingJson());
        PlayerPrefs.Save();
    }
    
    private void ReadPlayerData() {
        // Read Audio Setting
        MainMusicManager.Instance.SetMainMusicVolume(PlayerPrefs.GetFloat(PlayerPrefStringParams.MainMusicVolume, 1.0f));
        SfxManager.Instance.SfxVolumeChange(PlayerPrefs.GetFloat(PlayerPrefStringParams.SfxMusicVolume, 1.0f));
        
        // Read Input Setting
        if (PlayerPrefs.HasKey(PlayerPrefStringParams.PlayerCustomInput)) {
            InputManager.Instance.UseRebindingJson(PlayerPrefs.GetString(PlayerPrefStringParams.PlayerCustomInput));
        }
        this.CurrentUnlockedLevel = PlayerPrefs.GetInt(PlayerPrefStringParams.CurrentUnlockedLevel, (int)GameLevel);
    }

    public void ChangeState(BaseGameState state) {
        if (!IsHost) return;
        this.CurrentGameState?.Destruct();
        this.CurrentGameState = state;
        this.CurrentGameState?.Construct();
        this.IsPlaying = CurrentGameState is PlayingGameState;
        SyncPlayingStateClientRpc(this.IsPlaying);
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void SyncPlayingStateClientRpc(bool playing) {
        this.IsPlaying = playing;
    }

    private void Update() {
        if (!IsHost) return;
        if (!this.CurrentGameState) return;
        this.CurrentGameState.Execute();
        this.CurrentGameState.Transition();
    }

    public void OrderCompleteAddScore(int baseScore, int feeGrade) {
        OrderComplete.Value += 1;
        AddScore(baseScore + Fee[feeGrade]);
    }

    public void AddScore(int score) {
        this.PlayerScore.Value += score;
    }

    public void PlayerSubmitErrorDish() {
        AddScore(-this.ErrorDishPunish);
    }

    public Transform GetScoreTipTransform() {
        return this.ScoreText.transform.parent;
    }

    public bool LevelIsLocked(Level level) {
        if (level == Level.StartGameLoading) return true;
        return (int)level > CurrentUnlockedLevel;
    }

    public int GetLevelScore(Level level) {
        return LevelIsLocked(level) ? 0 : PlayerPrefs.GetInt(PlayerPrefStringParams.LevelScore(level), 0);
    }

    public void ShowStatics() {
        CanvasManager.Instance.ShowStaticCanvas(LevelInfos[GetCurrentLevelInfoIndex()], this.PlayerScore.Value, this.OrderComplete.Value);
        SaveLevelData();
    }

    private void SaveLevelData() {
        if (!IsHost) return;
        string levelScore = PlayerPrefStringParams.LevelScore(this.CurrentLevel);
        int historyScore = PlayerPrefs.GetInt(levelScore, 0);
        if (this.PlayerScore.Value > historyScore) {
            PlayerPrefs.SetInt(levelScore, this.PlayerScore.Value);
        }

        if ((int)this.CurrentLevel != this.CurrentUnlockedLevel ||
            LevelInfos[GetCurrentLevelInfoIndex()].PrimaryScore >= this.PlayerScore.Value) return;
        this.CurrentUnlockedLevel++;
        PlayerPrefs.SetInt(PlayerPrefStringParams.CurrentUnlockedLevel, this.CurrentUnlockedLevel);
    }
}
