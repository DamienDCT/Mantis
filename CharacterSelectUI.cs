using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class CharacterSelectUI : MonoBehaviour
{
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button readyButton;

    private void Awake()
    {
        mainMenuButton.onClick.AddListener(() => {
            LeaveGame();
        });

        readyButton.onClick.AddListener(() => {
           GetReady();
        });
    }

    private void LeaveGame()
    {
        AudioManager.Instance?.Play("UIClick");
        NetworkManager.Singleton.Shutdown();
        Loader.Load(Loader.Scene.InitializeScene);
    }

    private void GetReady()
    {
        AudioManager.Instance?.Play("UIClick");
        CharacterSelectReady.Instance.SetPlayerReady();
    }
}
