
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyButton : MonoBehaviour {

    [SerializeField] private TextMeshProUGUI LobbyNameText;
    [SerializeField] private Button EnterLobby;

    public Action OnClickEnterLobby;
    
    private void Awake() {
        EnterLobby.onClick.AddListener(() => {
            OnClickEnterLobby?.Invoke();
        });
    }

    public void SetName(string lobbyName) {
        this.LobbyNameText.text = lobbyName;
    }
}

