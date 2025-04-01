
using TMPro;
using UnityEngine;

public class NetworkGameMessageUI : MonoBehaviour {

    [SerializeField] private TextMeshProUGUI Message;

    public void Show(string message) {
        this.Message.text = message;
        this.gameObject.SetActive(true);
    }

    public void Hide() {
        this.gameObject.SetActive(false);
    }

}



