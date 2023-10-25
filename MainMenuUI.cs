using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainMenuUI : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button quitBtn;
    [SerializeField] private Button backMenuBtn;
    [SerializeField] private Button joinGameBtn;
    [SerializeField] private Button createMenuGameBtn;

    [Header("Relay Buttons")]
    [SerializeField] private Button hostGameBtn;
    [SerializeField] private Button connexionJoinGameBtn;

    [Header("Panels")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject joinMenuPanel;
    [SerializeField] private GameObject createGameMenuPanel;


    private void Awake()
    {
        hostGameBtn.onClick.AddListener(() => {
            PlayUISound();
            NetworkManagerUI.Instance?.CreateRelay();
        });

        connexionJoinGameBtn.onClick.AddListener(() => {
            PlayUISound();
            NetworkManagerUI.Instance?.JoinRelay();
        });

        quitBtn.onClick.AddListener(() => {
            PlayUISound();
            Application.Quit();
        });

        createMenuGameBtn.onClick.AddListener(() => {
            PlayUISound();
            GoToCreateMenu();
        });

        backMenuBtn.onClick.AddListener(() => {
            PlayUISound();
            GoToMainMenu();
        });

        joinGameBtn.onClick.AddListener(() => {
            PlayUISound();
            GoToJoinMenu();
        });
    }

    public void PlayUISound()
    {
        AudioManager.Instance.Play("UIClick");
    }

    public void ChangeClientCode(string textFieldCode)
    {
        NetworkManagerUI.Instance.SetClientJoinCode(textFieldCode);
    }

    public void ChangeUsernameTxt(string username)
    {
        NetworkManagerUI.Instance?.SetUsername(username);
    }
    
    public void GoToMainMenu()
    {
        mainMenuPanel.SetActive(true);
        joinMenuPanel.SetActive(false);
        createGameMenuPanel.SetActive(false);
        NetworkManagerUI.Instance?.ResetNickname();
    }


    public void GoToCreateMenu()
    {
        mainMenuPanel.SetActive(false);
        createGameMenuPanel.SetActive(true);
        joinMenuPanel.SetActive(false);
        NetworkManagerUI.Instance?.ResetNickname();
    }

    private void GoToJoinMenu()
    {
        mainMenuPanel.SetActive(false);
        joinMenuPanel.SetActive(true);
        createGameMenuPanel.SetActive(false);
        NetworkManagerUI.Instance?.ResetNickname();
    }

    public void UpdateJoinCodeText(string code)
    {
        NetworkManagerUI.Instance?.SetClientJoinCode(code);
    }

}
