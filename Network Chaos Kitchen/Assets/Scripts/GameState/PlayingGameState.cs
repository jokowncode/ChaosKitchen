
using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class PlayingGameState : BaseGameState {

    [Header("Playing")] 
    [SerializeField] private float EndingTime = 240.0f;
    [SerializeField] private Image EndingTimeImage;
    
    private NetworkVariable<float> Timer;
    private BaseGameState EndingGameState;

    private void Awake() {
        EndingGameState = GetComponent<EndingGameState>();
        Timer = new NetworkVariable<float>(EndingTime);
    }

    public override void Construct() {
        this.Timer.Value = this.EndingTime;
    }

    public override void Execute() {
        Timer.Value -= Time.deltaTime;
        PlayingGameExecuteClientRpc();
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void PlayingGameExecuteClientRpc() {
        float ratio = Timer.Value / this.EndingTime;
        this.EndingTimeImage.fillAmount = ratio;
    }

    public override void Transition() {
        if (this.Timer.Value > 0.0f) return;
        PlayingEndClientRpc();
        GameManager.Instance.ChangeState(this.EndingGameState);
    }
    
    [Rpc(SendTo.ClientsAndHost)]
    private void PlayingEndClientRpc() {
        SfxManager.Instance.PlaySound(SFXType.Timeout, this.transform.position);
    }

    public override void Destruct() {
        PlayingDestructClientRpc();
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void PlayingDestructClientRpc() {
        this.EndingTimeImage.fillAmount = 1.0f;
    }
}
