using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using TMPro;
using Unity.Netcode.Transports.UTP;
using Unity.Collections;
using System;
using System.Collections.Generic;
using UnityEditor;

public class NetworkManagerUI : NetworkBehaviour
{

    [SerializeField] private string clientJoinCode = "";
    [SerializeField] public string usernameTxt = "";


    [SerializeField] private int[] pointsToWinList;

    private NetworkVariable<JoinCodeStruct> joinCode = new NetworkVariable<JoinCodeStruct>( new JoinCodeStruct{
        joinCode = "",
        username = ""
    }, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    private NetworkVariable<int> nbPointsToWin = new NetworkVariable<int>(10, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    public NetworkList<PlayerData> playerDataNetworkList;

    [SerializeField] private GameObject myNetworkPrefab;

    public event EventHandler OnPlayerDataNetworkListChanged;

    [SerializeField] private List<Color> playerColorList;

    public struct JoinCodeStruct : INetworkSerializable
    {
        public FixedString32Bytes joinCode;
        public FixedString32Bytes username;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref joinCode);
            serializer.SerializeValue(ref username);
        }
    }

    public static NetworkManagerUI Instance { get; private set; }

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else 
        {
            Instance.playerDataNetworkList = new NetworkList<PlayerData>(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
            Destroy(gameObject);
        }
        playerDataNetworkList = new NetworkList<PlayerData>(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
        playerDataNetworkList.OnListChanged += PlayerDataNetworkList_OnListChanged;

    }

    private void PlayerDataNetworkList_OnListChanged(NetworkListEvent<PlayerData> changeEvent)
    {
        OnPlayerDataNetworkListChanged?.Invoke(this, EventArgs.Empty);
    }

    // Start is called before the first frame update
    private async void Start()
    {
        await UnityServices.InitializeAsync();

        if(AuthenticationService.Instance.IsSignedIn)
            return;

        AuthenticationService.Instance.SignedIn += () => {
            Debug.Log("Signed in " + AuthenticationService.Instance.PlayerId);
        };

        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    public async void CreateRelay()
    {
        try{
            Debug.Log("we enter this");
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(3);

            joinCode = new NetworkVariable<JoinCodeStruct>( new JoinCodeStruct{
                joinCode = "",
                username = ""
            }, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

            playerDataNetworkList = new NetworkList<PlayerData>(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

            string joinCode2 = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            if(usernameTxt == "")
                usernameTxt = GenerateRandomUsername();
            joinCode.Value = new JoinCodeStruct {
                joinCode = joinCode2,
                username = usernameTxt
            };
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetHostRelayData(
                allocation.RelayServer.IpV4,
                (ushort)allocation.RelayServer.Port,
                allocation.AllocationIdBytes,
                allocation.Key,
                allocation.ConnectionData
            );

            NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_OnClientConnectedCallback;
            NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_Server_OnClientDisconnectedCallback;
            NetworkManager.Singleton.OnServerStopped += NetworkManager_OnServerStoppedCallback;


            NetworkManager.Singleton.StartHost();
            Loader.LoadNetwork(Loader.Scene.LobbyScene);
        } catch(RelayServiceException e)
        {
            Debug.Log(e);
        }
    }

    private void NetworkManager_OnServerStoppedCallback(bool obj)
    {
        Debug.Log("we disconnect");
        NetworkManager.Singleton.OnClientConnectedCallback -= NetworkManager_OnClientConnectedCallback;
        NetworkManager.Singleton.OnClientDisconnectCallback -= NetworkManager_Server_OnClientDisconnectedCallback;
        NetworkManager.Singleton.OnServerStopped -= NetworkManager_OnServerStoppedCallback;

        try{
            playerDataNetworkList = new NetworkList<PlayerData>(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
        } catch(NullReferenceException nfe) { }
    }


    private void NetworkManager_Server_OnClientDisconnectedCallback(ulong clientId)
    {
        for(int i = 0; i < playerDataNetworkList.Count; i++)
        {
            PlayerData playerData = playerDataNetworkList[i];
            if(playerData.clientId == clientId)
            {
                playerDataNetworkList.RemoveAt(i);
            }
        }
    }

    private void NetworkManager_OnClientConnectedCallback(ulong clientId)
    {   
        Debug.Log("LocalClientId = " + NetworkManager.Singleton.LocalClientId);
        PlayerData playerData = new PlayerData{
            clientId = clientId,
            colorId = GetFirstUnusedColorId(),
            username = ""
        };
        playerDataNetworkList.Add(playerData);
    }

    [ServerRpc(RequireOwnership = false)]
    public void UpdateNicknameServerRpc(ulong clientId, string text)
    {
        PlayerData playerData = GetPlayerDataFromClientId(clientId);
        int playerDataIndex = GetPlayerDataIndexFromClientId(clientId);
        playerDataNetworkList[playerDataIndex] = new PlayerData {
            clientId = clientId,
            colorId = playerData.colorId,
            username = text
        }; 

     //  Debug.Log("playerData.username = " + playerData.username);
        OnPlayerDataNetworkListChanged?.Invoke(this, EventArgs.Empty);
    }   

    public async void JoinRelay()
    {
        if(clientJoinCode == "") return;
        if(usernameTxt == "")
        {
            usernameTxt = GenerateRandomUsername();
        }

        try{
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(clientJoinCode);


            NetworkManager.Singleton.GetComponent<UnityTransport>().SetClientRelayData(
                joinAllocation.RelayServer.IpV4,
                (ushort)joinAllocation.RelayServer.Port,
                joinAllocation.AllocationIdBytes,
                joinAllocation.Key,
                joinAllocation.ConnectionData,
                joinAllocation.HostConnectionData
            );
            Debug.Log("we join a relay");


            NetworkManager.Singleton.StartClient();
        } catch(RelayServiceException e)
        {
            Debug.LogError(e);
        }
    }


    public void ResetNickname()
    {
        usernameTxt = "";
    }

    private string GenerateRandomUsername()
    {
        return "Joueur" + UnityEngine.Random.Range(0, 1000).ToString();
    }



    // public void CopyPasteCode()
    // {
    //     string copyPaste = GUIUtility.systemCopyBuffer;
    //     clientJoinCode = copyPaste;
    //     joinCodeTextField.text = copyPaste;
    // }



    public string GetJoinCode()
    {
        return joinCode.Value.joinCode.ToString();
    }

    public bool IsPlayerIndexConnected(int playerIndex)
    {
        return playerIndex < playerDataNetworkList.Count;
    }

    public int GetPlayerDataIndexFromClientId(ulong clientId)
    {
        for(int i = 0; i < playerDataNetworkList.Count; i++)
        {
            if(playerDataNetworkList[i].clientId == clientId)
            {
                return i;
            }
        }
        return -1;
    }

    public PlayerData GetPlayerDataFromClientId(ulong clientId)
    {
        foreach(PlayerData playerData in playerDataNetworkList)
        {
            if(playerData.clientId == clientId)
            {
                return playerData;
            }
        }
        return default;
    }

    public PlayerData GetPlayerData()
    {
        return GetPlayerDataFromClientId(NetworkManager.Singleton.LocalClientId);
    }

    public PlayerData GetPlayerDataFromPlayerIndex(int playerIndex)
    {
        return playerDataNetworkList[playerIndex];
    }

    public Color GetPlayerColor(int colorId)
    {
        return playerColorList[colorId];
    }

    public void ChangePlayerColor(int colorId)
    {
        ChangePlayerColorServerRpc(colorId);
    }

    [ServerRpc(RequireOwnership = false)]
    private void ChangePlayerColorServerRpc(int colorId, ServerRpcParams serverRpcParams = default)
    {
        if(!IsColorAvailable(colorId))
        {
            return;
        }

        int playerDataIndex = GetPlayerDataIndexFromClientId(serverRpcParams.Receive.SenderClientId);
        PlayerData playerData = playerDataNetworkList[playerDataIndex];
        playerData.colorId = colorId;
        playerDataNetworkList[playerDataIndex] = playerData;
        OnPlayerDataNetworkListChanged?.Invoke(this, EventArgs.Empty);
    }

    private bool IsColorAvailable(int colorId)
    {
        foreach(PlayerData playerData in playerDataNetworkList)
        {
            if(playerData.colorId == colorId)
            {
                return false;
            }
        }

        return true;
    }

    private int GetFirstUnusedColorId()
    {
        for(int i = 0; i < playerColorList.Count; i++)
        {
            if(IsColorAvailable(i))
                return i;
        }
        return -1;
    }

    public NetworkList<PlayerData> GetNetworkListPlayerDatas()
    {
        return this.playerDataNetworkList;
    }

    public string GetUsernameByClientId(ulong clientId)
    {
        foreach(PlayerData playerData in playerDataNetworkList)
        {
            if(playerData.clientId == clientId)
            {
                return playerData.username.ToString();
            }
        }
        return "";
    }

    public void OnDropdownValueChanged(int value)
    {
        nbPointsToWin.Value = pointsToWinList[value];
    }

    public int GetNbPointsNeeded()
    {
        return nbPointsToWin.Value;
    }

    public void SetClientJoinCode(string code)
    {
        clientJoinCode = code;
    }

    public void SetUsername(string username)
    {
        usernameTxt = username;
    }
}
