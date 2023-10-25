using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CharacterSelectPlayer : MonoBehaviour
{
    [SerializeField] private int playerIndex;
    [SerializeField] private GameObject readyGameObject;
    [SerializeField] private PlayerVisual playerVisual;
    [SerializeField] private TextMeshPro usernameText;

    private void Start()
    {
        NetworkManagerUI.Instance.OnPlayerDataNetworkListChanged += NetworkManagerUI_OnPlayerDataNetworkListChanged;
        CharacterSelectReady.Instance.OnReadyChanged += CharacterSelectReady_OnReadyChanged;
        UpdatePlayer();
    }


    private void CharacterSelectReady_OnReadyChanged(object sender, EventArgs e)
    {
        UpdatePlayer();
    }
    
    private void NetworkManagerUI_OnPlayerDataNetworkListChanged(object sender, System.EventArgs e)
    {
        UpdatePlayer();
    }

    private void UpdatePlayer()
    {
        if(NetworkManagerUI.Instance.IsPlayerIndexConnected(playerIndex))
        {
            Show();

            PlayerData playerData = NetworkManagerUI.Instance.GetPlayerDataFromPlayerIndex(playerIndex);
            readyGameObject.SetActive(CharacterSelectReady.Instance.IsPlayerReady(playerData.clientId));
            usernameText.text = playerData.username.ToString();
            playerVisual.SetPlayerColor(NetworkManagerUI.Instance.GetPlayerColor(playerData.colorId));
        } else {
            Hide();
        }
    }

    private void OnDestroy()
    {
        NetworkManagerUI.Instance.OnPlayerDataNetworkListChanged -= NetworkManagerUI_OnPlayerDataNetworkListChanged;
        CharacterSelectReady.Instance.OnReadyChanged -= CharacterSelectReady_OnReadyChanged;
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
