
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChange : MonoBehaviour {
    public void GoToTargetScene() {
        GameManager.Instance.GoToTargetScene();
    }

    public void InScene() {
        if (SceneManager.GetActiveScene().buildIndex < (int)Level.Level1_1) return;
        NetworkGameManager.Instance.PlayerInSceneServerRpc();
    }
}
