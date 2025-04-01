using System;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSlotVF : MonoBehaviour {

    [SerializeField] private TextMeshProUGUI ReadyText;
    [SerializeField] private Button KickButton;
    [SerializeField] private PlayerVisual Player;
    [SerializeField] private Image CurrentPlayerIdentifier;

    private int PlayerIndex;
    
    private void Awake() {
        KickButton.onClick.AddListener(() => {
            NetworkGameManager.Instance.KickPlayer(this.PlayerIndex);
        });
    }

    public void UpdateVisual(int index) {
        this.PlayerIndex = index;
        if (!NetworkGameManager.Instance.IsPlayerConnected(index)) {
            this.gameObject.SetActive(false);
            return;
        }
        this.gameObject.SetActive(true);
        KickButton.gameObject.SetActive(NetworkManager.Singleton.IsHost);

        Color playerColor = NetworkGameManager.Instance.GetPlayerColor(index);
        Player.SetColor(playerColor);
        CurrentPlayerIdentifier.color = playerColor;
        CurrentPlayerIdentifier.gameObject.SetActive(NetworkGameManager.Instance.IsCurrentPlayer(index));
        ReadyText.color = playerColor;
        ReadyText.gameObject.SetActive(NetworkGameManager.Instance.IsPlayerReady(index));
    }
}


