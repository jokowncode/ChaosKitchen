
using TMPro;
using UnityEngine;

public class CreateLobbySettingUI : MonoBehaviour {
    
    [SerializeField] private TMP_InputField LobbyNameInputField;

    public void Close() {
        this.gameObject.SetActive(false);
    }

    public void Show() {
        LobbyNameInputField.text = "Lobby Name";
        this.gameObject.SetActive(true);
    }

    public void CreatePrivateLobby() {
        string lobbyName = LobbyNameInputField.text;
        if (lobbyName == "") {
            lobbyName = "Lobby";
        }
        NetworkLobbyManager.Instance.CreateLobby(lobbyName,true);
    }

    public void CreatePublicLobby() {
        string lobbyName = LobbyNameInputField.text;
        if (lobbyName == "") {
            lobbyName = "Lobby";
        }
        NetworkLobbyManager.Instance.CreateLobby(lobbyName,false);
    }
}

