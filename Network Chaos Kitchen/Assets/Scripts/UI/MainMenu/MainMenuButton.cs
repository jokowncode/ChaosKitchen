using UnityEngine;

public class MainMenuButton : MonoBehaviour {
    
    public void QuitGame() {
        Application.Quit();
    }

    public void StartGame() {
        NetworkGameManager.Instance.SinglePlayerStartGame();
    }

    public void ShowOption() {
        CanvasManager.Instance.ShowOption();
    }

    public void MultiPlayer() {
        NetworkGameManager.Instance.MultiplayerStartGame();
    }

}
