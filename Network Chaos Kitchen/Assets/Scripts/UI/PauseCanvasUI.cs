
using System;
using UnityEngine;
using UnityEngine.UI;

public class PauseCanvasUI : MonoBehaviour {

    [SerializeField] private Animator PauseAnimator;
    [SerializeField] private Button RestartButton;

    public void Toggle(bool show) {
        RestartButton.gameObject.SetActive(NetworkGameManager.Instance.IsHost);
        PauseAnimator.SetBool(AnimationParams.Show, show);
    }
}

