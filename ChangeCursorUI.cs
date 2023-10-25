using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ChangeCursorUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Texture2D cursorHand;
    [SerializeField] private Texture2D cursorArrow;

    public void OnPointerEnter(PointerEventData eventData)
    {
        Cursor.SetCursor(cursorHand, Vector2.zero, CursorMode.Auto);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Cursor.SetCursor(cursorArrow, Vector2.zero, CursorMode.Auto);
    }
}
