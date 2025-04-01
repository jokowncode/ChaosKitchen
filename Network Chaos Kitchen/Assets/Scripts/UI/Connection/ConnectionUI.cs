using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class ConnectionUI : MonoBehaviour {

    [Header("Input")]
    [SerializeField] private TMP_InputField CodeInputField;

    [Header("Other UI")]
    [SerializeField] private CreateLobbySettingUI CreateLobbySetting;
    [SerializeField] private ConnectionMessageUI ConnectionMessage;

    [Header("Lobby List")] 
    [SerializeField] private LobbyButton LobbyButtonPrefab;
    [SerializeField] private GameObject LobbyListContainer;

    private float Timer;
    
    private void Awake() {
        CreateLobbySetting.Close();
        NetworkLobbyManager.Instance.OnLobbyException += OnLobbyException;
        NetworkGameManager.Instance.OnClientAfterDisconnect += OnClientDisconnect;
        NetworkLobbyManager.Instance.OnLobbyListChanged += OnLobbyListChanged;
    }

    private void OnDestroy() {
        NetworkLobbyManager.Instance.OnLobbyException -= OnLobbyException;
        NetworkLobbyManager.Instance.OnLobbyListChanged -= OnLobbyListChanged;
        NetworkGameManager.Instance.OnClientAfterDisconnect -= OnClientDisconnect;
    }

    private void OnLobbyListChanged(List<Lobby> lobbies) {
        foreach (Transform child in LobbyListContainer.transform) {
            Destroy(child.gameObject);
        }
        
        foreach (Lobby lobby in lobbies) {
            LobbyButton button = Instantiate(LobbyButtonPrefab, LobbyListContainer.transform);
            button.SetName(lobby.Name);
            button.OnClickEnterLobby += () => {
                NetworkLobbyManager.Instance.JoinLobbyById(lobby.Id);
            };
        }
    }
    
    private void OnLobbyException(string message) {
        ConnectionMessage.Show(message);
    }

    private void OnClientDisconnect() {
        ConnectionMessage.Show(NetworkManager.Singleton.DisconnectReason);
    }
    
    public void CreateRoom() {
        // TODO: Lobby
        // CreateLobbySetting.Show();
        
        NetworkGameManager.Instance.ConnectServer(true);
        GameManager.Instance.GoToLevel(Level.CharacterSelect);
    }

    public void QuickJoinRoom() {
        // TODO : Lobby
        // NetworkLobbyManager.Instance.QuickJoinLobby();
        
        NetworkGameManager.Instance.ConnectServer(false);
        GameManager.Instance.GoToLevel(Level.CharacterSelect);
    }

    public void JoinLobbyByCode() {
        string lobbyCode = CodeInputField.text;
        NetworkLobbyManager.Instance.JoinLobbyByCode(lobbyCode);
    }

    public void MainMenu() {
        GameManager.Instance.GoBackToMainMenu();
    }
}

