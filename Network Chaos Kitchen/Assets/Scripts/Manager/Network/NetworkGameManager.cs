using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkGameManager : NetworkBehaviour {

    [SerializeField] private int MaxPlayer = 4;
    [SerializeField] private Player PlayerPrefab;
    
    [SerializeField] private Color PlayerInitialColor;
    [field: SerializeField] public Color[] AvailableColors { get; private set; }

    public static NetworkGameManager Instance { get; private set; }
    
    public Camera MainCamera { get; private set; }
    public Action OnSceneChange;
    
    public Action OnClientAfterDisconnect;
    
    private NetworkList<ulong> ReadyClients;
    private NetworkList<PlayerNetworkData> Players;
    private NetworkVariable<ulong> DisconnectCount;
    private NetworkVariable<int> PauseGamePlayerCount;
    private NetworkVariable<int> InScenePlayerCount;
    
    public Action OnPlayerChanged;
    public Action OnPlayerReadyChanged;
    public Action<int> OnPausePlayerCountChanged;

    private int CurrentClientIndex;
    private SpawnPoint CurrentLevelSpawnPoint;

    private bool Multiplayer;

    private void Awake() {
        if (Instance != null) {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this.gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
        
        Players = new NetworkList<PlayerNetworkData>();
        ReadyClients = new NetworkList<ulong>();
        DisconnectCount = new NetworkVariable<ulong>();
        PauseGamePlayerCount = new NetworkVariable<int>();
        InScenePlayerCount = new NetworkVariable<int>();
        
        Players.OnListChanged += OnPlayerListChanged;
        ReadyClients.OnListChanged += OnPlayerReadyListChanged;
        PauseGamePlayerCount.OnValueChanged += OnPausePlayerChanged;
        InScenePlayerCount.OnValueChanged += OnInScenePlayerCountChanged;
    }

    private void OnInScenePlayerCountChanged(int previousValue, int newValue) {
        if (GameManager.Instance.IsPlaying) return;
        if (this.InScenePlayerCount.Value == NetworkManager.ConnectedClientsIds.Count) {
            GameManager.Instance.ResetRound();
            CanvasManager.Instance.HideNotification();
        } else {
            CanvasManager.Instance.ShowNotification("Wait Other Player...");
        }
    }

    private void OnPausePlayerChanged(int previousValue, int newValue) {
        OnPausePlayerCountChanged?.Invoke(newValue);
    }

    public int GetMaxPlayer() {
        return this.MaxPlayer;
    }

    private void OnPlayerReadyListChanged(NetworkListEvent<ulong> changeEvent) {
        OnPlayerReadyChanged?.Invoke();
    }

    private void OnPlayerListChanged(NetworkListEvent<PlayerNetworkData> changeEvent) {
        if (changeEvent.Type != NetworkListEvent<PlayerNetworkData>.EventType.Value) {
            this.CurrentClientIndex = GetPlayerIndex();
        }
        OnPlayerChanged?.Invoke();
    }

    private void ClientDisconnectCallback(ulong id) {
        if (IsHost) {
            DisconnectCount.Value += 1;
            PauseGamePlayerCount.Value -= 1;
            InScenePlayerCount.Value -= 1;
            RemoveConnectPlayer(id);
            if (ReadyClients.Contains(id)) {
                ReadyClients.Remove(id);
            }
            return;
        }
        
        if (id == NetworkManager.ServerClientId + DisconnectCount.Value + 1) {
            CanvasManager.Instance.ShowNetworkMessage("Host Is Disconnected...");
            return;
        }

        if (id == NetworkManager.LocalClientId) {
            OnClientAfterDisconnect?.Invoke();
        }
    }

    private void RemoveConnectPlayer(ulong clientId) {
        for (int i = 0; i < Players.Count; i++) {
            if (Players[i].ClientId != clientId) continue;
            this.Players.RemoveAt(i);
            break;
        }
    }

    private void ClientConnectCallback(ulong id) {
        if (id == NetworkManager.LocalClientId) {
            this.Players.Clear();
            PauseGamePlayerCount.Value = 0;
            DisconnectCount.Value = 0;
        }

        int colorIndex = this.Players.Count;
        this.Players.Add(new PlayerNetworkData() {
            ClientId = id,
            PlayerColor = this.AvailableColors[colorIndex]
        });
        
        if (!this.Multiplayer) {
            SpawnPlayer();
        }
        ClientConnectClientRpc(this.Multiplayer, RpcTarget.Single(id, RpcTargetUse.Temp));
    }

    [Rpc(SendTo.SpecifiedInParams)]
    private void ClientConnectClientRpc(bool multiplayer, RpcParams _) {
        if (multiplayer) {
            SceneManager.LoadScene((int)Level.CharacterSelect);    
        } else {
            GameManager.Instance.GoToLevel(Level.LevelChoose);   
        }
    }

    private void ConnectionApprovalCallback(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response) {
        if (NetworkManager.ConnectedClientsList.Count == MaxPlayer) {
            response.Approved = false;
            response.Reason = "Room Is Full!";
            return;
        }
        if (SceneManager.GetActiveScene().buildIndex >= (int)Level.Level1_1) {
            response.Approved = false;
            response.Reason = "Room Is Playing!";
            return;
        }
        response.Approved = true;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        if (scene.buildIndex == (int)Level.StartGameLoading) {
            SceneManager.LoadScene((int)Level.MainMenu);
            return;
        }

        if (scene.buildIndex >= (int)Level.Level1_1) {
            this.CurrentLevelSpawnPoint = FindAnyObjectByType<SpawnPoint>();
            if (IsHost) {
                InScenePlayerCount.Value = 0;
            }
        }

        SceneLoadedSetState();
        if (scene.buildIndex != (int)Level.CharacterSelect || !IsHost) return;
        ReadyClients.Clear();
    }

    private void SceneLoadedSetState() {
        MainCamera = Camera.main;
        MainMusicManager.Instance.PlayMainMusic();
        Time.timeScale = 1.0f;
        Application.targetFrameRate = -1;
        MainMusicManager.Instance.MainMusicFadeIn();
        OnSceneChange?.Invoke();
    }

    public void SinglePlayerStartGame() {
        Multiplayer = false;
        // TODO: Lobby
        // NetworkLobbyManager.Instance.CreateLobby("SinglePlayer", true);
        ConnectServer(true);
    }

    public void MultiplayerStartGame() {
        Multiplayer = true;
        GameManager.Instance.GoToLevel(Level.Connection);
    }

    public void ConnectServer(bool isHost) {
        NetworkManager.OnClientDisconnectCallback += ClientDisconnectCallback;
        if (isHost) {
            NetworkManager.ConnectionApprovalCallback = ConnectionApprovalCallback;
            NetworkManager.OnClientConnectedCallback += ClientConnectCallback;
            NetworkManager.StartHost();
        } else {
            NetworkManager.StartClient();
        }
    }
    
    public void DisConnectServer() {
        if (!IsClient && !IsHost) {
            return;
        }
        NetworkManager.OnClientDisconnectCallback -= ClientDisconnectCallback;
        if (IsHost) {
            NetworkLobbyManager.Instance.DeleteLobby();
            NetworkManager.OnClientConnectedCallback -= ClientConnectCallback;
            NetworkManager.ConnectionApprovalCallback = null;
        } else {
            NetworkLobbyManager.Instance.RemovePlayer();
        }
        NetworkManager.Shutdown();
    }

    public void ClientReady() {
        ClientReadyServerRpc();        
    }

    [Rpc(SendTo.Server)]
    private void ClientReadyServerRpc(RpcParams rpcParams = default) {
        ulong id = rpcParams.Receive.SenderClientId;
        if (ReadyClients.Contains(id)) {
            ReadyClients.Remove(id);
        } else {
            ReadyClients.Add(id);
        }

        if (ReadyClients.Count == NetworkManager.ConnectedClientsIds.Count) {
            ClientAllReadyClientRpc();            
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void ClientAllReadyClientRpc() {
        SpawnPlayer();
        if (IsHost) {
            GameManager.Instance.GoToLevel(Level.LevelChoose);
        } else {
            CanvasManager.Instance.ShowNotification("Waiting Host Choose Level...");   
        }
    }

    private void SpawnPlayer() {
        SpawnPlayerServerRpc();
    }

    [Rpc(SendTo.Server)]
    private void SpawnPlayerServerRpc(RpcParams rpcParams = default) {
        Player player = Instantiate(PlayerPrefab);
        player.GetComponent<NetworkObject>().SpawnAsPlayerObject(rpcParams.Receive.SenderClientId);
    }

    public bool IsPlayerConnected(int index) {
        return index < this.Players.Count;
    }

    public bool IsPlayerReady(int index) {
        ulong clientId = Players[index].ClientId;
        return ReadyClients.Contains(clientId);
    }

    public void KickPlayer(int playerIndex) {
        ulong clientId = Players[playerIndex].ClientId;
        KickPlayerClientRpc(RpcTarget.Single(clientId, RpcTargetUse.Temp));
    }

    [Rpc(SendTo.SpecifiedInParams)]
    private void KickPlayerClientRpc(RpcParams _) {
        DisConnectServer();
        CanvasManager.Instance.ShowNetworkMessage("You are Kicked!");
    }

    public Color GetPlayerColor() {
        return GetPlayerColor(this.CurrentClientIndex);
    }

    public Color GetPlayerColor(int index) {
        if (index >= Players.Count || index < 0) {
            return PlayerInitialColor;
        }
        return Players[index].PlayerColor;
    }

    private int GetPlayerIndex() {
        return GetPlayerIndex(NetworkManager.LocalClientId);
    }

    public int GetPlayerIndex(ulong clientId) {
        for (int i = 0; i < Players.Count; i++) {
            if (Players[i].ClientId == clientId) {
                return i;
            }
        }
        return -1;
    }

    public void PlayerSelectColor(int selectColorIndex) {
        PlayerSelectColorServerRpc(this.CurrentClientIndex, selectColorIndex);
    }

    [Rpc(SendTo.Server)]
    private void PlayerSelectColorServerRpc(int clientIndex, int selectColorIndex) {
        PlayerNetworkData data = this.Players[clientIndex];
        data.PlayerColor = this.AvailableColors[selectColorIndex];
        this.Players[clientIndex] = data;
    }

    public bool IsCurrentPlayer(int index) {
        return this.CurrentClientIndex == index;
    }

    public Vector3 GetPlayerPosition(int index) {
        return this.CurrentLevelSpawnPoint == null ? Vector3.zero : this.CurrentLevelSpawnPoint.GetSpawnPoint(index);
    }

    [Rpc(SendTo.Server)]
    public void PauseGameRpc() {
        this.PauseGamePlayerCount.Value += 1;
    }

    [Rpc(SendTo.Server)]
    public void ResumeGameRpc() {
        this.PauseGamePlayerCount.Value -= 1;
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void GoToLevelClientRpc(Level level) {
        GameManager.Instance.GoToLevel(level);
    }

    [Rpc(SendTo.Server)]
    public void PlayerInSceneServerRpc() {
        InScenePlayerCount.Value += 1;
    }
}


