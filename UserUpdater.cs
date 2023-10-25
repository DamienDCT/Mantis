
using UnityEngine;
using Unity.Netcode;

public class UserUpdater : NetworkBehaviour
{
    private void Start()
    {
        Debug.Log("coucou " + NetworkManagerUI.Instance.usernameTxt);
        NetworkManagerUI.Instance.UpdateNicknameServerRpc(NetworkManager.Singleton.LocalClientId, NetworkManagerUI.Instance.usernameTxt);
    }
}
