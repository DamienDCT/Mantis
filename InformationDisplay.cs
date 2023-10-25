using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InformationDisplay : MonoBehaviour
{
    public static InformationDisplay Instance;

    [SerializeField] private TextMeshPro informationText;
    [SerializeField] private TextMeshPro deckInformationText;

    private void Awake()
    {
        Instance = this;
    }

    public void RemoveText()
    {
        informationText.text = "";
    }

    public void DisplayInformation(string informationToDisplay)
    {
        informationText.text = informationToDisplay;
    }

    public void DisplayInformationDeck(int nbCardRemaining)
    {
        deckInformationText.text = "" + nbCardRemaining;
    }
    
}
