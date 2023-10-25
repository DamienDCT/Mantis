using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;
using TMPro;
using System.Collections;
using System;

public class Deck : NetworkBehaviour
{
    private int nbCardInDeck;

    public NetworkList<Card> cardList;
    [SerializeField] public TextMeshProUGUI text;

    [SerializeField] private Transform cardVisual;

    [SerializeField] private NetworkVariable<int> indexCardFromDeck = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    private int index = 0;

    public static Deck Instance;

    private bool isGeneratedForHost;

    
    // EVENTS
    public event EventHandler Deck_OnCardsGenerated;
    public event EventHandler Deck_OnCardsDistributed;

    private void Awake()
    {
        cardList = new NetworkList<Card>();
        Instance = this;
        isGeneratedForHost = false;
    }

    private void Start()
    {
        nbCardInDeck = 140;
        InitializeDeckServerRpc();
        indexCardFromDeck.OnValueChanged += Deck_IndexCardFromDeckOnValueChanged;
    }
    

    public void DisplayNextCard(Vector3 positionPlayer)
    {
        CardHolderAnimation.Instance.ExitCard(GetLastCard(), positionPlayer);
    }

    private void Deck_IndexCardFromDeckOnValueChanged(int previousValue, int newValue)
    {
        if(!IsOwner)
            return;
        DisplayLastCardClientRpc(newValue);
    }


    [ServerRpc(RequireOwnership = false)]
    private void InitializeDeckServerRpc()
    {
        if(IsHost)
        {
            /*
                Distribution : 
                [0] = Orange
                [1] = Yellow
                [2] = Green
                [3] = Pink
                [4] = Blue
                [5] = Purple

            */
            int[] distributionCard = new int[] {20, 20, 20, 20, 20, 20, 20};
            
            int nbCardGenerated = 0;
            while(nbCardGenerated < nbCardInDeck)
            {
                int randomColorId = UnityEngine.Random.Range(0,7);
                if(distributionCard[randomColorId] == 0)
                    continue;

                distributionCard[randomColorId]--;
                CardColor randomCardColor = CardDictionary.Instance.GetCardColorFromColorId(randomColorId);
                Card card = new Card{
                    colorCard = randomCardColor,
                    cardId = nbCardGenerated
                };

                card.GenerateOtherColors();
                nbCardGenerated++;
                cardList.Add(card);
            }
            DisplayLastCardClientRpc();
            DisplayAmountCardOnDeckClientRpc(indexCardFromDeck.Value);
            if(isGeneratedForHost || NetworkManagerUI.Instance.GetNetworkListPlayerDatas().Count == 1)
            {
                Debug.Log("we enter the function InitializeDeck " + NetworkManager.Singleton.LocalClientId);
                Deck_OnCardsGenerated?.Invoke(this, EventArgs.Empty);
            } else {
                index++;
                if(index == (NetworkManagerUI.Instance.GetNetworkListPlayerDatas().Count - 1))
                {
                    isGeneratedForHost = true;  
                }
            }
        }
    }

    [ClientRpc]
    private void DisplayLastCardClientRpc(int value = -1)
    {
        if(cardList.Count > 0)
        {
            if(value == -1)
                cardVisual.GetComponent<CardVisual>().UpdateGraphics(cardList[indexCardFromDeck.Value]);
            else {
                cardVisual.GetComponent<CardVisual>().UpdateGraphics(cardList[value]);
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void NextCardServerRpc()
    {
        if(IsHost)
        {
            indexCardFromDeck.Value = indexCardFromDeck.Value + 1;
            DisplayAmountCardOnDeckClientRpc(indexCardFromDeck.Value);
        }
    }

    [ClientRpc]
    public void DisplayAmountCardOnDeckClientRpc(int nbCard)
    {
        InformationDisplay.Instance?.DisplayInformationDeck(nbCardInDeck - nbCard);
    }

    public void DistributeCards()
    {
        DistributeCardsServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void DistributeCardsServerRpc()
    {

        if(!IsHost)
            return;
        if(MantisGameMultiplayer.Instance.GetListOfPlayers() == null)
            return;

        for(int i = 0; i < 4; i++)
        {
            foreach(Player player in MantisGameMultiplayer.Instance.GetListOfPlayers())
            {
                if(player.gameObject.activeSelf)
                {
                    DistributeCardsOnLocalClientRpc(GetLastCard(), player.GetPlayerId());
                    NextCardServerRpc();
                }
            }
        }
        Deck_OnCardsDistributed?.Invoke(this, EventArgs.Empty);
    }

    [ClientRpc] 
    private void DistributeCardsOnLocalClientRpc(Card card, ulong playerId)
    {
        foreach(Player _player in MantisGameMultiplayer.Instance.GetListOfPlayers())
        {
            if(_player.gameObject.activeSelf)
            {
                if(_player.GetPlayerId() == playerId)
                {
                    _player.GetHand().AddCardInHand(card);
                }
            }
        }
    }

    public Card GetLastCard()
    {
        return cardList[indexCardFromDeck.Value];
    }
}
