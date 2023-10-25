using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterColorSelectSingleUI : MonoBehaviour
{
    [SerializeField] private int colorId;
    [SerializeField] private Image image;
    [SerializeField] private GameObject selectedImage;

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(() => {
            NetworkManagerUI.Instance.ChangePlayerColor(colorId);
        });
    }

    private void Start()
    {
        NetworkManagerUI.Instance.OnPlayerDataNetworkListChanged += NetworkManagerUI_OnPlayerDataNetworkListChanged;
        image.color = NetworkManagerUI.Instance.GetPlayerColor(colorId);
        UpdateIsSelected();
    }

    private void NetworkManagerUI_OnPlayerDataNetworkListChanged(object sender, System.EventArgs e)
    {
        UpdateIsSelected();
    }

    private void OnDestroy()
    {
        NetworkManagerUI.Instance.OnPlayerDataNetworkListChanged -= NetworkManagerUI_OnPlayerDataNetworkListChanged;
    }

    private void UpdateIsSelected()
    {
        if(NetworkManagerUI.Instance.GetPlayerData().colorId == colorId)
        {
            selectedImage.SetActive(true);
        } else {
            selectedImage.SetActive(false);
        }
    }
}
