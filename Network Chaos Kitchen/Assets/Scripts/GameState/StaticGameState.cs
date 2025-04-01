
using Unity.Netcode;

public class StaticGameState : BaseGameState {
    public override void Construct() {
        StaticGameConstructClientRpc();
    }
    
    [Rpc(SendTo.ClientsAndHost)]
    private void StaticGameConstructClientRpc() {
        GameManager.Instance.ShowStatics();
    }

    public override void Transition() {
        GameManager.Instance.ChangeState(null);
    }
}

