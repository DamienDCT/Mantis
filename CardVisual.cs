using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CardVisual : MonoBehaviour
{
    [Header("Visual parts")]
    [SerializeField] private SpriteRenderer topCircle;
    [SerializeField] private SpriteRenderer middleCircle;
    [SerializeField] private SpriteRenderer bottomCircle;

    [SerializeField] private SpriteRenderer topRainbow;
    [SerializeField] private SpriteRenderer middleRainbow;
    [SerializeField] private SpriteRenderer bottomRainbow;

    [SerializeField] private Color topColor;
    [SerializeField] private Color middleColor;
    [SerializeField] private Color bottomColor;

    public void UpdateGraphics(Card card){
        topColor = CardDictionary.Instance.GetColorFromCardColor(card.topColorCard);
        middleColor = CardDictionary.Instance.GetColorFromCardColor(card.middleColorCard);
        bottomColor = CardDictionary.Instance.GetColorFromCardColor(card.bottomColorCard);

        topCircle.color = topColor;
        topRainbow.color = topColor;

        middleCircle.color = middleColor;
        middleRainbow.color = middleColor;

        bottomCircle.color = bottomColor;
        bottomRainbow.color = bottomColor;
    }

}
