using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardDictionary : MonoBehaviour
{
    public static CardDictionary Instance;

    private void Awake()
    {
        Instance = this;
    }



    [SerializeField] private List<CardColor> cardColors;
    [SerializeField] private List<Color> colorsList;

    public CardColor GetCardColorFromColorId(int colorId)
    {
        return cardColors[colorId];
    }

    public List<CardColor> GiveTwoRandomColors(CardColor cardColor){
        List<CardColor> colorsGenerated = new List<CardColor>();
        colorsGenerated.Add(cardColor);
        int nbColorsGenerated = 0;
        while(nbColorsGenerated < 2)
        {
            int colorId = Random.Range(0,7);
            if(!colorsGenerated.Contains(cardColors[colorId]))
            {
                colorsGenerated.Add(cardColors[colorId]);
                nbColorsGenerated++;
            }
        }
        return colorsGenerated;
    }

    public Color GetColorFromCardColor(CardColor cardColor)
    {
        return colorsList[cardColors.IndexOf(cardColor)];
    }
    
    public int GetColorIdFromCardColor(CardColor cardColor)
    {
        return cardColors.IndexOf(cardColor);
    }
}

[System.Serializable]
public enum CardColor {
    ORANGE,
    YELLOW,
    GREEN,
    PINK,
    BLUE,
    PURPLE,
    RED
}
