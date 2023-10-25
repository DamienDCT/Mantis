using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class HostDisconnectedUI : MonoBehaviour
{

    [SerializeField] private Button playAgainBtn;

    [SerializeField] private GameObject panelToDisable;

    private void Awake()
    {
        NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectedCallback;

        playAgainBtn.onClick.AddListener(() => {
            NetworkManager.Singleton.Shutdown();
            Loader.Load(Loader.Scene.InitializeScene);
        });

        Hide();
    }



    private void NetworkManager_OnClientDisconnectedCallback(ulong clientId)
    {
        if(clientId == NetworkManager.ServerClientId)
        {            
            // try{
            //     NetworkManagerUI.Instance.playerDataNetworkList.Clear();
            // } catch(NullReferenceException nfe) { }
            Show();
        }
    }

    private void OnDestroy()
    {
        if(NetworkManager.Singleton != null)
            NetworkManager.Singleton.OnClientDisconnectCallback -= NetworkManager_OnClientDisconnectedCallback;
    }

    private void Show()
    {
        panelToDisable?.SetActive(true);
    }

    private void Hide()
    {
        panelToDisable?.SetActive(false);
    }
}
