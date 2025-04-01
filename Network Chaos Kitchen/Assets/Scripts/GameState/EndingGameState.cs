
using System;
using Unity.Netcode;
using UnityEngine;

public class EndingGameState : BaseGameState {
    
    [Header("Ending")]
    [SerializeField] private GameObject TimeoutUI;
    [SerializeField] private float TimeoutWaitTime = 1.0f;

    private NetworkVariable<float> Timer;
    private BaseGameState StaticGameState;

    private void Awake() {
        StaticGameState = GetComponent<StaticGameState>();
        Timer = new NetworkVariable<float>(TimeoutWaitTime);
    }

    public override void Construct() {
        this.Timer.Value = TimeoutWaitTime;
        EndingGameConstructClientRpc();
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void EndingGameConstructClientRpc() {
        TimeoutUI.gameObject.SetActive(true);
    }
    
    public override void Execute() {
        Timer.Value -= Time.deltaTime;
    }
    
    public override void Transition() {
        if (this.Timer.Value > 0.0f) return;
        EndingGameEndClientRpc();
        GameManager.Instance.ChangeState(this.StaticGameState);
    }
    
    [Rpc(SendTo.ClientsAndHost)]
    private void EndingGameEndClientRpc() {
        TimeoutUI.gameObject.SetActive(false);
    }
}

