using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Unity.Netcode;
using UnityEngine;

public class CardColorSingleUI : NetworkBehaviour
{
    [SerializeField] private GameObject colorCardPrefab;
    [SerializeField] private float rotationZAxisCard;


    public void UpdateDisplayCard(int amountCards)
    {
        if(amountCards == transform.childCount)
            return;
        else if(amountCards > transform.childCount)
            IncreaseNbCards(amountCards);
        else
            DecreaseNbCards(amountCards);
    }

    private void IncreaseNbCards(int amount)
    {
        for(int i = transform.childCount; i < amount; i++)
        {
            GameObject go = Instantiate(colorCardPrefab);
            go.transform.SetParent(this.transform);
            go.transform.rotation = Quaternion.Euler(0, 0, rotationZAxisCard);
        }
        UpdateTextUI();
    }

    private void DecreaseNbCards(int amount)
    {
        int counter = 0;
        for(int i = amount; i < transform.childCount; i++)
        {
            Destroy(transform.GetChild(counter).gameObject);
            counter++;
        }
        UpdateTextUI();
    }

    private void UpdateTextUI()
    {
        for(int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).GetComponent<AmountCardUI>().Hide();
        }

        transform.GetChild(transform.childCount - 1).GetComponent<AmountCardUI>().UpdateText(transform.childCount);
    }
}
