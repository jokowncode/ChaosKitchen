
using System;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class CountDownGameState : BaseGameState {

    [Header("Countdown")] 
    [SerializeField] private float CountdownTime = 3.0f;
    [SerializeField] private TextMeshProUGUI CountdownText;
    [SerializeField] private Animator CountdownAnimator;
    
    private int PreNumber = -1;
    private BaseGameState PlayingGameState;
    private NetworkVariable<float> Timer;
    
    private void Awake() {
        PlayingGameState = GetComponent<PlayingGameState>();
        Timer = new NetworkVariable<float>(CountdownTime);
    }

    public override void Construct() {
        this.Timer.Value = this.CountdownTime;
        CountDownGameConstructClientRpc();
    }
    
    [Rpc(SendTo.ClientsAndHost)]
    private void CountDownGameConstructClientRpc() {
        this.PreNumber = -1;
        this.CountdownAnimator.SetBool(AnimationParams.Show, true);
    }

    public override void Execute() {
        Timer.Value -= Time.deltaTime;
        if (Timer.Value <= 0.0f) return;
        CountDownExecuteClientRpc();
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void CountDownExecuteClientRpc() {
        int number = Mathf.CeilToInt(this.Timer.Value);
        this.CountdownText.text = number.ToString();
        if (PreNumber != number) {
            this.CountdownAnimator.SetTrigger(AnimationParams.Change); 
        }
        PreNumber = number;
    }

    public override void Transition() {
        if (Timer.Value > 0.0f) return;
        CountDownEndClientRpc();
        GameManager.Instance.ChangeState(this.PlayingGameState);
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void CountDownEndClientRpc() {
        this.CountdownAnimator.SetBool(AnimationParams.Show, false);
    }
}


