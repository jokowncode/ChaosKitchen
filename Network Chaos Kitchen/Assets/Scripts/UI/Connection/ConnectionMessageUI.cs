
using TMPro;
using UnityEngine;

public class ConnectionMessageUI : MonoBehaviour {

    [SerializeField] private TextMeshProUGUI Message;

    public void Show(string message) {
        Message.text = message;
        this.gameObject.SetActive(true);
    }

    public void Hide() {
        this.gameObject.SetActive(false);
    }

}

