using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CardInHand : MonoBehaviour
{
    //public NetworkList<Card> cardLists;

    [SerializeField] private List<Card> cardLocalList;

    [SerializeField] private HandCardUI handCardUI;

    private void Awake()
    {
      //  cardLists = new NetworkList<Card>(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        cardLocalList = new List<Card>();
      //  cardLists.OnListChanged += UpdateColor;
    }


    public void UpdateColor()
    {
        Debug.Log("On veut modifier la liste");
        handCardUI?.UpdateUIColor(cardLocalList);
        // if(gameObject.activeSelf) 
        //     StartCoroutine(UpdateWithTimer());
    }

    private IEnumerator UpdateWithTimer()
    {
        yield return new WaitForSecondsRealtime(0.2f);
        handCardUI?.UpdateUIColor(cardLocalList);
    }

    public void AddCardInHand(Card card)
    {
        cardLocalList.Add(card);
        UpdateColor();
    }

    public void RemoveCardInHandFromColor(CardColor cardColor)
    {
        int nbCards = GetAmountCardByColor(cardColor);
        for(int i = 0; i < nbCards; i++)
        {
            foreach(Card card in cardLocalList)
            {
                if(card.colorCard == cardColor)
                {
                    cardLocalList.Remove(card);
                    break;
                }
            }
        }  
        UpdateColor();
    }

    public void AddCardInHandWithAmount(Card card, int amount)
    {
        for(int i = 0; i < amount; i++)
        {
            AddCardInHand(card);
        }
    }

    public Vector3 GetLocalPosition()
    {
        return this.handCardUI.GetLocalPosition().position;
    }

    public bool HasCardColor(CardColor cardColor)
    {
        foreach(Card card in cardLocalList)
        {
            if(card.colorCard == cardColor)
            {
                return true;
            }
        }
        return false;
    }

    public int GetAmountCardByColor(CardColor cardColor)
    {
        int amount = 0;
        foreach(Card card in cardLocalList)
        {
            if(card.colorCard == cardColor)
            {
                amount++;
            }
        }
        return amount;
    }

    public void SetupHandCardUI(HandCardUI _handCardUI){
        this.handCardUI = _handCardUI;
    }

    public void DisableHand()
    {
        handCardUI.gameObject.SetActive(false);
    }
}
