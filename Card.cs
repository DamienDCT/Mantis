using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;

[System.Serializable]
public struct Card : IEquatable<Card>, INetworkSerializable
{
    public CardColor colorCard;
    public int cardId;

    public CardColor topColorCard;
    public CardColor middleColorCard;
    public CardColor bottomColorCard;

    bool IEquatable<Card>.Equals(Card other)
    {
        return this.cardId == other.cardId;
    }

    void INetworkSerializable.NetworkSerialize<T>(BufferSerializer<T> serializer)
    {
        serializer.SerializeValue(ref colorCard);
        serializer.SerializeValue(ref cardId);
        serializer.SerializeValue(ref topColorCard);
        serializer.SerializeValue(ref middleColorCard);
        serializer.SerializeValue(ref bottomColorCard);
    }

    public void GenerateOtherColors()
    {
        List<CardColor> colorCards = CardDictionary.Instance.GiveTwoRandomColors(colorCard);
        colorCards.Shuffle();
        topColorCard = colorCards[0];
        middleColorCard = colorCards[1];
        bottomColorCard = colorCards[2];
    }
}