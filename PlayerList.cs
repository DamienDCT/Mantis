using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerList : NetworkBehaviour
{
    public static PlayerList Instance {get; private set; }

    private const int MAX_PLAYER_AMOUNT = 4;

 //   private NetworkList<PlayerData> playerDataNetworkList;

    private void Awake()
    {
        Instance = this;

        DontDestroyOnLoad(this);
      //  playerDataNetworkList = new NetworkList<PlayerData>();
    }

}
