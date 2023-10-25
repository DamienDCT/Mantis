using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;

public class TurnSystem : NetworkBehaviour
{
    public static TurnSystem Instance;

    [SerializeField] private NetworkVariable<ulong> currentPlayerPlaying;
    private NetworkVariable<int> indexNetworkListCurrentPlayer;

   [SerializeField] private GameObject actionsPanel;

    private void Awake()
    {
        Instance = this;
        currentPlayerPlaying = new NetworkVariable<ulong>(200000, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        indexNetworkListCurrentPlayer = new NetworkVariable<int>(20000, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        currentPlayerPlaying.OnValueChanged += ShowActions;
        MantisGameMultiplayer.Instance.OnPlayerDisconnect += PlayerDisconnect;
    }

    private void PlayerDisconnect(object sender, MantisGameMultiplayer.PlayerDisconnectedArgs args)
    {
        if(currentPlayerPlaying.Value != args.playerId)
            return;
        else
            NextPlayerServerRpc();
    }

    private void ShowActions(ulong previousValue, ulong newValue)
    {
        ShowActionsPlayer(newValue);
    }

    public void RandomFirstPlayerPlaying()
    {
        if(IsHost)
        {
            int indexPlayerRandom = Random.Range(0, NetworkManagerUI.Instance.GetNetworkListPlayerDatas().Count);
            indexNetworkListCurrentPlayer.Value = indexPlayerRandom;
            currentPlayerPlaying.Value = NetworkManagerUI.Instance.GetNetworkListPlayerDatas()[indexPlayerRandom].clientId;
            UpdateNextPlayerTurnClientRpc(indexNetworkListCurrentPlayer.Value);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void NextPlayerServerRpc()
    {
        indexNetworkListCurrentPlayer.Value = (indexNetworkListCurrentPlayer.Value + 1) % NetworkManagerUI.Instance.GetNetworkListPlayerDatas().Count;
        currentPlayerPlaying.Value = NetworkManagerUI.Instance.GetNetworkListPlayerDatas()[indexNetworkListCurrentPlayer.Value].clientId;
        UpdateNextPlayerTurnClientRpc(indexNetworkListCurrentPlayer.Value);
    }

    [ClientRpc]
    public void UpdateNextPlayerTurnClientRpc(int index)
    {
        PlayerData playerData = NetworkManagerUI.Instance.GetNetworkListPlayerDatas()[index];
        ulong clientId = playerData.clientId;
        if(clientId == NetworkManager.Singleton.LocalClientId)
        {
            InformationDisplay.Instance?.DisplayInformation("A vous de jouer !");
        } else {
            InformationDisplay.Instance?.DisplayInformation("Au tour de " + playerData.username);
        }
    }

    private void ShowActionsPlayer(ulong valueId)
    {
        if(NetworkManager.Singleton.LocalClientId == valueId)
        {
            actionsPanel.GetComponent<ActionsPanelUI>().ShowButtons();
        } else {
            actionsPanel.GetComponent<ActionsPanelUI>().HideButtons();
        }
    }

    public void StopPlay()
    {
        actionsPanel.GetComponent<ActionsPanelUI>().HideButtons();
    }
}
