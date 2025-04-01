
using System;
using Unity.Netcode;
using UnityEngine;

public struct PlayerNetworkData : IEquatable<PlayerNetworkData>, INetworkSerializable {

    public ulong ClientId;
    public Color PlayerColor;
    
    public bool Equals(PlayerNetworkData other) {
        return ClientId == other.ClientId && PlayerColor == other.PlayerColor;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter {
        serializer.SerializeValue(ref ClientId);
        serializer.SerializeValue(ref PlayerColor);
    }
}



