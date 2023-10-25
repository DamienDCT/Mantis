using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;

public class HandCardUI : NetworkBehaviour
{
    [SerializeField] private GameObject[] childrenObjects;
    [SerializeField] private Transform positionInGame;

    public void UpdateUIColor(List<Card> cardLists)
    {
        int[] colorNumber = new int[]{0, 0, 0, 0, 0, 0, 0};
        for(int i = 0; i < cardLists.Count; i++)
        {
            int index = CardDictionary.Instance.GetColorIdFromCardColor(cardLists[i].colorCard);
            colorNumber[index]++;
        }

        UpdateUIColorVisual(colorNumber);
    }

    private void UpdateUIColorVisual(int[] colorNumber)
    {
        for(int i = 0; i < childrenObjects.Length; i++)
        {
            childrenObjects[i].GetComponent<CardColorSingleUI>().UpdateDisplayCard(colorNumber[i]);
            if(colorNumber[i] == 0)
            {
                childrenObjects[i].SetActive(false);
            } else {
                childrenObjects[i].SetActive(true);
            }
        }
    }

    public Transform GetLocalPosition()
    {
        return this.positionInGame;
    }
}
