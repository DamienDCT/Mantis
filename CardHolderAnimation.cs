using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CardHolderAnimation : MonoBehaviour
{
    private Vector3 defaultTransform;
    private Vector3 defaultScale;
    private Vector3 defaultRotation;


    public static CardHolderAnimation Instance;

    private float _timer;
    [SerializeField] private float animationDuration;
    [SerializeField] private Vector3 scaleMax;

    [SerializeField] private Transform[] handsArray;

    [SerializeField] private Sprite transparentSprite;

    [SerializeField] private Transform middleScreenTransform;
    [SerializeField] private Transform handleDeckTransform;

    [SerializeField] private Sprite[] faceCardsSprites;

    [SerializeField] private CardVisual cardVisual;

    private void Awake()
    {
        defaultTransform = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        defaultScale = transform.localScale;
        defaultRotation = transform.rotation.eulerAngles;
        Instance = this;
        _timer = 0f;
    }
    
    
    public void ExitCard(Card card, Vector3 positionPlayer)
    {
        int colorId = CardDictionary.Instance.GetColorIdFromCardColor(card.colorCard);
        cardVisual.gameObject.SetActive(true);
        cardVisual.UpdateGraphics(card);
        GetComponent<SpriteRenderer>().sprite = faceCardsSprites[colorId];
        StartCoroutine(ExitCardFromDeckAnimation(positionPlayer));
    }

    private IEnumerator ExitCardFromDeckAnimation(Vector3 positionPlayer){
        _timer = 0f;
        Debug.Log("coucou in first coroutine");
        while(_timer < animationDuration)
        {
            float lerpRatio = _timer / animationDuration;

            transform.position = Vector3.Lerp(middleScreenTransform.position, handleDeckTransform.position, lerpRatio);
            transform.rotation = Quaternion.Euler(Vector3.Lerp(defaultRotation, new Vector3(0f, 0f, 0f), lerpRatio));

            if(_timer > (animationDuration / 2))
                GetComponent<SpriteRenderer>().sortingOrder = 2;   
            _timer += Time.deltaTime;
            yield return null;
        }
        yield return new WaitForSecondsRealtime(1f);
        StartCoroutine(TransitToHandDeck(positionPlayer));
    }

    private IEnumerator TransitToHandDeck(Vector3 positionPlayer){
        _timer = 0f;
        Vector3 currentPos = transform.position;
        while(_timer < animationDuration)
        {
            float lerpRatio = _timer / animationDuration;

            transform.position = Vector3.Lerp(currentPos, positionPlayer, lerpRatio);
            transform.localScale = Vector3.Lerp(defaultScale, scaleMax, lerpRatio);

            _timer += Time.deltaTime;
            yield return null;
        }
        ResetPositionAnimated();
    }



    private void ResetPositionAnimated(){
        GetComponent<SpriteRenderer>().sprite = transparentSprite;
        transform.position = middleScreenTransform.position;
        transform.localScale = defaultScale;
        transform.rotation = Quaternion.Euler(defaultRotation);
        cardVisual.gameObject.SetActive(false);
        GetComponent<SpriteRenderer>().sortingOrder = 0;
    }
}
