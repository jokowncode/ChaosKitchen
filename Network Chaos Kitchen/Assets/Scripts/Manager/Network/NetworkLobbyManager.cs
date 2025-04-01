
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

public class NetworkLobbyManager : MonoBehaviour {

    [SerializeField] private float HeartBeatInterval = 20.0f;
    [SerializeField] private float LobbyListRefreshInterval = 10.0f;
    
    public static NetworkLobbyManager Instance { get; private set; }

    private Lobby CurrentLobby;

    public Action<string> OnLobbyException;
    public Action<List<Lobby>> OnLobbyListChanged;

    private float HeartbeatTimer;
    private float LobbyListRefreshTimer;
    
    private const string RelayJoinCodeKey = "RelayJoinCode";
    
    private void Awake() {
        Instance = this;
        HeartbeatTimer = HeartBeatInterval;
        LobbyListRefreshTimer = LobbyListRefreshInterval;
        this.enabled = false;
        // TODO: Lobby
        // Initialize();
    }
    
    public string GetLobbyName() {
        return this.CurrentLobby?.Name;
    }

    public string GetLobbyCode() {
        return this.CurrentLobby?.LobbyCode;
    }

    private async void Initialize() {
        try {
            if (UnityServices.State != ServicesInitializationState.Uninitialized) {
                return;
            }
            CanvasManager.Instance.ShowNotification();
            await UnityServices.InitializeAsync();
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            CanvasManager.Instance.HideNotification();
            this.enabled = true;
        } catch (Exception e) {
            CanvasManager.Instance.HideNotification();
            CanvasManager.Instance.ShowNetworkMessage(e.Message);
        }
    }
    
    private void Update() {
        HandleHeartbeat();
        HandleLobbyList();
    }

    private void HandleLobbyList() {
        if (this.CurrentLobby != null || !AuthenticationService.Instance.IsSignedIn) return;
        LobbyListRefreshTimer -= Time.deltaTime;
        if (LobbyListRefreshTimer > 0.0f) return;
        LobbyListRefreshTimer = LobbyListRefreshInterval;
        QueryLobby();
    }

    private bool Validate() {
        if (this.CurrentLobby == null) return false;
        if (this.CurrentLobby.HostId != AuthenticationService.Instance.PlayerId) return false;
        return true;
    }

    private async void HandleHeartbeat() {
        try {
            HeartbeatTimer -= Time.deltaTime;
            if (HeartbeatTimer > 0.0f) return;
            if (!Validate()) return;
            HeartbeatTimer = HeartBeatInterval;
            await LobbyService.Instance.SendHeartbeatPingAsync(this.CurrentLobby.Id);
        } catch (Exception e) {
            CanvasManager.Instance.ShowNetworkMessage(e.Message);
        }
    }

    public async void CreateLobby(string lobbyName, bool isPrivate) {
        try {
            CanvasManager.Instance.ShowNotification();
            this.CurrentLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName,
                NetworkGameManager.Instance.GetMaxPlayer() - 1, new CreateLobbyOptions() {
                IsPrivate = isPrivate
            });
            Allocation allocation = await AllocateRelay();
            string joinCode = await GetRelayJoinCode(allocation.AllocationId);
            await LobbyService.Instance.UpdateLobbyAsync(this.CurrentLobby.Id, new UpdateLobbyOptions() {
                Data = new Dictionary<string, DataObject>() {
                    {RelayJoinCodeKey, new DataObject(DataObject.VisibilityOptions.Member, joinCode)}
                }
            });
            UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            transport.SetRelayServerData(allocation.ToRelayServerData("dtls"));
            NetworkGameManager.Instance.ConnectServer(true);
        } catch (Exception e) {
            CanvasManager.Instance.HideNotification();
            OnLobbyException?.Invoke(e.Message);
        }
    }
    
    public async void QuickJoinLobby() {
        try {
            CanvasManager.Instance.ShowNotification();
            this.CurrentLobby = await LobbyService.Instance.QuickJoinLobbyAsync();
            
            JoinAllocation allocation = await JoinRelay(this.CurrentLobby.Data[RelayJoinCodeKey].Value);
            UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            transport.SetRelayServerData(allocation.ToRelayServerData("dtls"));
            
            NetworkGameManager.Instance.ConnectServer(false);
        } catch (Exception e) {
            CanvasManager.Instance.HideNotification();
            OnLobbyException?.Invoke(e.Message);
        }
    }

    public async void JoinLobbyByCode(string lobbyCode) {
        try {
            CanvasManager.Instance.ShowNotification();
            this.CurrentLobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode);
            
            JoinAllocation allocation = await JoinRelay(this.CurrentLobby.Data[RelayJoinCodeKey].Value);
            UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            transport.SetRelayServerData(allocation.ToRelayServerData("dtls"));
            
            NetworkGameManager.Instance.ConnectServer(false);
        } catch (Exception e) {
            CanvasManager.Instance.HideNotification();
            OnLobbyException?.Invoke(e.Message);
        }
    }

    public async void JoinLobbyById(string lobbyId) {
        try {
            CanvasManager.Instance.ShowNotification();
            this.CurrentLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobbyId);
            
            JoinAllocation allocation = await JoinRelay(this.CurrentLobby.Data[RelayJoinCodeKey].Value);
            UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            transport.SetRelayServerData(allocation.ToRelayServerData("dtls"));
            
            NetworkGameManager.Instance.ConnectServer(false);
        } catch (Exception e) {
            CanvasManager.Instance.HideNotification();
            OnLobbyException?.Invoke(e.Message);
        }
    }
    
    public async void DeleteLobby() {
        try {
            if (!Validate()) return;
            await LobbyService.Instance.DeleteLobbyAsync(this.CurrentLobby.Id);
            this.CurrentLobby = null;
        } catch (Exception e) {
            CanvasManager.Instance.ShowNetworkMessage(e.Message);
        }
    }

    public async void RemovePlayer() {
        try {
            if (this.CurrentLobby == null) return;
            await LobbyService.Instance.RemovePlayerAsync(this.CurrentLobby.Id, AuthenticationService.Instance.PlayerId);
            this.CurrentLobby = null;
        } catch (Exception e) {
            CanvasManager.Instance.ShowNetworkMessage(e.Message);
        }
    }

    private async void QueryLobby() {
        try {
            List<QueryFilter> filters = new List<QueryFilter> {
                new (QueryFilter.FieldOptions.AvailableSlots, "0", QueryFilter.OpOptions.GE)
            };

            QueryLobbiesOptions options = new QueryLobbiesOptions() {
                Filters = filters
            };
            QueryResponse response = await LobbyService.Instance.QueryLobbiesAsync(options);
            OnLobbyListChanged?.Invoke(response.Results);
        } catch (Exception e) {
            CanvasManager.Instance.HideNotification();
            OnLobbyException?.Invoke(e.Message);
        }
    }
    
    // -------------- Relay --------------
    private async Task<Allocation> AllocateRelay() {
        return await RelayService.Instance.CreateAllocationAsync(NetworkGameManager.Instance.GetMaxPlayer());
    }

    private async Task<string> GetRelayJoinCode(Guid allocationId) {
        return await RelayService.Instance.GetJoinCodeAsync(allocationId);
    }

    private async Task<JoinAllocation> JoinRelay(string joinCode) {
        return await RelayService.Instance.JoinAllocationAsync(joinCode);
    }
}

