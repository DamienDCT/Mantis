using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;
using TMPro;

[System.Serializable]
public class Player : MonoBehaviour
{
    [SerializeField] private PlayerVisual playerVisual;

    [SerializeField] private CardInHand cardsInHand;

    [SerializeField] private Transform handPlayerTransform;

    [SerializeField] private int pointsAmount;

    [SerializeField] private TextMeshPro usernameText;
    [SerializeField] private TextMeshPro pointsAmountText;

    [SerializeField] private ulong playerLocalId;

    [SerializeField] private PlayerData playerData;

    [SerializeField] private ActionsPanelUI actionsPanelUI;

    private void Awake()
    {
        pointsAmount = 0;
        playerLocalId = 200000;
    }

    public void SetLocalClientId(ulong playerId)
    {
        this.playerLocalId = playerId;
    }

    public void SetupPlayer(PlayerData playerData)
    {
        this.playerData = playerData;
        SetupPlayerVisuals();
    }

    private void SetupPlayerVisuals()
    {
        playerVisual.SetPlayerColor(NetworkManagerUI.Instance.GetPlayerColor(playerData.colorId));
        if(NetworkManager.Singleton.LocalClientId == playerLocalId)
            usernameText.text = "Vous";
        else     
            usernameText.text = playerData.username.ToString(); 
    }

    public void IncreaseLocalScore(int nbPoints)
    {
        pointsAmount = pointsAmount + nbPoints;
        pointsAmountText.text = pointsAmount + " points";
    }

    public CardInHand GetHand()
    {
        return cardsInHand;
    }

    public ulong GetPlayerId()
    {
        return playerData.clientId;
    }
    
    public void DisableHand()
    {
        GetHand().DisableHand();
    }
}

