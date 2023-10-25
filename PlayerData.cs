using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.Netcode;
using Unity.Collections;

public struct PlayerData : IEquatable<PlayerData>, INetworkSerializable
{
    public ulong clientId;
    public int colorId;
    public FixedString32Bytes username;

    bool IEquatable<PlayerData>.Equals(PlayerData other)
    {
         return clientId == other.clientId && colorId == other.colorId && username == other.username;
    }

    void INetworkSerializable.NetworkSerialize<T>(BufferSerializer<T> serializer)
    {
        serializer.SerializeValue(ref clientId);
        serializer.SerializeValue(ref colorId);
        serializer.SerializeValue(ref username);
    }
}
