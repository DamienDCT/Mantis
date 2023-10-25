using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class GameEndedUI : MonoBehaviour
{
    [SerializeField] private Button playAgainBtn;

    [SerializeField] private GameObject panelUI;
    [SerializeField] private TextMeshProUGUI winnerUsernameTxt;
    
    private void Awake()
    {
        playAgainBtn.onClick.AddListener(() => {
            PlayAgain();
        });
    }

    private void Start()
    {
        MantisGameMultiplayer.Instance.OnGameEnded += MantisGameMultiplayer_OnGameEnded;
    }

    private void PlayAgain()
    {
        AudioManager.Instance?.Play("UIClick");
        NetworkManager.Singleton.Shutdown();
        Loader.Load(Loader.Scene.InitializeScene);
    }

    private void MantisGameMultiplayer_OnGameEnded(object sender, MantisGameMultiplayer.PlayerWinnerArgs args)
    {
        if(args.winnerUsername == "")
        {
            winnerUsernameTxt.text = "Fin de la partie : manque de joueurs.";
        } else {
            if(args.winnerPlayerId == NetworkManager.Singleton.LocalClientId)
            {
                winnerUsernameTxt.text = "Vous avez gagné la partie !";
                AudioManager.Instance?.Play("winGame");
            }
            else
                winnerUsernameTxt.text = args.winnerUsername + " a gagné la partie !";
        }
        Show();
    }

    private void OnDestroy()
    {
        MantisGameMultiplayer.Instance.OnGameEnded -= MantisGameMultiplayer_OnGameEnded;
    }
    
    private void Show()
    {
        panelUI.SetActive(true);
    }

    private void Hide()
    {
        panelUI.SetActive(false);
    }
}
