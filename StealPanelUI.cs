using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using System;

public class StealPanelUI : NetworkBehaviour
{

    [SerializeField] private Button[] buttons;

    [SerializeField] private ActionsPanelUI actionsPanelUI;

    private Dictionary<ulong, Button> dictButtonById;

    private void Start()
    {
        dictButtonById = new Dictionary<ulong, Button>();
        MantisGameMultiplayer.Instance.ActionsPanel_OnPlayerDisconnect += PlayerDisconnect;
        Invoke("UpdateButtonsBelongPlayerAmount", 0.05f);
    }



    public void UpdateButtonsBelongPlayerAmount()
    {
      //  NetworkList<PlayerData> list = NetworkManagerUI.Instance.GetNetworkListPlayerDatas();
        List<Player> listPlayers = MantisGameMultiplayer.Instance.GetListOfOtherPlayers();
        int nbPlayers = listPlayers.Count;
        Debug.Log("we setup buttons + count = " + nbPlayers);
        foreach(Button button in buttons)
        {
            button.gameObject.SetActive(false);
        }
        for(int i = 0; i < nbPlayers; i++)
        {
            buttons[i].gameObject.SetActive(true);
            ulong playerId = listPlayers[i].GetPlayerId();

            dictButtonById[playerId] = buttons[i];
            buttons[i].onClick.AddListener(() => {
                actionsPanelUI.StealPlayer(playerId);
                Hide();
            });
        }
        Hide();
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void PlayerDisconnect(object sender, MantisGameMultiplayer.PlayerDisconnectedArgs args)
    {
        dictButtonById[args.playerId].gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        MantisGameMultiplayer.Instance.ActionsPanel_OnPlayerDisconnect -= PlayerDisconnect;
    }

}
