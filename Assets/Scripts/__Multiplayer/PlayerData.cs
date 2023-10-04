using System;
using Unity.Collections;
using Unity.Netcode;

public struct PlayerData : IEquatable<PlayerData>, INetworkSerializable
{
    public ulong clientId;      //for multiplayer
    public int colorId;
    public FixedString32Bytes playerName;
    public FixedString32Bytes playerId; //for lobby

    public readonly bool Equals(PlayerData other)
    {
        return clientId == other.clientId 
            && colorId == other.colorId
            && playerName == other.playerName
            && playerId == other.playerId;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref clientId);
        serializer.SerializeValue(ref colorId);
        serializer.SerializeValue(ref playerName);
        serializer.SerializeValue(ref playerId);
    }
}
