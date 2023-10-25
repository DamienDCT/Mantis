using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;
using UnityEngine.UI;

public class VisualUpdater : NetworkBehaviour
{
    [SerializeField] private TextMeshProUGUI joinCodeText;
    [SerializeField] private TextMeshProUGUI nbPointsText;
    [SerializeField] private Button copyClipboardBtn;

    private void Awake()
    {
        copyClipboardBtn.onClick.AddListener(() => {
            CopyClipboard();
        });
    }

    private void CopyClipboard()
    {
        AudioManager.Instance?.Play("UIClick");
        GUIUtility.systemCopyBuffer = NetworkManagerUI.Instance?.GetJoinCode();
    }

    private void Start()
    {
        UpdateJoinCodeTextClientServerRpc();
        UpdatePointsAmountServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void UpdateJoinCodeTextClientServerRpc()
    {
        UpdateJoinCodeTextClientClientRpc(NetworkManagerUI.Instance.GetJoinCode());
    }

    [ClientRpc]
    private void UpdateJoinCodeTextClientClientRpc(string joinCode)
    {
        joinCodeText.text = "Code de partie : " + joinCode;
    }


    [ServerRpc(RequireOwnership = false)]
    private void UpdatePointsAmountServerRpc()
    {
        UpdatePointsAmountClientRpc(NetworkManagerUI.Instance.GetNbPointsNeeded());
    }

    [ClientRpc]
    private void UpdatePointsAmountClientRpc(int nbPts)
    {
        nbPointsText.text = "Nombre de points pour gagner : " + nbPts;
    }

}
