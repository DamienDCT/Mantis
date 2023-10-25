using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class MantisGameManager : MonoBehaviour
{
    [SerializeField] private List<Transform> spawnpointList;
    [SerializeField] private GameObject playerPrefab;

    public static MantisGameManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        //SpawnPlayersServerRpc();
    }

    // [ServerRpc]
    // private void SpawnPlayersServerRpc()
    // {
    //     if(!IsHost)
    //         return;
    //     int currentSpawnpointIndex = 0;

    //     foreach(PlayerData playerData in NetworkManagerUI.Instance.GetNetworkListPlayerDatas())
    //     {
    //         GameObject playerSpawned = Instantiate(playerPrefab);

    //         playerSpawned.transform.position = spawnpointList[currentSpawnpointIndex++].position;
    //         Color color = NetworkManagerUI.Instance.GetPlayerColor(playerData.colorId);
    //         playerSpawned.GetComponent<Player>().ApplyColor(color);
    //     }
    
    // }
}
