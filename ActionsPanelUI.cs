using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class  ActionsPanelUI : NetworkBehaviour
{
    [SerializeField] private Button stealButton;
    [SerializeField] private Button scoreButton;

    [SerializeField] private GameObject buttonsPanel;

    [SerializeField] private GameObject stealPanelUI;

    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private Player currentPlayer;

    private void Awake()
    {
        stealButton.onClick.AddListener(() => {
            Steal();
        });

        scoreButton.onClick.AddListener(() => {
            Score();
        });
    }

    private void Steal(){
        stealPanelUI.SetActive(true);
        HideButtons();
    }

    private void Score()
    {
        Card card = Deck.Instance.GetLastCard();
        if(currentPlayer.GetHand().HasCardColor(card.colorCard))
        {
            // Score all of the cards
            int nbPoints = currentPlayer.GetHand().GetAmountCardByColor(card.colorCard);
            CardColor colorCard = card.colorCard;
            AnimationScoringWithCardsInHandServerRpc(colorCard, currentPlayer.GetPlayerId(), nbPoints + 1);
        } else {
            AnimationScoringWithNoCardsInHandServerRpc(card, currentPlayer.GetPlayerId());
        }
        HideButtons();
    }

    [ServerRpc(RequireOwnership = false)]
    private void AnimationScoringWithNoCardsInHandServerRpc(Card card, ulong playerId)
    {
        AnimationScoringWithNoCardsInHandClientRpc(card, playerId);
        Deck.Instance.NextCardServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void AnimationScoringWithCardsInHandServerRpc(CardColor colorCard, ulong playerId, int amountPoints)
    {
        AnimationScoringWithCardsInHandClientRpc(colorCard, playerId, amountPoints);
        Deck.Instance.NextCardServerRpc();
    }

    [ClientRpc]
    private void AnimationScoringWithNoCardsInHandClientRpc(Card card, ulong playerId)
    {
        foreach(Player player in MantisGameMultiplayer.Instance.GetListOfPlayers())
        {
            if(player.gameObject.activeSelf && player.GetPlayerId() == playerId)
            {
                if(playerId == NetworkManager.Singleton.LocalClientId)
                    InformationDisplay.Instance?.DisplayInformation("Vous avez décidé de marquer");
                else
                    InformationDisplay.Instance?.DisplayInformation(NetworkManagerUI.Instance.GetUsernameByClientId(playerId) + " a décidé de marquer");
                StartCoroutine(AnimationScoringWithNoCardsInHand(card, player, player.GetHand().GetLocalPosition()));
            }
        }
    }

    [ClientRpc]
    private void AnimationScoringWithCardsInHandClientRpc(CardColor colorCard, ulong playerId, int amountPoints)
    {
        foreach(Player player in MantisGameMultiplayer.Instance.GetListOfPlayers())
        {
            if(player.gameObject.activeSelf && player.GetPlayerId() == playerId)
            {
                if(playerId == NetworkManager.Singleton.LocalClientId)
                    InformationDisplay.Instance?.DisplayInformation("Vous avez décidé de marquer");
                else
                    InformationDisplay.Instance?.DisplayInformation(NetworkManagerUI.Instance.GetUsernameByClientId(playerId) + " a décidé de marquer");
                StartCoroutine(AnimationScoringWithCardsInHand(colorCard, player, player.GetHand().GetLocalPosition(), amountPoints));
            }
        }
    }

    /*
        Coroutine with animation scoring only if player has no cards of this color
    */
    private IEnumerator AnimationScoringWithNoCardsInHand(Card card, Player player, Vector3 handPosition)
    {
        Deck.Instance.DisplayNextCard(handPosition);
        yield return new WaitForSeconds(1.5f);
        player.GetHand().AddCardInHand(card);
        yield return new WaitForSecondsRealtime(1.5f);
        if(IsHost)
            TurnSystem.Instance.NextPlayerServerRpc();
    }

    /*
        Coroutine with animation scoring only if player has cards of this color
    */
    private IEnumerator AnimationScoringWithCardsInHand(CardColor colorCard, Player player, Vector3 handPosition, int amountPoints)
    {
        Deck.Instance.DisplayNextCard(handPosition);
        yield return new WaitForSeconds(1.5f);
        player.GetHand().RemoveCardInHandFromColor(colorCard);
        player.IncreaseLocalScore(amountPoints);
        yield return new WaitForSecondsRealtime(1.5f);
        if(IsHost)
        {
            MantisGameMultiplayer.Instance.ScoreServerRpc(player.GetPlayerId(), amountPoints);
            TurnSystem.Instance.NextPlayerServerRpc();
        }
    }

    public void StealPlayer(ulong playerId)
    {
        Card card = Deck.Instance.GetLastCard();
        Player stolenPlayer = MantisGameMultiplayer.Instance.FindPlayerWithPlayerId(playerId);
        if(stolenPlayer.GetHand().HasCardColor(card.colorCard))
        {
            int nbCardToStole = stolenPlayer.GetHand().GetAmountCardByColor(card.colorCard) + 1;
            CardColor cardColor = card.colorCard;
            AnimationStealingWithCardsInHandServerRpc(playerId, currentPlayer.GetPlayerId(), nbCardToStole, card, card.colorCard);
        } else {
            AnimationStealingWithNoCardsInHandServerRpc(playerId, currentPlayer.GetPlayerId(), card);
        }
        HideButtons();
    }

    [ServerRpc(RequireOwnership = false)]
    private void AnimationStealingWithNoCardsInHandServerRpc(ulong stolenPlayerId, ulong playerWhoStoleId, Card card)
    {
        AnimationStealingWithNoCardsInHandClientRpc(stolenPlayerId, playerWhoStoleId, card);
        Deck.Instance.NextCardServerRpc();
    }

    [ClientRpc]
    private void AnimationStealingWithNoCardsInHandClientRpc(ulong stolenPlayerId, ulong playerWhoStoleId, Card card)
    {
        if(stolenPlayerId == NetworkManager.Singleton.LocalClientId)
            InformationDisplay.Instance?.DisplayInformation(NetworkManagerUI.Instance.GetUsernameByClientId(playerWhoStoleId) + " a décidé de vous voler");
        else if(currentPlayer.GetPlayerId() == playerWhoStoleId)
            InformationDisplay.Instance?.DisplayInformation("Vous avez décidé de voler " + NetworkManagerUI.Instance.GetUsernameByClientId(stolenPlayerId));
        else 
        {
            InformationDisplay.Instance?.DisplayInformation(
                NetworkManagerUI.Instance.GetUsernameByClientId(playerWhoStoleId) + 
                " a décidé de voler " + 
                NetworkManagerUI.Instance.GetUsernameByClientId(stolenPlayerId)
            );
        }
        foreach(Player player in MantisGameMultiplayer.Instance.GetListOfPlayers())
        {
            if(player.gameObject.activeSelf && player.GetPlayerId() == stolenPlayerId)
            {
                StartCoroutine(AnimationStealingWithNoCardsInHand(player, card, player.GetHand().GetLocalPosition()));
            }
        }
    }

    // Give card to people who has been stolen
    private IEnumerator AnimationStealingWithNoCardsInHand(Player stolenPlayer, Card card, Vector3 handPosition)
    {
        Deck.Instance.DisplayNextCard(handPosition);
        yield return new WaitForSeconds(1.5f);
        stolenPlayer.GetHand().AddCardInHand(card);
        yield return new WaitForSecondsRealtime(1.5f);
        if(IsHost)
            TurnSystem.Instance.NextPlayerServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void AnimationStealingWithCardsInHandServerRpc(ulong stolenPlayerId, ulong currentPlayerId, int nbCardToStole, Card card, CardColor colorCard)
    {
        AnimationStealingWithCardsInHandClientRpc(stolenPlayerId, currentPlayerId, nbCardToStole, card, colorCard);
        Deck.Instance.NextCardServerRpc();
    }

    [ClientRpc]
    private void AnimationStealingWithCardsInHandClientRpc(ulong stolenPlayerId, ulong currentPlayerId, int nbCardToStole, Card card, CardColor colorCard)
    {
        if(stolenPlayerId == NetworkManager.Singleton.LocalClientId)
            InformationDisplay.Instance?.DisplayInformation(NetworkManagerUI.Instance.GetUsernameByClientId(currentPlayerId) + " a décidé de vous voler");
        else if(currentPlayer.GetPlayerId() == currentPlayerId)
            InformationDisplay.Instance?.DisplayInformation("Vous avez décidé de voler " + NetworkManagerUI.Instance.GetUsernameByClientId(stolenPlayerId));
        else 
        {
            InformationDisplay.Instance?.DisplayInformation(
                NetworkManagerUI.Instance.GetUsernameByClientId(currentPlayerId) + 
                " a décidé de voler " + 
                NetworkManagerUI.Instance.GetUsernameByClientId(stolenPlayerId)
            );
        }
        Player stolenPlayer = null, playerWhoStole = null;
        foreach(Player player in MantisGameMultiplayer.Instance.GetListOfPlayers())
        {
            if(player.gameObject.activeSelf)
            {
                if(player.GetPlayerId() == stolenPlayerId)
                {
                    stolenPlayer = player;
                } else if(player.GetPlayerId() == currentPlayerId)
                {
                    playerWhoStole = player;
                }

            }
        }
        StartCoroutine(AnimationStealingWithCardsInHand(stolenPlayer, playerWhoStole, nbCardToStole, card, colorCard, playerWhoStole.GetHand().GetLocalPosition()));
    }


    // Give card to people who stole
    private IEnumerator AnimationStealingWithCardsInHand(Player stolenPlayer, Player playerWhoStole, int nbCardToStole, Card card, CardColor cardColor, Vector3 handPosition)
    {
        Deck.Instance.DisplayNextCard(handPosition);
        yield return new WaitForSeconds(1.5f);
        playerWhoStole.GetHand().AddCardInHandWithAmount(card, nbCardToStole);
        stolenPlayer.GetHand().RemoveCardInHandFromColor(cardColor);
        yield return new WaitForSecondsRealtime(1.5f);
        if(IsHost)
            TurnSystem.Instance.NextPlayerServerRpc();
    }

    private void Hide()
    {
        this.buttonsPanel.SetActive(false);
    }

    private void Show()
    {
        this.buttonsPanel.SetActive(true);
    }

    public void HideButtons()
    {
        this.stealPanelUI.gameObject.SetActive(false);
        Hide();
    }

    public void ShowButtons()
    {
        this.stealPanelUI.gameObject.SetActive(true);
        Show();
    }
}
