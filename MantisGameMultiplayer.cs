using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;
using System;
using System.Linq;
using System.Collections;
using UnityEngine.AI;

public class MantisGameMultiplayer : NetworkBehaviour
{
    public static MantisGameMultiplayer Instance;
    [SerializeField] private GameObject deckPrefab;
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private List<Transform> spawnpointList;
    [SerializeField] private List<Player> players;
    [SerializeField] private List<Transform> handsDeckTransform;

    [SerializeField] private Transform bottomSpawnpoint;
    [SerializeField] private Transform bottomHandDeckTransform;

    [SerializeField] private List<Player> otherPlayers;
    [SerializeField] private Dictionary<ulong, int> scorePlayers;

    [SerializeField] private Transform spawnDeckTransform;

    private int nbPlayersReady = 0;


    public event EventHandler<PlayerDisconnectedArgs> OnPlayerDisconnect;
    public event EventHandler<PlayerDisconnectedArgs> ActionsPanel_OnPlayerDisconnect;
    public event EventHandler<PlayerWinnerArgs> OnGameEnded;

    public TextMeshProUGUI textDisplay;
    
    private void Awake()
    {
        Instance = this;
        otherPlayers = new List<Player>();
        scorePlayers = new Dictionary<ulong, int>();

        NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectedCallback;
    }

    private void NetworkManager_OnClientDisconnectedCallback(ulong playerId)
    {
        UpdatePlayersServerRpc(playerId);
    }

    [ServerRpc(RequireOwnership = false)]
    private void UpdatePlayersServerRpc(ulong playerId)
    {
        if(playerId != NetworkManager.ServerClientId)
        {
            OnPlayerDisconnect?.Invoke(this, new PlayerDisconnectedArgs{
                playerId = playerId
            });
            RemovePlayer(playerId);
            UpdateGameVisualClientRpc(playerId);
            if(otherPlayers.Count == 0)
            {
                OnGameEnded?.Invoke(this, new PlayerWinnerArgs{
                    winnerPlayerId = playerId,
                    winnerUsername = NetworkManagerUI.Instance.GetUsernameByClientId(OwnerClientId)
                });
            }
        }
    }

    [ClientRpc]
    private void UpdateGameVisualClientRpc(ulong playerId)
    {
        RemovePlayer(playerId);
    }

    private void Start()
    {
        if(IsHost)
        {
            SpawnPlayersServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void NewPlayerReadyServerRpc()
    {
        nbPlayersReady++;
        if(nbPlayersReady >= NetworkManagerUI.Instance.GetNetworkListPlayerDatas().Count)
        {
            SpawnDeckServerRpc();
            Deck.Instance.Deck_OnCardsGenerated += OnDeckGenerated;
            Deck.Instance.Deck_OnCardsDistributed += OnCardsDistributed;
        }
    }
    private void OnCardsDistributed(object sender, EventArgs e)
    {
        UpdateCardsClientRpc();
        TurnSystem.Instance.RandomFirstPlayerPlaying();
    }

    [ServerRpc]
    public void UpdateCardsServerRpc()
    {
        UpdateCardsClientRpc();
    }

    [ClientRpc]
    private void UpdateCardsClientRpc()
    {
        Debug.Log("we update the hand of players");
        foreach(Player player in MantisGameMultiplayer.Instance.GetListOfPlayers())
        {
            if(player.gameObject.activeSelf)
            {
                player.GetHand().UpdateColor();
            }
        }
    }

    private void OnDeckGenerated(object sender, EventArgs e)
    {
        DistributeCards();
    }

    public Player FindPlayerWithPlayerId(ulong playerId)
    {
        return otherPlayers.Where(p => p.GetPlayerId() == playerId).FirstOrDefault();
    }
       
    [ServerRpc(RequireOwnership = false)]
    private void SpawnPlayersServerRpc()
    {
        foreach(PlayerData playerData in NetworkManagerUI.Instance.GetNetworkListPlayerDatas())
        {
            scorePlayers[playerData.clientId] = 0;
        }
        SpawnPlayersClientRpc();   
    }

    [ServerRpc(RequireOwnership = false)]
    public void ScoreServerRpc(ulong playerId, int score)
    {
        scorePlayers[playerId] = scorePlayers[playerId] + score;
        DisplayInformationScoreClientRpc(playerId, score);
        CheckWinners(playerId);
    }

    [ClientRpc]
    private void DisplayInformationScoreClientRpc(ulong playerId, int score)
    {
        if(playerId == NetworkManager.Singleton.LocalClientId)
        {
            InformationDisplay.Instance?.DisplayInformation("Vous avez marqué " + score + " points");
            AudioManager.Instance?.Play("earnPoints");
        }
        else
            InformationDisplay.Instance?.DisplayInformation(NetworkManagerUI.Instance.GetUsernameByClientId(playerId) + " a marqué " + score + " points");
    }

    [ClientRpc]
    private void SpawnPlayersClientRpc()
    {
        foreach(Player player in players)
        {
            Debug.Log("On desactive " + player.gameObject.name);
            player.gameObject.SetActive(false);
        }
        List<PlayerData> listeJoueurTmp = new List<PlayerData>();
        NetworkList<PlayerData> list = NetworkManagerUI.Instance.GetNetworkListPlayerDatas();
        foreach(PlayerData playerData in list)
        {
            if(playerData.clientId != NetworkManager.Singleton.LocalClientId)
            {
                listeJoueurTmp.Add(playerData);
                Debug.Log("on ajoute " + playerData.clientId + " à la liste");
            } else
            {
                players[0].gameObject.SetActive(true);
                players[0].SetLocalClientId(NetworkManager.Singleton.LocalClientId);
                players[0].SetupPlayer(playerData);
            }
        }

        Debug.Log("taille de la liste = " + listeJoueurTmp.Count);
        for(int i = 0; i < listeJoueurTmp.Count; i++)
        {
            otherPlayers.Add(players[i+1]);
            Debug.Log("we active " + players[i+1].gameObject.name);
            players[i+1].gameObject.SetActive(true);
            players[i+1].SetLocalClientId(listeJoueurTmp[i].clientId);
            players[i+1].SetupPlayer(listeJoueurTmp[i]);
        }
        NewPlayerReadyServerRpc();
    }


    [ServerRpc(RequireOwnership = false)]
    private void SpawnDeckServerRpc()
    {
        if(!IsOwner)
            return;
        Transform deckTransform = Instantiate(deckPrefab).transform;
        deckTransform.GetComponent<Deck>().text = textDisplay;
        NetworkObject networkObject = deckTransform.GetComponent<NetworkObject>();
        deckTransform.position = spawnDeckTransform.position;
        networkObject.Spawn(true);
    }


    public List<Player> GetListOfPlayers(){
        return players;
    }

    private void DistributeCards()
    {
        Deck.Instance.DistributeCards();
    }

    public List<Transform> GetListOfHands(){
        return this.handsDeckTransform;
    }

    private void CheckWinners(ulong playerId)
    {
        if(scorePlayers[playerId] >= NetworkManagerUI.Instance.GetNbPointsNeeded())
        {
            TurnSystem.Instance.StopPlay();
            PrintVictoryClientRpc(playerId);
        }
    }

    public List<Player> GetListOfOtherPlayers()
    {
        return this.otherPlayers;
    }

    [ClientRpc]
    private void PrintVictoryClientRpc(ulong playerId)
    {
        TurnSystem.Instance.StopPlay();
        StartCoroutine(DisplayVictoryText(playerId));
    }

    private IEnumerator DisplayVictoryText(ulong playerId)
    {
        yield return new WaitForSecondsRealtime(1.5f);
        OnGameEnded?.Invoke(this, new PlayerWinnerArgs{
            winnerPlayerId = playerId,
            winnerUsername = NetworkManagerUI.Instance.GetUsernameByClientId(playerId)
        });
    }

    private void RemovePlayer(ulong playerId)
    {
        foreach(Player player in otherPlayers)
        {
            if(player.GetPlayerId() == playerId)
            {
                otherPlayers.Remove(player);
                ActionsPanel_OnPlayerDisconnect?.Invoke(this, new PlayerDisconnectedArgs{
                    playerId = playerId
                });
                player.DisableHand();
                player.gameObject.SetActive(false);
                break;
            }
        }
    }
    
    public class PlayerDisconnectedArgs : EventArgs
    {
        public ulong playerId;
    }

    public class PlayerWinnerArgs : EventArgs
    {
        public ulong winnerPlayerId;
        public string winnerUsername;
    }
}

