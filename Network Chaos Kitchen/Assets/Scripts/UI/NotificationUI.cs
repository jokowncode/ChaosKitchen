
using TMPro;
using UnityEngine;

public class NotificationUI : MonoBehaviour {
        
    [SerializeField] private TextMeshProUGUI NotificationText;

    public void ShowNotification(string notificationText = "Connecting...") {
        this.NotificationText.text = notificationText;
        this.gameObject.SetActive(true);
    }

    public void HideNotification() {
        this.gameObject.SetActive(false);
    }

}

