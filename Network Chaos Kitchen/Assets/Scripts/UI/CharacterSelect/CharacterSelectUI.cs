using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class CharacterSelectUI : MonoBehaviour {

    [SerializeField] private TextMeshProUGUI ReadyButtonText;
    [SerializeField] private CharacterSelectColorUI[] PlayerSelectColors;
    [SerializeField] private TextMeshProUGUI LobbyNameText;
    [SerializeField] private TextMeshProUGUI LobbyCodeText;

    private bool IsReady;

    private void Start() {
        Color clientColor = NetworkGameManager.Instance.GetPlayerColor();
        Color[] availableColors = NetworkGameManager.Instance.AvailableColors;
        for (int i = 0; i < PlayerSelectColors.Length; i++) {
            PlayerSelectColors[i].SetColor(availableColors[i], i);
            PlayerSelectColors[i].OnSelected += OnColorSelected;
            if (availableColors[i] == clientColor) {
                PlayerSelectColors[i].ToggleSelected(true);
            }
        }
        
        LobbyNameText.text = NetworkLobbyManager.Instance.GetLobbyName();
        LobbyCodeText.text = NetworkLobbyManager.Instance.GetLobbyCode();
    }

    private void OnColorSelected(int selectedIndex) {
        for (int i = 0; i < PlayerSelectColors.Length; i++) {
            PlayerSelectColors[i].ToggleSelected(i == selectedIndex);
        }
        NetworkGameManager.Instance.PlayerSelectColor(selectedIndex);
    }
    
    public void Ready() {
        IsReady = !IsReady;
        if (IsReady) {
            ReadyButtonText.text = "Cancel";
        } else {
            ReadyButtonText.text = "Ready";
        }
        NetworkGameManager.Instance.ClientReady();
    }

    public void MainMenu() {
        GameManager.Instance.GoBackToMainMenu();
    }
}


