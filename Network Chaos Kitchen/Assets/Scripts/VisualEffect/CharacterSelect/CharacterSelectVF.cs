using UnityEngine;

public class CharacterSelectVF : MonoBehaviour {

    [SerializeField] private PlayerSlotVF[] PlayerSlots;
    
    private void Start() {
        UpdatePlayerVisual();
        NetworkGameManager.Instance.OnPlayerChanged += OnPlayerChanged;
        NetworkGameManager.Instance.OnPlayerReadyChanged += OnPlayerChanged;
    }
    
    private void OnDestroy() {
        NetworkGameManager.Instance.OnPlayerChanged -= OnPlayerChanged;
        NetworkGameManager.Instance.OnPlayerReadyChanged -= OnPlayerChanged;
    }
    
    private void OnPlayerChanged() {
        UpdatePlayerVisual();
    }

    private void UpdatePlayerVisual() {
        for (int i = 0; i < PlayerSlots.Length; i++) {
            PlayerSlots[i].UpdateVisual(i);
        }
    }
}

