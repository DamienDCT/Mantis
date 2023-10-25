using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;

public class PauseGameUI : MonoBehaviour
{
    [SerializeField] private Button unpauseBtn;
    [SerializeField] private Button goBackMenuBtn;

    [SerializeField] private GameObject pauseMenuPanel;
    [SerializeField] private Button[] buttonsToDisable;

    private bool isGamePausedLocally;

    private void Awake()
    {
        isGamePausedLocally = false;

        unpauseBtn.onClick.AddListener(() => {
            Unpause();
        });

        goBackMenuBtn.onClick.AddListener(() => {
            NetworkManager.Singleton.Shutdown();
            Loader.Load(Loader.Scene.InitializeScene);
        });
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            isGamePausedLocally = !isGamePausedLocally;
            SwapMenu();
        }
    }

    private void SwapMenu()
    {
        if(isGamePausedLocally)
        {
            Pause();
        } else 
        {
            Unpause();
        }
    }

    private void Pause()
    {
        pauseMenuPanel.gameObject.SetActive(true);
        isGamePausedLocally = true;
        DisableButtons();
    }

    private void Unpause()
    {
        pauseMenuPanel.gameObject.SetActive(false);
        isGamePausedLocally = false;
        EnableButtons();
    }

    private void DisableButtons()
    {
        foreach(Button button in buttonsToDisable)
        {
            button.enabled = false;
        }
    }

    private void EnableButtons()
    {
        foreach(Button button in buttonsToDisable)
        {
            button.enabled = true;
        }
    }
}
